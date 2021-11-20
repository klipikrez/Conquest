using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class wata : MonoBehaviour
{
    public Terrain terrain;
    public MeshRenderer rend;
    public float amount = 0.5f;

    // Start is called before the first frame update
    void OnEnable()
    {
        //rend.material.SetTexture("Height", terrain.terrainData.heightmapTexture);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
