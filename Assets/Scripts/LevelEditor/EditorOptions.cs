using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Texture2D BrushImage;
    public TMP_InputField terrainNameInput;
    public GameObject saveTerrainButton;
    public GameObject cancelSetPlayerPosButtons;
    public GameObject modeTabs;
    public GameObject setPlayerSpawnButton;
    public float brushHeight = 0.51f;
    public float folageDensity = 1;
    public int selectedTexture = 0;
    public float minScale = 1;
    public float maxScale = 1;
    public int selectedTree = 0;
    public float treeSpacing = 3f;
    public static EditorOptions Instance;

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
        modeTabs.SetActive(true);

        cancelSetPlayerPosButtons.SetActive(false);
        brushHeightSlider.gameObject.SetActive(false);
        brushSizeSlider.gameObject.SetActive(false);
        brushStrenthSlider.gameObject.SetActive(false);
        BrushTabs.SetActive(false);
        FolageSlider.gameObject.SetActive(false);
        terrainNameInput.gameObject.SetActive(false);
        saveTerrainButton.SetActive(false);
        BrushView.SetActive(false);
        TextureView.SetActive(false);
        setPlayerSpawnButton.SetActive(false);
        switch (val)
        {
            case 0:
                {
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushTabs.SetActive(true);
                    BrushView.SetActive(true);
                    break;
                }
            case 1:
                {
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushView.SetActive(true);
                    TextureView.SetActive(true);
                    break;
                }
            case 2:
                {
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    break;
                }
            case 3:
                {
                    brushSizeSlider.gameObject.SetActive(true);
                    brushStrenthSlider.gameObject.SetActive(true);
                    BrushView.SetActive(true);
                    FolageSlider.gameObject.SetActive(true);
                    break;
                }
            case 4:
                {
                    break;
                }
            case 5:
                {
                    terrainNameInput.gameObject.SetActive(true);
                    saveTerrainButton.SetActive(true);
                    setPlayerSpawnButton.SetActive(true);
                    break;
                }
            case 6:
                {
                    cancelSetPlayerPosButtons.SetActive(true);
                    modeTabs.SetActive(false);
                    break;
                }
            default:
                {
                    modeTabs.SetActive(true);
                    editorMenu.SetActive(false);

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
