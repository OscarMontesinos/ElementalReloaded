using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System.Reflection;

public class PlayerController : MonoBehaviour
{   
    [HideInInspector]
    public Vector2 inputMov;
    public Camera cam;
    public LayerMask wallLayer;
    float speedDecrease = 5;
    [HideInInspector]
    public Rigidbody2D rb;
    public PjBase character;
    [HideInInspector]
    public bool lockPointer;
    public GameObject targetBoss;
    float maxViewportDistance = 16;

    public void LockPointer(bool value)
    {
        lockPointer = value;
    }

    void Awake()
    {
        rb = character.GetComponent<Rigidbody2D>();
        cam = FindObjectOfType<Camera>();
        cam.transform.parent.GetComponent<CameraController>().playerController = this;
        //character.hpBar.gameObject.SetActive(false);
        //character.stunnBar.gameObject.SetActive(false);
    }

    void Start()
    {
        Instantiate(GameManager.Instance.FoV, transform).GetComponent<FieldOfView>().team = character.team;

        StartCoroutine(PostStart());    

        character.MoveSetUp();
        character.UIManager.UpdateHabIndicatorsImages();
    }
    
    IEnumerator PostStart()
    {
        yield return null;
        foreach (PjBase unit in GameManager.Instance.pjList)
        {
            if (unit.team != character.team)
            {
                unit.hpBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        lockPointer = character.lockPointer;

        if (!Input.GetKey(KeyCode.Tab))
        {
            foreach (PjBase unit in GameManager.Instance.pjList)
            {
                if (unit != null)
                {

                    if (unit.team != character.team)
                    {
                        var dir = unit.transform.position - transform.position;
                        if (!Physics2D.Raycast(transform.position, dir, dir.magnitude, wallLayer))
                        {
                            if (Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.playerWallLayer))
                            {
                                Barrier barrier = Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.playerWallLayer).rigidbody.gameObject.GetComponent<Barrier>();
                                if (barrier.user.team != character.team && barrier.deniesVision)
                                {
                                    unit.hide = true;
                                }
                                else
                                {
                                    unit.hide = false;
                                }
                            }
                            else
                            {
                                unit.hide = false;
                            }
                        }
                        else
                        {
                            unit.hide = true;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (PjBase unit in GameManager.Instance.pjList)
            {
                unit.hide = false;
            }
        }

        if(cam != Camera.main)
        {
            cam = Camera.main;
        }

        if (character != null)
        {
            transform.position = character.transform.position;

            HandleHabilities();

            if (character.stunTime <= 0)
            {
                HandlePointer();

                HandleMovement();
            }

            HandleCamera();
        }



        //if (unit.pointer != null) { unit.pointer.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z)); }




        /*foreach (Unit unit_ in unit.manager.units)
        {
            if (unit_ != null)
            {
                if (unit_.team != unit.team)
                {
                    var dir = unit_.transform.position - transform.position;
                    if (!Physics2D.Raycast(transform.position, dir, dir.magnitude, wallLayer))
                    {
                        unit_.oculto = false;
                    }
                    else
                    {
                        unit_.oculto = true;
                    }
                }
            }
        }*/


        /*if (unit.aim)
        {
            transform.GetChild(0).up = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z)) - transform.position;
        }
        transform.GetChild(0).eulerAngles = new Vector3(0, 0, transform.GetChild(0).eulerAngles.z);
        unit.pointer.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z));*/
    }


    public virtual void FixedUpdate()
    {
        if (character != null)
        {
            if (!character.dashing)
            {
                if (character.stunTime <= 0)
                {
                    rb.velocity = (transform.right * inputMov.x + transform.up * inputMov.y) * character.stats.spd;
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }

            if (rb.velocity.magnitude > 0)
            {
                character.Moving(rb.velocity.magnitude * Time.deltaTime);
                foreach (PjBase pj in GameManager.Instance.pjList)
                {
                    pj.GlobalMoving(rb.velocity.magnitude * Time.deltaTime, character);
                }
            }
        }


    }

    void HandlePointer()
    {
        if (character.lookAtPointer)
        {
            if (!lockPointer && Time.timeScale != 0)
            {
                Vector2 dir = UtilsClass.GetMouseWorldPosition() - character.pointer.transform.position;
                character.pointer.transform.up = dir;
            }
        }
        else
        {
             character.pointer.transform.up = rb.velocity.normalized;
        }
        character.cursor.transform.position = UtilsClass.GetMouseWorldPosition();
    }
    void HandleCamera()
    {
        /* cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
         if (targetBoss == null)
         {
             Camera.main.orthographicSize = maxViewportDistance;
         }
         else
         {
             Vector3 dist = targetBoss.transform.position - character.transform.position;
             dist = character.transform.position + (dist * 0.5f);
             cam.transform.position = new Vector3(dist.x, dist.y , cam.transform.position.z);

             dist = targetBoss.transform.position - character.transform.position;
             if (dist.magnitude > maxViewportDistance)
             {
                 Camera.main.orthographicSize = dist.magnitude;
             }
         }*/
    }

    void HandleHabilities()
    {
        if (Input.GetMouseButton(0))
        {
           StartCoroutine(character.MainAttack());
        }
        if (Input.GetMouseButton(1))
        {
            StartCoroutine(character.Hab1());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(character.Hab2());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(character.Hab3());
        }
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            inputMov.x = Input.GetAxisRaw("Horizontal");
        }
        else if (inputMov.x != 0)
        {
            if (inputMov.x <= 0.2f && inputMov.x >= -0.2f /*|| unit.casting*/)
            {
                inputMov.x = 0;
            }
            if (inputMov.x != 0 && inputMov.x > 0)
            {
                inputMov.x -= speedDecrease * Time.deltaTime;
            }
            if (inputMov.x != 0 && inputMov.x < 0)
            {
                inputMov.x += speedDecrease * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            inputMov.y = Input.GetAxisRaw("Vertical");
        }
        else if (inputMov.y != 0)
        {
            if (inputMov.y <= 0.2f && inputMov.y >= -0.2f /*|| unit.casting*/)
            {
                inputMov.y = 0;
            }
            if (inputMov.y != 0 && inputMov.y > 0)
            {
                inputMov.y -= speedDecrease * Time.deltaTime;
            }
            if (inputMov.y != 0 && inputMov.y < 0)
            {
                inputMov.y += speedDecrease * Time.deltaTime;
            }
        }

    }
}
