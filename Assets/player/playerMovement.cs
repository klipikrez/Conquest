using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    public CharacterController controller;
    public float speed = 1f;
    public float mouseSpeed = 20f;

    public float zoomLevel = 5f;
    public float zoomSpeed = 30f;
    public float zoomChange = 5f;
    public float minZoom = 0;
    public float maxZoom = 10;

    void Update()
    {
        Move();
        Zoom();
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
            hor -= Input.GetAxis("Mouse X") * mouseSpeed;
            vert -= Input.GetAxis("Mouse Y") * mouseSpeed;
        }

        if (Input.GetMouseButtonUp(2))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Vector3 moveVector = transform.right * hor + transform.forward * vert;

        controller.Move(moveVector * Time.deltaTime * (zoomLevel/5));

    }

    void Zoom()
    {

        RaycastHit hit;
        if(!Physics.Raycast(transform.position,Vector3.down, out hit, 100f, LayerMask.GetMask("terrain")))
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

        float Y = Mathf.Lerp(0, offset, Mathf.Clamp01(zoomSpeed * Time.deltaTime));
        Vector3 moveVector = new Vector3(0, Y, 0);

        controller.Move(moveVector);
        
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
}
