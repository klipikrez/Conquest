using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selection : MonoBehaviour
{



    [System.NonSerialized]
    public playerSelectionDictionary selectedDictionary;
    [System.NonSerialized]
    public RaycastHit hit;
    [System.NonSerialized]
    public bool dragSelect;

    //Collider variables
    //=======================================================//

    MeshCollider selectionBox;
    Mesh selectionMesh;
    [System.NonSerialized]
    public Vector3 p1;
    [System.NonSerialized]
    public Vector3 p2;
    Vector3 P1Point;

    //the corners of our 2d selection box
    Vector2[] corners;

    //the vertices of our meshcollider
    Vector3[] verts;
    Vector3[] vecs;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void Select()
    {
        if (Input.GetMouseButtonUp(1))
        {
            p1 = Input.mousePosition;
            RayCastRightClick();

        }

        //1. when left mouse button clicked (but not released)
        if (Input.GetMouseButtonDown(0))
        {
            p1 = Input.mousePosition;
            Ray rej = Camera.main.ScreenPointToRay(p1);
            if (Physics.Raycast(rej, out hit, 50000.0f, LayerMask.GetMask("terrain")))
            {
                P1Point = hit.point;
            }
            else
            {
                P1Point = rej.direction * float.MaxValue;  //Vector3.positiveInfinity;
            }

        }

        //2. while left mouse button held
        if (Input.GetMouseButton(0))
        {
            Ray rej = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rej, out hit, 50000.0f, LayerMask.GetMask("terrain")))
            {
                if ((P1Point - hit.point).magnitude > 0.5f)
                {
                    dragSelect = true;
                }
            }
            else
            {
                if ((p1 - Input.mousePosition).magnitude > 40)
                {
                    dragSelect = true;
                }
            }
        }

        //3. when mouse button comes up
        if (Input.GetMouseButtonUp(0))
        {
            if (dragSelect == false) //single select
            {
                RayCastLeftClick();
            }
            else //marquee select
            {
                p2 = Input.mousePosition;

                p1 = Camera.main.WorldToScreenPoint(P1Point); //p1 to LAst recorded p1 raycast from camera to terrain and again to camera
                verts = new Vector3[4];
                vecs = new Vector3[4];
                int i = 0;

                corners = getBoundingBox(p1, p2);

                foreach (Vector2 corner in corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner);

                    if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")))
                    {
                        //ovde se serem
                        /*verts[i] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        vecs[i] = ray.origin - hit.point;*/
                        verts[i] = new Vector3(hit.point.x - transform.position.x, hit.point.y - transform.position.y, hit.point.z - transform.position.z).normalized * 50000.0f;
                        vecs[i] = (ray.origin - hit.point).normalized * 50000.0f;
                    }
                    i++;
                }

                //generate the mesh
                selectionMesh = generateSelectionMesh(verts, vecs);

                selectionBox = gameObject.AddComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;
                if (!Input.GetKey(KeyCode.LeftShift) && !EventSystem.current.IsPointerOverGameObject())
                {

                    selectedDictionary.RemoveAll();
                }

                Destroy(selectionBox, 0.02f);


                /* { // OLD SELECION
                     RaycastHit hit1;

                     Ray ray = Camera.main.ScreenPointToRay(p1);
                     if (Physics.Raycast(ray, out hit1, 50000.0f, LayerMask.GetMask("terrain")))
                     {
                         RaycastHit hit2;
                         ray = Camera.main.ScreenPointToRay(p2);
                         if (Physics.Raycast(ray, out hit2, 50000.0f, LayerMask.GetMask("terrain")))
                         {
                             boxCollider = gameObject.AddComponent<BoxCollider>();

                             boxCollider.size = new Vector3(Mathf.Abs(hit2.point.x - hit1.point.x),
                             50000.0f,
                             Mathf.Abs(hit2.point.z - hit1.point.z));

                             boxCollider.center = new Vector3((hit1.point.x + hit2.point.x) / 2, -25000.0f, (hit1.point.z + hit2.point.z) / 2);

                             boxCollider.isTrigger = true;
                             if (!Input.GetKey(KeyCode.LeftShift) && !EventSystem.current.IsPointerOverGameObject())
                             {

                                 selectedDictionary.RemoveAll();
                             }
                             Destroy(boxCollider, 0.02f);
                         }
                     }


                 }*/
            }//end marquee select

            dragSelect = false;

        }

    }

    public virtual void RayCastLeftClick()
    {

    }

    public virtual void RayCastRightClick()
    {

    }

    private void OnGUI()
    {
        if (dragSelect == true)
        {
            var rect = Utils.GetScreenRect(Camera.main.WorldToScreenPoint(P1Point), Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }


    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] getBoundingBox(Vector2 p1, Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x < p2.x) //if p1 is to the left of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else //if p1 is below p2
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else //if p1 is to the right of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else //if p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }

        }

        Vector2[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }

    //generate a mesh from the 4 bottom points
    Mesh generateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }




}
