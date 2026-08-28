using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Connection : MonoBehaviour
{
    [Header("Line Renderers")]
    public LineRenderer line1;
    public LineRenderer line2;
    [Header("Collider")]
    public MeshCollider coll;

    [Header("Collider Update")]
    [Tooltip("How often the collider mesh is regenerated, in seconds.")]
    [Min(0.01f)]
    public float colliderUpdateInterval = 0.1f;
    private Mesh colliderMesh;
    private float colliderUpdateTimer;


    private void Awake()
    {
        // Create the mesh only ONCE.
        colliderMesh = new Mesh
        {
            name = $"{gameObject.name}_ColliderMesh"
        };

        // Prevent Unity from saving this mesh as an asset.
        colliderMesh.hideFlags = HideFlags.DontSave;

        if (coll != null)
        {
            coll.sharedMesh = colliderMesh;
        }
    }

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        //EditorManager.Instance.editorconnections.Add(this);
        SetMesh();

    }

    private void LateUpdate()
    {
        colliderUpdateTimer += Time.deltaTime;

        if (colliderUpdateTimer >= colliderUpdateInterval)
        {
            colliderUpdateTimer = 0f;
            SetMesh();
        }
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


    private void SetMesh()
    {
        if (colliderMesh == null)
            return;

        if (coll == null)
            return;

        if (line1 == null)
            return;

        // IMPORTANT:
        // Reuse the existing mesh instead of creating a new one.
        colliderMesh.Clear();

        line1.BakeMesh(colliderMesh, true);

        coll.sharedMesh = colliderMesh;
    }

    protected virtual void OnDestroy()
    {
        // Detach the mesh from the collider first.
        if (coll != null)
        {
            coll.sharedMesh = null;
        }

        // Destroy the mesh we created in Awake().
        if (colliderMesh != null)
        {
            Destroy(colliderMesh);
            colliderMesh = null;
        }
    }


}
