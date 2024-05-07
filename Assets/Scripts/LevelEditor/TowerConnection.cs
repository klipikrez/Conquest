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
        line1.positionCount = (int)distance;
        line2.positionCount = (int)distance;
        for (int i = 0; i < distance - 1; i++)
        {
            Vector3 newPoint = Vector3.Lerp(p1, p2, (float)i / distance);
            RaycastHit hit;
            if (!Physics.Raycast(newPoint + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
                Debug.Log("erro");
            Vector3 pos = new Vector3(newPoint.x, hit.point.y + 2, newPoint.z);
            line1.SetPosition(i, pos);
            line2.SetPosition(line2.positionCount - i - 1, pos);
        }
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
        if (line1.enabled && line2.enabled)
        {
            line1.enabled = false; line2.enabled = true;
            return;
        }

        if (!line1.enabled && line2.enabled)
        {
            line1.enabled = true; line2.enabled = false;
            return;
        }
        line1.enabled = true; line2.enabled = true;
    }
}
