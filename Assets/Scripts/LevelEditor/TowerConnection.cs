using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerConnection : MonoBehaviour
{
    public EditorTower tower1;
    public EditorTower tower2;
    public LineRenderer line1;
    public LineRenderer line2;

    public MeshCollider coll;
    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        //EditorManager.Instance.editorconnections.Add(this);
    }

    private void FixedUpdate()
    {
        SetMesh();
    }

    private void OnDestroy()
    {
        EditorManager.Instance.editorconnections.Remove(this);
    }

    public void SetConnection(EditorTower p1, EditorTower p2)
    {
        tower1 = p1;
        tower2 = p2;
        /*line1.SetPosition(0, p1.transform.position + Vector3.up);
        line1.SetPosition(1, p2.transform.position + Vector3.up);
        line2.SetPosition(0, p2.transform.position + Vector3.up);
        line2.SetPosition(1, p1.transform.position + Vector3.up);*/
        CalculateFollowTerrain(p1.transform.position + Vector3.up, p2.transform.position + Vector3.up);
    }

    public void Drag(EditorTower p1, Vector3 p2)
    {
        /*line1.SetPosition(0, p1.transform.position + Vector3.up);
        line1.SetPosition(1, p2 + Vector3.up);
        line2.SetPosition(0, p2 + Vector3.up);
        line2.SetPosition(1, p1.transform.position + Vector3.up);*/
        CalculateFollowTerrain(p1.transform.position + Vector3.up, p2 + Vector3.up);
    }

    public void UpdatePosition()
    {
        /*line1.SetPosition(0, tower1.transform.position + Vector3.up);
        line1.SetPosition(1, tower2.transform.position + Vector3.up);
        line2.SetPosition(0, tower2.transform.position + Vector3.up);
        line2.SetPosition(1, tower1.transform.position + Vector3.up);*/
        CalculateFollowTerrain(tower1.transform.position + Vector3.up, tower2.transform.position + Vector3.up);
    }

    public void CalculateFollowTerrain(Vector3 p1, Vector3 p2)
    {
        // Debug.Log(line1.pos"");
        float distance = Vector3.Distance(p1, p2);
        line1.positionCount = (int)distance + 1;
        line2.positionCount = (int)distance + 1;

        Vector3 newPoint;
        RaycastHit hit;
        Vector3 pos;

        /*if (Physics.Raycast(p2 + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("building")) && Vector3.Distance(hit.collider.transform.position, p1) > 0.01)
        { p2 = hit.collider.transform.position; }*/

        for (int i = 0; i < distance - 1; i++)
        {
            if (i == 0)
            {
                line1.SetPosition(0, p1);
                line2.SetPosition(line2.positionCount - 1, p1);
                continue;
            }

            newPoint = Vector3.Lerp(p1, p2, (float)i / distance);
            if (!Physics.Raycast(newPoint + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
            { Debug.Log("erro"); break; }

            pos = new Vector3(newPoint.x, hit.point.y + 2, newPoint.z);
            line1.SetPosition(i, pos);
            line2.SetPosition(line2.positionCount - i - 1, pos);
        }
        line1.SetPosition(line1.positionCount - 1, p2);
        line2.SetPosition(0, p2);
        /* newPoint = p2;
         if (!Physics.Raycast(newPoint + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
             Debug.Log("erro");
         pos = new Vector3(newPoint.x, hit.point.y + 2, newPoint.z);
         line1.SetPosition(line1.positionCount - 1, pos);
         line2.SetPosition(0, pos);*/

        SetMesh();
    }

    void SetMesh()
    {
        Mesh mesh = new Mesh();
        line1.BakeMesh(mesh, true);
        coll.sharedMesh = mesh;
    }

    public void Delete()
    {
        tower1.RemoveConnection(this);
        tower2.RemoveConnection(this);
        Destroy(gameObject);
    }

    public void CycleConnectionType()
    {
        tower1.RemoveConnection(this);
        tower2.RemoveConnection(this);

        if (line1.enabled && line2.enabled)
        {
            Debug.Log("cycle tower connection mode - 1");
            line1.enabled = false; line2.enabled = true;
            //tower1.RemoveConnection(this);
            tower2.AddConnection(tower1);
            return;
        }

        if (!line1.enabled && line2.enabled)
        {
            Debug.Log("cycle tower connection mode - 2");
            line1.enabled = true; line2.enabled = false;
            tower1.AddConnection(tower2);
            //tower2.RemoveConnection(this);
            return;
        }

        Debug.Log("cycle tower connection mode - 3");
        line1.enabled = true; line2.enabled = true;
        tower1.AddConnection(tower2);
        tower2.AddConnection(tower1);
    }

    public void CalculateUppdateConnectionType()
    {

        if (tower1.connections.Contains(this)) { line1.enabled = true; } else { line1.enabled = false; }

        if (tower2.connections.Contains(this)) { line2.enabled = true; } else { line2.enabled = false; }

    }
}
