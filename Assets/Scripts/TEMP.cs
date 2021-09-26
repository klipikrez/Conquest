using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TEMP : MonoBehaviour
{
    public RenderPipelineAsset exampleAssetA;
    public RenderPipelineAsset exampleAssetB;
    public RenderPipelineAsset exampleAssetC;
    bool toogle1 = false;
    bool toogle2 = false;


    public void SetAllOutline()
    {

        toogle1 = !toogle1;
        Aply();

    }
    public void SetReversedOutline()
    {

        toogle2 = !toogle2;
        Aply();
    }

    void Aply()
    {
        if (!toogle1 && !toogle2)
        {
            GraphicsSettings.renderPipelineAsset = exampleAssetA;
        }
        else if (toogle1 && !toogle2)
        {
            GraphicsSettings.renderPipelineAsset = exampleAssetB;
        }
        else
        {
            GraphicsSettings.renderPipelineAsset = exampleAssetC;
        }
        Debug.Log("Default render pipeline asset is: " + GraphicsSettings.renderPipelineAsset.name);

    }

}
