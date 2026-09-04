using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;

public class Hit : MonoBehaviour
{
    public LineRenderer lineRenderer;
    float archStrenth = 3f;
    float randomStrenth = 0.25f;

    int n = 20;
    Material mat;
    internal void Initialize(Vector3 buildingShoot, Vector3 unitAgent, float shotLifeTime)
    {
        vfxManager.Instance.Play(unitAgent, 1);
        GenerateLine(buildingShoot, unitAgent);
        mat = lineRenderer.material;
        mat.SetFloat("_life", shotLifeTime);
        StartCoroutine(DestroyAfterTime(shotLifeTime));
    }

    public IEnumerator DestroyAfterTime(float shotLifeTime)
    {
        float timer = 0;
        while (timer < shotLifeTime)
        {
            mat.SetFloat("_time", timer);
            yield return new WaitForEndOfFrame();

            Vector3[] points = new Vector3[n + 1];
            lineRenderer.GetPositions(points);

            for (int i = 0; i < n; i++)
            {
                points[i] = points[i] + RandomVector3(Time.deltaTime * 5) + Vector3.up * Time.deltaTime * 5;
            }
            lineRenderer.SetPositions(points);

            timer += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    void GenerateLine(Vector3 p1, Vector3 p2)
    {

        Vector3[] points = new Vector3[n + 1];
        for (int i = 0; i < n; i++)
        {
            points[i] = Vector3.Lerp(p1, p2, ((float)i) / (float)n);
            points[i] = new Vector3(points[i].x
            , points[i].y + curveFactor(n, i) * archStrenth
            , points[i].z) + RandomVector3(randomStrenth);
        }
        points[n] = p2;
        lineRenderer.positionCount = n + 1;
        lineRenderer.SetPositions(points);

    }

    float curveFactor(int n, int i)
    {
        return (float)(-i * i + n * i) / (((float)n / 2) * ((float)n / 2));
    }

    Vector3 RandomVector3(float strenth)
    {
        return new Vector3(UnityEngine.Random.Range(-strenth, strenth)
            , UnityEngine.Random.Range(-strenth, strenth)
            , UnityEngine.Random.Range(-strenth, strenth));
    }


}
