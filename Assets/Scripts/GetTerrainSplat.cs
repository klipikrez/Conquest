using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTerrainSplat : MonoBehaviour
{
    public Material mat;
    Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetTexture("_Control", terrain.terrainData.GetAlphamapTexture(0));
    }
}
