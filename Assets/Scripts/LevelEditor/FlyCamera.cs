using UnityEngine;
using System.Collections;
using System.Drawing;

public class FlyCamera : MonoBehaviour
{

    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


    public float mainSpeed = 100.0f; //regular speed
    public float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    public float maxShift = 1000.0f; //Maximum speed when holdin gshift
    public float camSens = 0.25f; //How sensitive it with mouse
    private float totalRun = 1.0f;
    public Camera PlayerCamera;
    bool wrapMouse = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            wrapMouse = true;
            //UnityEditor.EditorGUIUtility.SetWantsMouseJumping(1);
        }
        if (Input.GetMouseButton(2))
        {

            Vector3 look = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            look *= camSens;

            float camRotationX = PlayerCamera.transform.rotation.eulerAngles.x >= 270 ? PlayerCamera.transform.rotation.eulerAngles.x - 360f : PlayerCamera.transform.rotation.eulerAngles.x;

            camRotationX -= look.y;
            camRotationX = Mathf.Clamp(camRotationX, -90f, 90f);

            PlayerCamera.gameObject.transform.eulerAngles = new Vector3(
                camRotationX,
                PlayerCamera.gameObject.transform.eulerAngles.y,
                PlayerCamera.gameObject.transform.eulerAngles.z);


            PlayerCamera.transform.parent.transform.Rotate(new Vector3(0, look.x, 0));


            /*lastMouse = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            //lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            //Debug.Log(lastMouse);
            transform.Rotate(lastMouse);*/
        }
        if (Input.GetMouseButtonUp(2))
        {
            //UnityEditor.EditorGUIUtility.SetWantsMouseJumping(0);
            wrapMouse = false;
        }

        if (wrapMouse)
            WrapCursor();


        //Keyboard commands
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftControl))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            Vector3 newPosition = PlayerCamera.transform.position;
            if (!Input.GetKey(KeyCode.Space))
            { //If player wants to move on X and Z axis only
                transform.Translate(p, PlayerCamera.transform.parent);
                /*newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;*/
            }
            else
            {
                transform.Translate(p, PlayerCamera.transform);
            }
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        return p_Velocity;
    }



    void WrapCursor()
    {
        // Get the mouse position in world coordinates
        Vector3 pos = Input.mousePosition;

        //Debug.Log(Screen.width);
        // Check if the mouse is outside the screen boundaries
        if (pos.x >= Screen.width - 2)
        {
            UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(new Vector2(5, pos.y));
        }
        else if (pos.x <= 0 + 2)
        {
            UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(new Vector2(Screen.width - 5, pos.y));
        }

        if (pos.y >= Screen.height - 2)
        {
            UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(new Vector2(pos.x, 5));
        }
        else if (pos.y <= 0 + 2)
        {
            UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(new Vector2(pos.x, Screen.height - 5));
        }

        // Update the mouse position

    }
}