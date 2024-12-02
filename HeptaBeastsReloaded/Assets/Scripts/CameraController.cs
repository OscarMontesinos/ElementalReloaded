using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public GameObject fog;
    public PlayerController playerController;
    public float speed;
    public float rSpeed;
    public float zPos;
    float camPos;
    public float camDistance;
    bool moveAlone;
    public float speedZoom;
    public float zoomOut;
    public float zoomIn;
    public Camera cam;
    bool rotateCamera;
    bool beginExpectate;
    // Start is called before the first frame update
    void Awake()
    {
        zoomIn = cam.orthographicSize;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        zPos = cam.transform.position.z;
        if (playerController != null)
        {
            playerController.cam = transform.GetChild(0).GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cam.orthographicSize = zoomOut;
            fog.SetActive(false);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            cam.orthographicSize = zoomIn;
            fog.SetActive(true);
        }

        if (playerController == null )
        {
            moveAlone = true;
            cam = FindObjectOfType<Camera>();
        }
        else
        {
            transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, playerController.transform.position.z);
        }

        if (moveAlone)
        {
            if (!beginExpectate)
            {
                foreach(PjBase unit in GameManager.Instance.pjList)
                {
                    unit.hide = false;
                }
                fog.SetActive(false);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                cam.orthographicSize += speedZoom;
                if (cam.orthographicSize > 45)
                {
                    cam.orthographicSize = 45;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                cam.orthographicSize -= speedZoom;
                if (cam.orthographicSize < 10)
                {
                    cam.orthographicSize = 10;
                }

            }
            if (Input.GetKey(KeyCode.W))
            {
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + speed * Time.deltaTime, cam.transform.position.z);
            }
            if (Input.GetKey(KeyCode.S))
            {
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - speed * Time.deltaTime, cam.transform.position.z);
            }
            if (Input.GetKey(KeyCode.D))
            {
                cam.transform.position = new Vector3(cam.transform.position.x + speed * Time.deltaTime, cam.transform.position.y, cam.transform.position.z);
            }
            if (Input.GetKey(KeyCode.A))
            {
                cam.transform.position = new Vector3(cam.transform.position.x - speed * Time.deltaTime, cam.transform.position.y, cam.transform.position.z);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed *= 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed /= 2;
            }
        }
        else
        {


            /*
            if (rotateCamera)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, playerController.character.spinObjects.transform.rotation, rSpeed * Time.deltaTime);
                if (camPos < camDistance)
                {
                    transform.GetChild(0).Translate(transform.up * Time.deltaTime * speed * 3);
                    camPos += Time.deltaTime * speed * 3;
                }
            }
            else
            {
                if (camPos > 0.1f)
                {
                    transform.GetChild(0).Translate(transform.up * Time.deltaTime * -speed * 3);
                    camPos += Time.deltaTime * -speed * 3;
                }
                else if (camPos <= 0.1f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, playerController.character.spinObjects.transform.rotation, rSpeed * 10 * Time.deltaTime);
                    transform.GetChild(0).transform.localPosition = new Vector3(0, 0, zPos);
                    camPos = 0;
                }
            }
            transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);*/
        }
    }

    public void PullBack()
    {
        cam.orthographicSize += camDistance;
       //rotateCamera = !rotateCamera;
    }
    public void PullForward()
    {
        cam.orthographicSize -= camDistance;
    }
}
