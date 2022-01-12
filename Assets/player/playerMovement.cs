using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    public CharacterController controller;
    public Camera payerCamera;
    Vector3 baseCameraAnchor;
    Vector3 previousPosition;
    public float speed = 1f;
    public float mouseSpeed = 20f;

    public float zoomLevel = 5f;
    public float zoomSpeed = 30f;
    public float zoomChange = 5f;
    public float minZoom = 0;
    public float maxZoom = 10;
    public float minZoomRotation = 35;
    public float maxZoomRotation = 60;

    public bool viewProjection = false;


    private void Start()
    {
        baseCameraAnchor = transform.position;
        Zoom(1);
    }
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            viewProjection = !viewProjection;
            if (viewProjection)
            {
                payerCamera.orthographic = true;
                payerCamera.cullingMask -= LayerMask.GetMask("unit");
                payerCamera.cullingMask += LayerMask.GetMask("unitMarker");
                payerCamera.cullingMask -= LayerMask.GetMask("building");
                payerCamera.cullingMask += LayerMask.GetMask("buildingMarker");
                payerCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                previousPosition = transform.position;
            }
            else
            {
                payerCamera.orthographic = false;
                payerCamera.cullingMask += LayerMask.GetMask("unit");
                payerCamera.cullingMask -= LayerMask.GetMask("unitMarker");
                payerCamera.cullingMask += LayerMask.GetMask("building");
                payerCamera.cullingMask -= LayerMask.GetMask("buildingMarker");
                controller.Move(new Vector3(previousPosition.x - transform.position.x, 0, previousPosition.z - transform.position.z));//reset camera to previous position
            }

        }

        if (viewProjection)
        {
            SpaceAction();
        }
        else
        {
            Move();
            Zoom(zoomSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        float vert = Input.GetAxis("Vertical") * speed;
        float hor = Input.GetAxis("Horizontal") * speed;

        if (Input.GetMouseButtonDown(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButton(2))
        {
            hor -= Input.GetAxis("Mouse X") * mouseSpeed / 2;
            vert -= Input.GetAxis("Mouse Y") * mouseSpeed / 2;
        }

        if (Input.GetMouseButtonUp(2))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Vector3 moveVector = transform.right * hor + transform.forward * vert;

        controller.Move(moveVector * Time.deltaTime * (zoomLevel / 5));

    }

    void Zoom(float moveAmount)
    {

        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, 800f, LayerMask.GetMask("terrain")))
        {
            //ako nema dno  hit.distance = zoomLevel; da bi kamera ostala u mestu
            Debug.Log("nes nera di bur z");
            hit.distance = zoomLevel;
            //ovo nikad ne treba da se desi!
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            UpdateZoomLevel();
        }

        float offset = zoomLevel - hit.distance;

        float Y = Mathf.Lerp(0, offset, Mathf.Clamp01(moveAmount));
        Vector3 moveVector = new Vector3(0, Y, 0);

        controller.Move(moveVector);
        SetCameraRotation(hit.distance);
        //COOLmaterial;
        //laseri i vatromet :)
    }

    void UpdateZoomLevel()
    {
        if (zoomLevel - Input.GetAxis("Mouse ScrollWheel") * zoomChange > maxZoom)
        {
            zoomLevel = maxZoom;
        }
        else
        {
            if (zoomLevel - Input.GetAxis("Mouse ScrollWheel") * zoomChange < minZoom)
            {
                zoomLevel = minZoom;
            }
            else
            {
                zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomChange;

            }
        }
    }

    void SetCameraRotation(float height)
    {
        float amount = Mathf.Pow((minZoom - height + 10) / 10, 3f);
        float lerpFloat = Mathf.Lerp(maxZoomRotation, minZoomRotation, amount);
        //Quaternion target = Quaternion.Lerp(Quaternion.Euler(35, 0, 0), Quaternion.Euler(80, 0, 0), (height - minZoom) / 10);

        Quaternion target = Quaternion.Euler(lerpFloat, 0, 0);
        if (payerCamera.transform.rotation.eulerAngles != target.eulerAngles)
        {
            payerCamera.transform.rotation = target;
        }
    }

    void SpaceAction()
    {
        Vector3 acc = new Vector3(
            Mathf.Lerp(transform.position.x, baseCameraAnchor.x, Time.deltaTime),
            baseCameraAnchor.y,
            Mathf.Lerp(transform.position.z, baseCameraAnchor.z, Time.deltaTime));

        controller.Move(acc - transform.position);
    }
}
