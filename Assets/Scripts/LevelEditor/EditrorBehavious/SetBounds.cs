using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;



public class SetBounds : EditorBehaviour
{
    public class ConnectionWithPoints
    {
        public BoundConnection connection;
        public ConnectionWithPoints(Transform bound1, Transform bound2)
        {
            connection = Object.Instantiate(EditorManager.Instance.boundsConnectionObject, Vector2.zero, quaternion.identity).GetComponent<BoundConnection>();

            connection.SetConnection(bound1, bound2);
        }
    }


    public List<Transform> boundPoints;
    public List<ConnectionWithPoints> boundConnections;
    Transform selected;
    bool editing = false;
    bool placed = false;
    public override void ChangedEditorMode(EditorManager editor)
    {
        editor.dynamicMeshGenerator.SetMeshVisibility(true);
        boundPoints = new List<Transform>();
        boundConnections = new List<ConnectionWithPoints>();
        if (editor.bounds.Count == 0)
        {
            editor.bounds.Add(new Vector2(1.0001f, 1.0001f) * 5);
            editor.bounds.Add(new Vector2(1.0001f, -1.0001f) * 5);
            editor.bounds.Add(new Vector2(-1.0001f, -1.0001f) * 5);
            editor.bounds.Add(new Vector2(-1.0001f, 1.0001f) * 5);

        }

        Transform prevBound = null;

        foreach (Vector2 point in editor.bounds)
        {

            Transform bound = CreateBoundPoint(editor, point);
            if (bound != null)
                boundPoints.Add(bound);

            if (prevBound != null)
            {
                boundConnections.Add(new ConnectionWithPoints(prevBound, bound));
                boundConnections[boundConnections.Count - 1].connection.gameObject.name = "connection" + (boundConnections.Count - 1);
            }

            prevBound = bound;
        }
        boundConnections.Add(new ConnectionWithPoints(prevBound, boundConnections[0].connection.bound1));
        boundConnections[boundConnections.Count - 1].connection.gameObject.name = "connection" + (boundConnections.Count - 1);

        RefreshConnections();
    }

    public override void EditorUpdate(EditorManager editor)
    {
        //left click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            editor.terrain.drawTreesAndFoliage = false;
            editor.HideObjects();
            editing = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("bounds")) && !EventSystem.current.IsPointerOverGameObject())
            {
                selected = hit.collider.transform;

            }
        }
        if (Input.GetMouseButtonUp(0)) { editor.terrain.drawTreesAndFoliage = true; editor.ShowObjects(); editing = false; editor.RecalculateObjectsHeight(); selected = null; }




        //left click drag
        if (Input.GetMouseButton(0) && editing && selected != null)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                RefreshConnections();
                selected.position = hit.point;
                Ground(selected);

            }
        }

        if (Input.GetMouseButton(0) && editing && selected != null)
        {

        }

        List<Vector3> points = new List<Vector3>();
        foreach (Transform bound in boundPoints)
        {
            points.Add(bound.position);
        }
        editor.dynamicMeshGenerator.UpdateMeshEditor(points);

        //right click up
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("bounds", "connection")) && !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log(hit.collider.gameObject.layer + " -- " + LayerMask.NameToLayer("bounds"));
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("bounds"))
                {
                    if (boundPoints.Count <= 3) { ErrorManager.Instance.SendError("You can not have less than 3 bound points!"); return; }

                    ConnectionWithPoints connection1 = boundConnections.Find(x => x.connection.bound1 == hit.collider.transform);
                    ConnectionWithPoints connection2 = boundConnections.Find(x => x.connection.bound2 == hit.collider.transform);

                    //                    Debug.Log(connection2.bound2 + " ---- " + connection1.bound2);
                    connection2.connection.SetConnection(connection2.connection.bound1, connection1.connection.bound2);

                    boundConnections.Remove(connection1);
                    GameObject.Destroy(connection1.connection.gameObject);

                    boundPoints.Remove(hit.collider.transform);
                    GameObject.Destroy(hit.collider.gameObject);



                    RefreshConnections();
                }
                else
                {
                    Transform bound = CreateBoundPoint(editor, new Vector2(hit.point.x, hit.point.z));
                    if (bound != null)
                    {
                        int index = boundConnections.FindIndex(x => x.connection.transform == hit.collider.gameObject.transform.parent);
                        Debug.Log(hit.collider.gameObject.transform.parent);
                        boundPoints.Insert(++index, bound);
                        RecalculateConnections(editor);
                        RefreshConnections();
                        placed = true;
                        selected = bound;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1) && placed == true && selected != null)
        {
            Debug.Log("aaaa");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                RefreshConnections();
                selected.position = hit.point;
                Ground(selected);

            }
        }

        if (Input.GetMouseButtonUp(1) && placed == true)
        {
            Debug.Log("bbb");
            placed = false;
            selected = null;
            editor.terrain.drawTreesAndFoliage = true; editor.ShowObjects(); editor.RecalculateObjectsHeight();
        }

    }

    Transform CreateBoundPoint(EditorManager editor, Vector2 point)
    {
        Transform bound = null;

        bound = GameObject.Instantiate(editor.boundPrefab).transform;
        bound.transform.position = new Vector3(point.x, 0, point.y);
        bound.gameObject.name = "bound" + boundPoints.Count.ToString();
        if (Ground(bound))
            return bound;
        return null;

    }

    bool Ground(Transform obj)
    {

        RaycastHit hit;
        if (!Physics.Raycast(obj.transform.position + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
        { Debug.Log("erro"); return false; }
        obj.transform.position = new Vector3(obj.transform.position.x, hit.point.y, obj.transform.position.z);
        return true;
    }

    void RecalculateConnections(EditorManager editor)
    {

        foreach (ConnectionWithPoints bound in boundConnections)
        {
            GameObject.Destroy(bound.connection.gameObject);
        }
        boundConnections.Clear();
        Transform prevBound = null;

        foreach (Transform point in boundPoints)
        {


            if (prevBound != null)
            {
                boundConnections.Add(new ConnectionWithPoints(prevBound, point));
                boundConnections[boundConnections.Count - 1].connection.gameObject.name = "connection" + (boundConnections.Count - 1);
            }

            prevBound = point;
        }
        boundConnections.Add(new ConnectionWithPoints(prevBound, boundConnections[0].connection.bound1));
        boundConnections[boundConnections.Count - 1].connection.gameObject.name = "connection" + (boundConnections.Count - 1);

    }

    void RefreshConnections()
    {
        foreach (ConnectionWithPoints cp in boundConnections)
        {
            cp.connection.UpdatePosition();
        }
    }

    void MoveBoundPoint(GameObject bound)
    {

    }

    public override void ExitEditorMode(EditorManager editor)
    {
        editor.bounds.Clear();
        editor.dynamicMeshGenerator.SetMeshVisibility(false);
        foreach (Transform bound in boundPoints)
        {
            editor.bounds.Add(new Vector2(bound.position.x, bound.position.z));
            GameObject.Destroy(bound.gameObject);
        }

        foreach (ConnectionWithPoints bound in boundConnections)
        {
            GameObject.Destroy(bound.connection.gameObject);
        }
    }
}
