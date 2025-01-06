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
        rend.enabled = visible;
    }

    public void UpdateMeshEditor(List<Vector3> bounds)
    {

        //Mesh mesh;
        if (bounds.Count >= 3)
        {




            Vector3[] convxPoints = bounds.ToArray(); ///GetConvexHull(bounds).ToArray();
            Mesh dynamicMesh = GenerateMesh(convxPoints);
            if (col != null) col.sharedMesh = dynamicMesh;
            filter.mesh = dynamicMesh;/**/
        }
        else
        {
            if (filter.mesh != new Mesh() || col.sharedMesh != new Mesh())
            {
                filter.mesh = new Mesh();
                if (col != null) col.sharedMesh = new Mesh();
            }
        }
        //else
        //{
        //    mesh = GenerateMesh(pits);
        //}
        //col.sharedMesh = dynamicMesh;
        //filter.mesh = dynamicMesh;/**/

        /*else
        {
            List<Vector3> PiramidPoints = new List<Vector3>();
            foreach (Vector3 point in pointsssdfdfs)
            {
                PiramidPoints.Add(new Vector3(point.x, -52, point.z));
            }
            //PiramidPoints.AddRange(pointsssdfdfs);
            Vector3[] convxPoints = GetConvexHull(PiramidPoints).ToArray();
            Mesh dynamicMesh = GenerateMesh(convxPoints);
            col.sharedMesh = dynamicMesh;
            filter.mesh = dynamicMesh;
        }*/
    }

    public void SetMeshOnPlay(Vector2[] bounds)
    {
        if (bounds.Length >= 3)
        {

            Vector3[] convxPoints = new Vector3[bounds.Length];

            for (int i = 0; i < bounds.Length; i++)
            {
                convxPoints[i] = new Vector3(bounds[i].x, 0, bounds[i].y);
            }

            Mesh dynamicMesh = GenerateMesh(convxPoints);
            if (col != null) col.sharedMesh = dynamicMesh;
            filter.mesh = dynamicMesh;/**/
        }
        else
        {
            Debug.Log("Bounds in save file have less than 3 points...");
            if (filter.mesh != new Mesh() || col.sharedMesh != new Mesh())
            {
                filter.mesh = new Mesh();
                if (col != null) col.sharedMesh = new Mesh();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }

    public Mesh GenerateMesh(Vector3[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 4];
        //int[] tris = new int[verts.Length * 2];
        List<int> tris = new List<int>();
        for (int i = 0; i < points.Length - 1; i++)
        {
            verts[i * 4] = points[i];
            verts[i * 4 + 1] = points[i] + Vector3.up * 152f;

            verts[i * 4 + 2] = points[i + 1];
            verts[i * 4 + 3] = points[i + 1] + Vector3.up * 152f;

            tris.Add((i + 1) * 4 - 4);
            tris.Add((i + 1) * 4 - 3);
            tris.Add((i + 1) * 4 - 2);

            tris.Add((i + 1) * 4 - 3);
            tris.Add((i + 1) * 4 - 1);
            tris.Add((i + 1) * 4 - 2);

        }

        verts[verts.Length - 4] = points[points.Length - 1];
        verts[verts.Length - 3] = points[points.Length - 1] + Vector3.up * 152f;

        verts[verts.Length - 2] = points[0];
        verts[verts.Length - 1] = points[0] + Vector3.up * 152f;


        tris.Add(verts.Length - 4);
        tris.Add(verts.Length - 3);
        tris.Add(verts.Length - 2);

        tris.Add(verts.Length - 3);
        tris.Add(verts.Length - 1);
        tris.Add(verts.Length - 2);

        /*for (int i = 0; i < verts.Length - 2; i++)
        {
            if (i % 2 == 0)
            {
                tris.Add(i);
                tris.Add(i + 1);
                tris.Add(i + 2);
            }
            else
            {
                tris.Add(i);
                tris.Add(i + 2);
                tris.Add(i + 1);
            }
        }
        tris.Add(verts.Length - 2);
        tris.Add(verts.Length - 1);
        tris.Add(0);
        tris.Add(verts.Length - 1);
        tris.Add(1);
        tris.Add(0);*/
        //int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube



        Mesh generatedMesh = new Mesh();
        generatedMesh.vertices = verts;
        generatedMesh.triangles = tris.ToArray();

        generatedMesh.uv = CalcUVPerFace(generatedMesh.vertices);
        generatedMesh.uv2 = CalcUVAllStreach(generatedMesh.vertices);
        generatedMesh.uv3 = CalcUVAllTile(generatedMesh.vertices);
        generatedMesh.RecalculateBounds();
        return generatedMesh;
        //
        //selectionMesh.RecalculateNormals();

    }

    public Vector2[] CalcUVPerFace(Vector3[] verts)
    {

        Vector2[] uvs = new Vector2[verts.Length];
        for (int i = 0; i < verts.Length / 4; i++)
        {
            uvs[i * 4] = new Vector2(0, 0);
            uvs[i * 4 + 1] = new Vector2(0, 1);
            uvs[i * 4 + 2] = new Vector2(1, 0);
            uvs[i * 4 + 3] = new Vector2(1, 1);
        }
        return uvs;
    }

    public Vector2[] CalcUVAllStreach(Vector3[] verts)
    {
        Vector2[] uvs = new Vector2[verts.Length];
        int numOfFaces = verts.Length / 4;
        //      Debug.Log("=========" + numOfFaces);
        for (int i = 0; i < numOfFaces; i++)
        {
            uvs[i * 4] = new Vector2(i / (float)numOfFaces, 0);
            uvs[i * 4 + 1] = new Vector2(i / (float)numOfFaces, 1);
            uvs[i * 4 + 2] = new Vector2((i + 1) / (float)numOfFaces, 0);
            uvs[i * 4 + 3] = new Vector2((i + 1) / (float)numOfFaces, 1);
            //            Debug.Log(i / (float)numOfFaces + " ==:::== " + (i + 1) / (float)numOfFaces);
        }
        return uvs;
    }

    public Vector2[] CalcUVAllTile(Vector3[] verts)
    {
        Vector2[] uvs = new Vector2[verts.Length];
        int numOfFaces = verts.Length / 4;
        Vector3 firstPoint = verts[0];
        float distanceFromStart = 0;

        //      Debug.Log("=========" + numOfFaces);
        for (int i = 0; i < numOfFaces; i++)
        {
            float faceWidth = Vector3.Distance(verts[i * 4], verts[i * 4 + 2]);

            uvs[i * 4] = new Vector2(distanceFromStart * tilingV / 52, firstPoint.y * tilingU / 52);
            uvs[i * 4 + 1] = new Vector2(distanceFromStart * tilingV / 52, (firstPoint.y - verts[i * 4 + 1].y) * tilingU / 52);
            uvs[i * 4 + 2] = new Vector2((distanceFromStart + faceWidth) * tilingV / 52, firstPoint.y * tilingU / 52);
            uvs[i * 4 + 3] = new Vector2((distanceFromStart + faceWidth) * tilingV / 52, (firstPoint.y - verts[i * 4 + 1].y) * tilingU / 52);
            distanceFromStart += faceWidth;
            //            Debug.Log(i / (float)numOfFaces + " ==:::== " + (i + 1) / (float)numOfFaces);
        }
        return uvs;
    }
    public List<Vector3> GetConvexHull(List<Vector3> points)
    {
        if (points.Count == 3)
        {
            return points;
        }


        List<Vector3> convexHull = new List<Vector3>();

        //Step 1. Find the vertex with the smallest x coordinate
        //If several have the same x coordinate, find the one with the smallest z
        Vector3 startVertex = points[0];

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 testPos = points[i];

            //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
            if (testPos.x < startVertex.x || (Mathf.Approximately(testPos.x, startVertex.x) && testPos.z < startVertex.z))
            {
                startVertex = points[i];
            }
        }

        //This vertex is always on the convex hull
        convexHull.Add(startVertex);

        points.Remove(startVertex);



        //Step 2. Loop to generate the convex hull
        Vector3 currentPoint = convexHull[0];
        List<Vector3> colinearPoints = new List<Vector3>();

        int counter = 0;

        while (true)
        {
            //After 2 iterations we have to add the start position again so we can terminate the algorithm
            //Cant use convexhull.count because of colinear points, so we need a counter
            if (counter == 2)
            {
                points.Add(convexHull[0]);
            }
            if (points.Count == 0)
            {
                break;
            }
            //Pick next point randomly
            Vector3 nextPoint = points[Random.Range(0, points.Count)];

            //To 2d space so we can see if a point is to the left is the vector ab
            Vector2 a = new Vector2(currentPoint.x, currentPoint.z);

            Vector2 b = new Vector2(nextPoint.x, nextPoint.z);

            //Test if there's a point to the right of ab, if so then it's the new b
            for (int i = 0; i < points.Count; i++)
            {
                //Dont test the point we picked randomly
                if (points[i].Equals(nextPoint))
                {
                    continue;
                }

                Vector2 c = new Vector2(points[i].x, points[i].z);

                //Where is c in relation to a-b
                // < 0 -> to the right
                // = 0 -> on the line
                // > 0 -> to the left
                float relation = CheckPositionBasedOnLine(a, b, c);

                //Colinear points
                //Cant use exactly 0 because of floating point precision issues
                //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
                float accuracy = 0.00001f;

                if (relation < accuracy && relation > -accuracy)
                {
                    colinearPoints.Add(points[i]);
                }
                //To the right = better point, so pick it as next point on the convex hull
                else if (relation < 0f)
                {
                    nextPoint = points[i];

                    b = new Vector2(nextPoint.x, nextPoint.z);

                    //Clear colinear points
                    colinearPoints.Clear();
                }
                //To the left = worse point so do nothing
            }



            //If we have colinear points
            if (colinearPoints.Count > 0)
            {
                colinearPoints.Add(nextPoint);

                //Sort this list, so we can add the colinear points in correct order
                //colinearPoints = colinearPoints.Sort((n,n2) => Vector3.SqrMagnitude(n - currentPoint)).ToList();

                convexHull.AddRange(colinearPoints);

                currentPoint = colinearPoints[colinearPoints.Count - 1];

                //Remove the points that are now on the convex hull
                for (int i = 0; i < colinearPoints.Count; i++)
                {
                    points.Remove(colinearPoints[i]);
                }

                colinearPoints.Clear();
            }
            else
            {
                convexHull.Add(nextPoint);

                points.Remove(nextPoint);

                currentPoint = nextPoint;
            }

            //Have we found the first point on the hull? If so we have completed the hull
            if (currentPoint.Equals(convexHull[0]))
            {
                //Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
                convexHull.RemoveAt(convexHull.Count - 1);

                break;
            }

            counter += 1;
        }

        return convexHull;
    }
    public float CheckPositionBasedOnLine(Vector2 linePointA, Vector2 linePointB, Vector2 point)
    {
        float fx = linePointB.x - linePointA.x;
        float fy = linePointB.y - linePointA.y;
        return fx * (point.y - linePointA.y) - fy * (point.x - linePointA.x);
    }

}

