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
        public Transform bound1, bound2;
        public ConnectionWithPoints(Transform bound1, Transform bound2)
        {
            connection = Object.Instantiate(EditorManager.Instance.boundsConnectionObject, Vector2.zero, quaternion.identity).GetComponent<BoundConnection>();
            this.bound1 = bound1;
            this.bound2 = bound2;
            connection.SetConnection(bound1, bound2);
        }
    }


    public List<Transform> boundPoints;
    public List<ConnectionWithPoints> boundConnections;
    Transform selected;
    bool editing = false;
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
        boundConnections.Add(new ConnectionWithPoints(prevBound, boundConnections[0].bound1));
        boundConnections[boundConnections.Count - 1].connection.gameObject.name = "connection" + (boundConnections.Count - 1);
        RefreshConnections();
    }

    public override void EditorUpdate(EditorManager editor)
    {

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
        List<Vector3> points = new List<Vector3>();
        foreach (Transform bound in boundPoints)
        {
            points.Add(bound.position);
        }
        editor.dynamicMeshGenerator.UpdateMeshEditor(points);


        if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("bounds", "terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log(hit.collider.gameObject.layer + " -- " + LayerMask.NameToLayer("bounds"));
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("bounds"))
                {
                    if (boundPoints.Count <= 3) { ErrorManager.Instance.SendError("You can not have less than 3 bound points!"); return; }

                    boundPoints.Remove(hit.collider.transform);
                    GameObject.Destroy(hit.collider.gameObject);
                }
                else
                {
                    Transform bound = CreateBoundPoint(editor, new Vector2(hit.point.x, hit.point.z));
                    if (bound != null)
                        boundPoints.Add(bound);
                }
            }
        }
    }

    Transform CreateBoundPoint(EditorManager editor, Vector2 point)
    {
        Transform bound = null;

        bound = GameObject.Instantiate(editor.boundPrefab).transform;
        bound.transform.position = new Vector3(point.x, 0, point.y);
        bound.gameObject.name = "bound" + boundPoints.Count.ToString();
        Ground(bound);
        return bound;


    }

    void Ground(Transform obj)
    {

        RaycastHit hit;
        if (!Physics.Raycast(obj.transform.position + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
        { Debug.Log("erro"); return; }
        obj.transform.position = new Vector3(obj.transform.position.x, hit.point.y, obj.transform.position.z);

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
