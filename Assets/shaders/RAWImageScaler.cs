using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class RAWImageScaler : MonoBehaviour
{
    public RawImage img;

    public int pixelate = 10;

    // Start is called before the first frame update
    void Start()
    {




    }

    // Update is called once per frame
    void Update()
    {

        RenderTexture ttt = new RenderTexture((int)(((float)Screen.width / (float)Screen.height) * pixelate), pixelate, Camera.main.targetTexture.depth);
        Camera.main.targetTexture.Release();

        ttt.filterMode = FilterMode.Point;
        Camera.main.targetTexture = ttt;
        img.texture = Camera.main.targetTexture;
        //Camera.SetTargetBuffers(Camera.main.targetTexture,Camera.main.depth)
        //     UpdateAspect();
    }

    void UpdateAspect()
    {
        ScalableBufferManager.ResizeBuffers(0.4f, 0.01f/*(int)(Screen.width / Screen.height * pixelate), pixelate*/);
    }

}
