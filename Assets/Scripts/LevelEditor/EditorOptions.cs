using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

public class EditorOptions : MonoBehaviour
{
    public Slider brushSizeSlider;
    public Slider brushStrenthSlider;
    public Slider brushHeightSlider;
    public GameObject BrushTabs;
    public GameObject BrushView;
    public GameObject TextureView;
    public Slider FolageSlider;
    public int brushSize = 5;
    public float brushStrenth = 15;
    public GameObject editorMenu;
    public static EditorOptions Instance;
    public Texture2D BrushImage;
    public float brushHeight = 0.51f;
    public float folageDensity = 1;
    public int selectedTexture = 0;
    public float minScale = 1;
    public float maxScale = 1;
    public int selectedTree = 0;
    public float treeSpacing = 3f;

    private void Awake()
    {
        Instance = this;
    }

    public void SetBrushImage(Texture2D texture)
    {
        BrushImage = texture;

    }
    public void UpdateBrushSize(float val)
    {
        brushSizeSlider.value = val;
        brushSize = (int)val;
    }

    public void UpdateBrushHeight(float val)
    {
        brushHeightSlider.value = val;
        brushHeight = (val + 150) / 300;
    }

    public void UpdateFolageDensity(float val)
    {
        Debug.Log("Fola: " + val / 16);
        FolageSlider.value = val;
        folageDensity = val / 16;
    }

    public void UpdateBrushStrenth(float val)
    {
        brushStrenthSlider.value = val;
        brushStrenth = val;
    }

    private void Start()
    {
        SetMenuActive(-1);
        UpdateBrushSize(brushSize);
        UpdateBrushStrenth(brushStrenth);
        UpdateFolageDensity(folageDensity);
    }
    public void SetMenuActive(int val)
    {
        editorMenu.SetActive(true);
        switch (val)
        {
            case 0:
                {
                    brushHeightSlider.gameObject.SetActive(false);
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushTabs.SetActive(true);
                    BrushView.SetActive(true);
                    TextureView.SetActive(false);
                    FolageSlider.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    brushHeightSlider.gameObject.SetActive(false);
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushTabs.SetActive(false);
                    BrushView.SetActive(true);
                    TextureView.SetActive(true);
                    FolageSlider.gameObject.SetActive(false);
                    break;
                }
            case 2:
                {
                    brushHeightSlider.gameObject.SetActive(false);
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushTabs.SetActive(false);
                    BrushView.SetActive(false);
                    TextureView.SetActive(false);
                    FolageSlider.gameObject.SetActive(false);
                    break;
                }
            case 3:
                {
                    brushHeightSlider.gameObject.SetActive(false);
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushTabs.SetActive(false);
                    BrushView.SetActive(true);
                    TextureView.SetActive(false);
                    FolageSlider.gameObject.SetActive(true);
                    break;
                }
            default:
                {
                    brushHeightSlider.gameObject.SetActive(false);
                    brushSizeSlider.gameObject.SetActive(false);
                    brushStrenthSlider.gameObject.SetActive(false);
                    BrushTabs.SetActive(false);
                    editorMenu.SetActive(false);
                    FolageSlider.gameObject.SetActive(false);
                    break;
                }
        }

        /*  foreach (var item in editorMenus)
          {
              item.SetActive(false);
          }
          if (val != -1) editorMenus[val].SetActive(true);*/
    }
    public void SelectDrawingTexture(Texture2D texture)
    {
        TerrainLayer[] terrainLayers = EditorManager.Instance.terrain.terrainData.terrainLayers;

        for (int i = 0; i < terrainLayers.Length; i++)
        {
            if (terrainLayers[i].diffuseTexture == texture)
            {
                selectedTexture = i;
                //                Debug.Log("" + selectedTexture);
            }
        }
    }

}
