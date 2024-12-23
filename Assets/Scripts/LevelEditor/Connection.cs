using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Connection : MonoBehaviour
{

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


    public void CalculateFollowTerrain(Vector3 p1, Vector3 p2)
    {
        // Debug.Log(line1.pos"");
        float distance = Vector3.Distance(p1, p2);
        line1.positionCount = (int)distance + 1;
        if (line2 != null) line2.positionCount = (int)distance + 1;

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
                if (line2 != null) line2.SetPosition(line2.positionCount - 1, p1);
                continue;
            }

            newPoint = Vector3.Lerp(p1, p2, (float)i / distance);
            if (!Physics.Raycast(newPoint + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
            { Debug.Log("erro"); break; }

            pos = new Vector3(newPoint.x, hit.point.y + 2, newPoint.z);
            line1.SetPosition(i, pos);
            if (line2 != null) line2.SetPosition(line2.positionCount - i - 1, pos);
        }
        line1.SetPosition(line1.positionCount - 1, p2);
        if (line2 != null) line2.SetPosition(0, p2);
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


}
