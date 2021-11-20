using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class changeSettings : MonoBehaviour
{
    public RenderPipelineAsset RenderingAsset;

    void Start()
    {

        QualitySettings.renderPipeline = RenderingAsset;
    }
}
