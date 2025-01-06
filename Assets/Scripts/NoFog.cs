using UnityEngine;
using UnityEngine.Rendering;

public class NoFog : MonoBehaviour
{
    public Camera cameraWithoutFog;

    private void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }
    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == cameraWithoutFog)
            RenderSettings.fog = false;
        else
            RenderSettings.fog = true;
    }

}