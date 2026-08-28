using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DynamicMeshGenerator : MonoBehaviour
{
    public MeshCollider col;
    public MeshRenderer rend;
    public MeshFilter filter;
    public Material material;
    public float tilingU = 1f;
    public float tilingV = 1f;
    private Mesh dynamicMesh;
    public float meshHeight = 152f;

    private void Awake()
    {
        // Get/create components.
        if (col == null)
            col = GetComponent<MeshCollider>();

        if (rend == null)
            rend = GetComponent<MeshRenderer>();

        if (filter == null)
            filter = GetComponent<MeshFilter>();

        if (rend == null)
            rend = gameObject.AddComponent<MeshRenderer>();

        if (filter == null)
            filter = gameObject.AddComponent<MeshFilter>();

        // Create the mesh ONCE.
        dynamicMesh = new Mesh
        {
            name = $"{gameObject.name}_DynamicMesh"
        };

        dynamicMesh.MarkDynamic();

        // Prevent this runtime-generated mesh from being saved.
        dynamicMesh.hideFlags = HideFlags.DontSave;

        // Assign the same mesh to both.
        filter.sharedMesh = dynamicMesh;

        if (col != null)
            col.sharedMesh = dynamicMesh;

        // sharedMaterial avoids creating a material instance.
        if (rend != null)
            rend.sharedMaterial = material;
    }


    private void Start()
    {
        if (col == null)
            //col = gameObject.AddComponent<MeshCollider>();
            if (rend == null)
                rend = gameObject.AddComponent<MeshRenderer>();
        if (filter == null)
            filter = gameObject.AddComponent<MeshFilter>();
        rend.material = material;
        // dynamicMesh.MarkDynamic();
    }

    public void SetMeshVisibility(bool visible)
    {
        if (rend != null)
            rend.enabled = visible;

    }

    public void UpdateMeshEditor(List<Vector3> bounds)
    {
        if (bounds == null || bounds.Count < 3)
        {
            ClearMesh();
            return;
        }

        Vector3[] convexPoints = bounds.ToArray();

        GenerateMesh(convexPoints);
    }


    public void SetMeshOnPlay(Vector2[] bounds)
    {
        if (bounds == null || bounds.Length < 3)
        {
            Debug.LogWarning(
                "Bounds in save file have less than 3 points...",
                this
            );

            ClearMesh();
            return;
        }

        Vector3[] points = new Vector3[bounds.Length];

        for (int i = 0; i < bounds.Length; i++)
        {
            points[i] = new Vector3(
                bounds[i].x,
                0f,
                bounds[i].y
            );
        }

        GenerateMesh(points);

    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }

    public void GenerateMesh(Vector3[] points)
    {
        if (dynamicMesh == null)
            return;

        if (points == null || points.Length < 3)
        {
            ClearMesh();
            return;
        }

        int pointCount = points.Length;

        Vector3[] verts = new Vector3[pointCount * 4];
        List<int> tris = new List<int>((pointCount) * 6);

        // Generate the vertical wall sections.
        for (int i = 0; i < pointCount - 1; i++)
        {
            int vertexIndex = i * 4;

            verts[vertexIndex] =
                points[i];

            verts[vertexIndex + 1] =
                points[i] + Vector3.up * meshHeight;

            verts[vertexIndex + 2] =
                points[i + 1];

            verts[vertexIndex + 3] =
                points[i + 1] + Vector3.up * meshHeight;

            tris.Add(vertexIndex);
            tris.Add(vertexIndex + 1);
            tris.Add(vertexIndex + 2);

            tris.Add(vertexIndex + 1);
            tris.Add(vertexIndex + 3);
            tris.Add(vertexIndex + 2);
        }

        // Close the loop.
        int lastVertex = verts.Length - 4;

        verts[lastVertex] =
            points[pointCount - 1];

        verts[lastVertex + 1] =
            points[pointCount - 1] + Vector3.up * meshHeight;

        verts[lastVertex + 2] =
            points[0];

        verts[lastVertex + 3] =
            points[0] + Vector3.up * meshHeight;

        tris.Add(lastVertex);
        tris.Add(lastVertex + 1);
        tris.Add(lastVertex + 2);

        tris.Add(lastVertex + 1);
        tris.Add(lastVertex + 3);
        tris.Add(lastVertex + 2);

        // Replace the contents of the existing mesh.
        dynamicMesh.Clear();

        dynamicMesh.vertices = verts;
        dynamicMesh.triangles = tris.ToArray();

        dynamicMesh.uv =
            CalcUVPerFace(verts);

        dynamicMesh.uv2 =
            CalcUVAllStreach(verts);

        dynamicMesh.uv3 =
            CalcUVAllTile(verts);

        dynamicMesh.RecalculateBounds();

        // Keep the references pointing at the same mesh.
        if (filter != null)
            filter.sharedMesh = dynamicMesh;

        if (col != null)
        {
            col.sharedMesh = null;
            col.sharedMesh = dynamicMesh;
        }
    }


    public Vector2[] CalcUVPerFace(Vector3[] verts)
    {
        Vector2[] uvs = new Vector2[verts.Length];

        for (int i = 0; i < verts.Length / 4; i++)
        {
            int index = i * 4;

            uvs[index] =
                new Vector2(0f, 0f);

            uvs[index + 1] =
                new Vector2(0f, 1f);

            uvs[index + 2] =
                new Vector2(1f, 0f);

            uvs[index + 3] =
                new Vector2(1f, 1f);
        }

        return uvs;
    }


    public Vector2[] CalcUVAllStreach(Vector3[] verts)
    {
        Vector2[] uvs = new Vector2[verts.Length];

        int numOfFaces = verts.Length / 4;

        if (numOfFaces == 0)
            return uvs;

        for (int i = 0; i < numOfFaces; i++)
        {
            int index = i * 4;

            float start =
                i / (float)numOfFaces;

            float end =
                (i + 1) / (float)numOfFaces;

            uvs[index] =
                new Vector2(start, 0f);

            uvs[index + 1] =
                new Vector2(start, 1f);

            uvs[index + 2] =
                new Vector2(end, 0f);

            uvs[index + 3] =
                new Vector2(end, 1f);
        }

        return uvs;
    }


    public Vector2[] CalcUVAllTile(Vector3[] verts)
    {
        Vector2[] uvs = new Vector2[verts.Length];

        int numOfFaces = verts.Length / 4;

        if (numOfFaces == 0)
            return uvs;

        Vector3 firstPoint = verts[0];

        float distanceFromStart = 0f;

        for (int i = 0; i < numOfFaces; i++)
        {
            int index = i * 4;

            float faceWidth =
                Vector3.Distance(
                    verts[index],
                    verts[index + 2]
                );

            float x0 =
                distanceFromStart * tilingV / 52f;

            float x1 =
                (distanceFromStart + faceWidth)
                * tilingV / 52f;

            float yBottom =
                firstPoint.y * tilingU / 52f;

            float yTop =
                (firstPoint.y - verts[index + 1].y)
                * tilingU / 52f;

            uvs[index] =
                new Vector2(x0, yBottom);

            uvs[index + 1] =
                new Vector2(x0, yTop);

            uvs[index + 2] =
                new Vector2(x1, yBottom);

            uvs[index + 3] =
                new Vector2(x1, yTop);

            distanceFromStart += faceWidth;
        }

        return uvs;
    }

    public List<Vector3> GetConvexHull(List<Vector3> inputPoints)
    {
        if (inputPoints == null)
            return new List<Vector3>();

        if (inputPoints.Count <= 3)
            return new List<Vector3>(inputPoints);

        // Make a copy so we DON'T modify the caller's list.
        List<Vector3> points =
            new List<Vector3>(inputPoints);

        List<Vector3> convexHull =
            new List<Vector3>();

        Vector3 startVertex = points[0];

        // Find left-most point.
        for (int i = 1; i < points.Count; i++)
        {
            Vector3 testPos = points[i];

            if (testPos.x < startVertex.x ||
                (
                    Mathf.Approximately(
                        testPos.x,
                        startVertex.x
                    )
                    &&
                    testPos.z < startVertex.z
                ))
            {
                startVertex = testPos;
            }
        }

        convexHull.Add(startVertex);
        points.Remove(startVertex);

        Vector3 currentPoint =
            convexHull[0];

        List<Vector3> colinearPoints =
            new List<Vector3>();

        int counter = 0;

        while (true)
        {
            if (counter == 2)
            {
                points.Add(convexHull[0]);
            }

            if (points.Count == 0)
                break;

            Vector3 nextPoint =
                points[Random.Range(0, points.Count)];

            Vector2 a =
                new Vector2(
                    currentPoint.x,
                    currentPoint.z
                );

            Vector2 b =
                new Vector2(
                    nextPoint.x,
                    nextPoint.z
                );

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(nextPoint))
                    continue;

                Vector2 c =
                    new Vector2(
                        points[i].x,
                        points[i].z
                    );

                float relation =
                    CheckPositionBasedOnLine(
                        a,
                        b,
                        c
                    );

                const float accuracy = 0.00001f;

                if (relation < accuracy &&
                    relation > -accuracy)
                {
                    colinearPoints.Add(
                        points[i]
                    );
                }
                else if (relation < 0f)
                {
                    nextPoint = points[i];

                    b =
                        new Vector2(
                            nextPoint.x,
                            nextPoint.z
                        );

                    colinearPoints.Clear();
                }
            }

            if (colinearPoints.Count > 0)
            {
                colinearPoints.Add(nextPoint);

                convexHull.AddRange(
                    colinearPoints
                );

                currentPoint =
                    colinearPoints[
                        colinearPoints.Count - 1
                    ];

                for (int i = 0;
                     i < colinearPoints.Count;
                     i++)
                {
                    points.Remove(
                        colinearPoints[i]
                    );
                }

                colinearPoints.Clear();
            }
            else
            {
                convexHull.Add(nextPoint);

                points.Remove(nextPoint);

                currentPoint = nextPoint;
            }

            if (currentPoint.Equals(
                    convexHull[0]))
            {
                convexHull.RemoveAt(
                    convexHull.Count - 1
                );

                break;
            }

            counter++;
        }

        return convexHull;
    }


    public float CheckPositionBasedOnLine(Vector2 linePointA, Vector2 linePointB, Vector2 point)
    {
        float fx = linePointB.x - linePointA.x;
        float fy = linePointB.y - linePointA.y;
        return fx * (point.y - linePointA.y) - fy * (point.x - linePointA.x);
    }

    private void ClearMesh()
    {
        if (dynamicMesh == null)
            return;

        dynamicMesh.Clear();

        if (filter != null)
            filter.sharedMesh = dynamicMesh;

        if (col != null)
            col.sharedMesh = dynamicMesh;
    }


    private void OnDestroy()
    {
        // Detach mesh first.
        if (filter != null)
            filter.sharedMesh = null;

        if (col != null)
            col.sharedMesh = null;

        // Destroy the mesh we created.
        if (dynamicMesh != null)
        {
            Destroy(dynamicMesh);
            dynamicMesh = null;
        }
    }



}

