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

    private bool meshDirty;
    private bool initialized;


    private void Awake()
    {
        // Create the mesh only once.
        colliderMesh = new Mesh
        {
            name = $"{gameObject.name}_ColliderMesh",
            hideFlags = HideFlags.DontSave
        };

        colliderMesh.MarkDynamic();

        if (coll != null)
        {
            coll.sharedMesh = null;
        }

        initialized = true;
    }


    private void Start()
    {
        transform.position = Vector3.zero;

        // Don't bake here if the LineRenderer hasn't been configured yet.
        meshDirty = true;
    }


    private void LateUpdate()
    {
        if (!initialized)
            return;

        if (!meshDirty)
            return;

        colliderUpdateTimer += Time.deltaTime;

        if (colliderUpdateTimer >= colliderUpdateInterval)
        {
            colliderUpdateTimer = 0f;

            BakeColliderMesh();

            meshDirty = false;
        }
    }


    public void CalculateFollowTerrain(Vector3 p1, Vector3 p2)
    {
        if (line1 == null)
            return;

        float distance = Vector3.Distance(p1, p2);

        int positionCount = Mathf.Max(
            2,
            Mathf.FloorToInt(distance) + 1
        );

        line1.positionCount = positionCount;

        if (line2 != null)
            line2.positionCount = positionCount;


        Vector3 newPoint;
        RaycastHit hit;
        Vector3 pos;

        int terrainMask = LayerMask.GetMask("terrain");


        for (int i = 0; i < positionCount - 1; i++)
        {
            if (i == 0)
            {
                line1.SetPosition(0, p1);

                if (line2 != null)
                {
                    line2.SetPosition(
                        line2.positionCount - 1,
                        p1
                    );
                }

                continue;
            }


            newPoint = Vector3.Lerp(
                p1,
                p2,
                (float)i / (positionCount - 1)
            );


            if (!Physics.Raycast(
                    newPoint + Vector3.up * 520f,
                    Vector3.down,
                    out hit,
                    1040f,
                    terrainMask))
            {
                Debug.LogWarning(
                    $"Could not find terrain below connection point {newPoint}",
                    this
                );

                continue;
            }


            pos = new Vector3(
                newPoint.x,
                hit.point.y + 2f,
                newPoint.z
            );


            line1.SetPosition(i, pos);


            if (line2 != null)
            {
                line2.SetPosition(
                    line2.positionCount - i - 1,
                    pos
                );
            }
        }


        // Last point.
        line1.SetPosition(
            line1.positionCount - 1,
            p2
        );


        if (line2 != null)
        {
            line2.SetPosition(
                0,
                p2
            );
        }


        // Tell LateUpdate that the LineRenderer changed.
        meshDirty = true;
    }


    private void BakeColliderMesh()
    {
        if (colliderMesh == null)
            return;

        if (coll == null)
            return;

        if (line1 == null)
            return;

        if (!line1.enabled)
            return;

        // A LineRenderer with fewer than 2 points
        // doesn't have useful geometry to bake.
        if (line1.positionCount < 2)
            return;


        // Bake into the SAME mesh.
        colliderMesh.Clear();

        line1.BakeMesh(
            colliderMesh,
            true
        );


        // BakeMesh can still produce an empty mesh
        // if the LineRenderer has no actual geometry.
        if (colliderMesh.vertexCount == 0)
        {
            coll.sharedMesh = null;
            return;
        }


        // Force MeshCollider to recognize the updated mesh.
        coll.sharedMesh = null;
        coll.sharedMesh = colliderMesh;
    }


    /// <summary>
    /// Forces the collider to be regenerated
    /// on the next collider update tick.
    /// </summary>
    public void RefreshCollider()
    {
        meshDirty = true;

        // If you want an immediate refresh instead,
        // uncomment the next line:
        //
        // BakeColliderMesh();
    }


    protected virtual void OnDestroy()
    {
        // Detach mesh from collider first.
        if (coll != null)
        {
            coll.sharedMesh = null;
        }


        // Destroy the runtime-generated mesh.
        if (colliderMesh != null)
        {
            Destroy(colliderMesh);
            colliderMesh = null;
        }
    }
}

