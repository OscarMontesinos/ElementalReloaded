using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Reflection;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{   
    [HideInInspector]
    public Vector2 inputMov;
    Vector2 inputCursor;
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
    public SpriteRenderer cursorSprite;

    bool camPulledBack;
    bool aimingAssistance = true;

    bool moveBasic;
    bool move1;
    bool move2;
    bool move3;

    float maxRange;

    [HideInInspector]
    public bool isUsingGamepad;

    public void LockPointer(bool value)
    {
        lockPointer = value;
    }

    void Awake()
    {
        cursorSprite.enabled = true;
        rb = character.GetComponent<Rigidbody2D>();
        cam = FindObjectOfType<Camera>();
        cam.transform.parent.GetComponent<CameraController>().playerController = this;
        //character.hpBar.gameObject.SetActive(false);
        //character.stunnBar.gameObject.SetActive(false);
    }

    void Start()
    {
        Cursor.visible = false;


        Instantiate(GameManager.Instance.FoV, transform).GetComponent<FieldOfView>().team = character.team;

        StartCoroutine(PostStart());    

        character.MoveSetUp();
        character.UIManager.UpdateHabIndicatorsImages();

        maxRange = character.currentMoveBasic.range;
        if (maxRange < character.currentMove1.range)
        {
            maxRange = character.currentMove1.range;
        }
        if (maxRange < character.currentMove2.range)
        {
            maxRange = character.currentMove2.range;
        }
        if (maxRange < character.currentMove3.range)
        {
            maxRange = character.currentMove3.range;
        }
    }
    
    IEnumerator PostStart()
    {
        yield return null;
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

                //HandleMovement();
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
        if (!isUsingGamepad)
        {
            if (character.lookAtPointer)
            {
                if (!lockPointer && Time.timeScale != 0)
                {
                    Vector2 dir = UtilsClass.GetMouseWorldPosition() - character.pointer.transform.position;
                    character.pointer.transform.up = dir;
                }
            }
            else if (rb.velocity.magnitude > 1 && !character.lookAtPointer)
            {
                character.pointer.transform.up = rb.velocity.normalized;
            }

            character.cursor.transform.position = UtilsClass.GetMouseWorldPosition();
        }
        else
        {
            if (rb.velocity.magnitude > 1 && !character.lookAtPointer)
            {
                character.pointer.transform.up = rb.velocity.normalized;
            }



            character.cursor.transform.position = (Vector2)character.transform.position + (inputCursor * maxRange);
            if (inputCursor.magnitude == 0)
            {
                character.cursor.transform.position = character.cursor.transform.position + (character.pointer.transform.up * 1.5f);
            }

            if (aimingAssistance)
            {
                Vector2 dir = character.cursor.transform.position - character.transform.position;

                RaycastHit2D[] ray = Physics2D.CircleCastAll((Vector2)character.transform.position + (dir.normalized * 4.5f), 4.5f, dir.normalized, maxRange - 6, GameManager.Instance.unitLayer, -10, 100);

                PjBase target = null;
                int times = 0;

                foreach (RaycastHit2D rayHit in ray)
                {
                    if (target == null || target == character)
                    {
                        target = rayHit.collider.GetComponent<PjBase>();
                        if (target == character || target.hide)
                        {
                            target = null;
                        }
                    }
                    else if (target != null)
                    {
                        Vector2 dist = target.transform.position - character.transform.position;

                        PjBase targetbackUp = target;
                        PjBase target2 = rayHit.collider.GetComponent<PjBase>();
                        Vector2 dist2 = target2.transform.position - character.transform.position;

                        if (!target2.hide)
                        {
                            if (dist2.magnitude < dist.magnitude)
                            {
                                target = target2;
                            }


                            if (target == character)
                            {
                                target = targetbackUp;
                            }
                        }
                    }
                    times++;
                }

                if (target != null && target != character)
                {
                    character.cursor.transform.position = target.transform.position;
                }
            }
        }
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

    void CheckInput(string inputName)
    {
        isUsingGamepad = !(inputName == "Mouse" || inputName == "Keyboard");
    }

    public void BasicMove(InputAction.CallbackContext ctx)
    {
        moveBasic = ctx.action.triggered;

        CheckInput(ctx.action.activeControl.device.displayName);
    }
    public void Move1(InputAction.CallbackContext ctx)
    {
        move1 = ctx.action.triggered;

        CheckInput(ctx.action.activeControl.device.displayName);
    }
    public void Move2(InputAction.CallbackContext ctx)
    {
        move2 = ctx.action.triggered;

        CheckInput(ctx.action.activeControl.device.displayName);

    }
    public void Move3(InputAction.CallbackContext ctx)
    {
        move3 = ctx.action.triggered;

        CheckInput(ctx.action.activeControl.device.displayName);
    }

    public void PullBackCamera(InputAction.CallbackContext ctx)
    {
        camPulledBack = !camPulledBack;
        if (camPulledBack)
        {
            CameraController.Instance.PullBack();
        }
        else
        {
            CameraController.Instance.PullForward();
        }

        CheckInput(ctx.action.activeControl.device.displayName);
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        character.UIManager.Pause();

        CheckInput(ctx.action.activeControl.device.displayName);
    }

    public void ChangeAimingAssistance(InputAction.CallbackContext ctx)
    {
        aimingAssistance = !aimingAssistance;

        CheckInput(ctx.action.activeControl.device.displayName);
    }
        void HandleHabilities()
    {
        if (character.stunTime <= 0)
        {
            if (moveBasic)
            {
                StartCoroutine(character.MainAttack());
            }
            if (move1)
            {
                StartCoroutine(character.Hab1());
            }
            if (move2)
            {
                StartCoroutine(character.Hab2());
            }
            if (move3)
            {
                StartCoroutine(character.Hab3());
            }
        }
    }

    public void OnMove(InputAction.CallbackContext ctx) 
    {
        inputMov = ctx.ReadValue<Vector2>();

        CheckInput(ctx.action.activeControl.device.displayName);
    }
    public void OnCursor(InputAction.CallbackContext ctx) 
    { 
        inputCursor = ctx.ReadValue<Vector2>();

        CheckInput(ctx.action.activeControl.device.displayName);
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
