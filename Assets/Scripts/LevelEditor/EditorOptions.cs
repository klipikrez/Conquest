using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

public class LevelOptions
{
    public Vector3 playerPos;
}

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
    public GameObject[] towerOverrideInputs;
    public float brushHeight = 0.51f;
    public float folageDensity = 1;
    public int selectedTexture = 0;
    public float minScale = 1;
    public float maxScale = 1;
    public int selectedTree = 0;
    public float treeSpacing = 3f;
    public slider[] sliders;
    public static EditorOptions Instance;
    TowerButton selectedTowerButton;

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

        foreach (GameObject obj in towerOverrideInputs)
            obj.SetActive(false);
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
                    foreach (GameObject obj in towerOverrideInputs)
                        obj.SetActive(true);
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

    public void SelectedTowerPreset(TowerButton btn)
    {

        selectedTowerButton = btn;


        int i = 0;
        foreach (slider slider in sliders)
        {
            float val = float.NaN;
            switch (i++)
            {
                case 0:
                    val = btn.presetData.product;
                    break;
                case 1:
                    val = btn.presetData.maxUnits;
                    break;
                case 2:
                    val = btn.presetData.productProduction;
                    break;
                case 3:
                    val = btn.presetData.cost;
                    break;
                case 4:
                    val = btn.presetData.vulnerability;
                    break;
            };
            SetupSliders(slider, val);
        }
        foreach (KeyValuePair<int, GameObject> tower in EditorManager.Instance.editorSelection.selectedDictionary.selected)
        {
            tower.Value.GetComponent<EditorTower>().SetPreset(btn.presetData, btn.textName.text, btn.GetRandomMesh());
        }
    }

    public void UpdateOverrideValue(string name, float value)
    {
        foreach (KeyValuePair<int, GameObject> tower in EditorManager.Instance.editorSelection.selectedDictionary.selected)
        {
            tower.Value.GetComponent<EditorTower>().AddOverride(name, value);
        }
    }
    public void ClearOverride(string name)
    {
        foreach (KeyValuePair<int, GameObject> tower in EditorManager.Instance.editorSelection.selectedDictionary.selected)
        {
            tower.Value.GetComponent<EditorTower>().ClearOverride(name);
        }
    }
    public void ClearAllOverrides()
    {
        foreach (KeyValuePair<int, GameObject> tower in EditorManager.Instance.editorSelection.selectedDictionary.selected)
        {
            tower.Value.GetComponent<EditorTower>().ClearOverrides();
        }
    }

    public void SelectedEditorTowers()
    {
        if (EditorManager.Instance.editorSelection.selectedDictionary.selected.Count == 0)
        {
            SelectedTowerPreset(selectedTowerButton);
            return;
        }
        float[] ovverrides = new float[5];

        int i = 0;

        foreach (KeyValuePair<int, GameObject> obj in EditorManager.Instance.editorSelection.selectedDictionary.selected)
        {
            TowerPresetData preset = obj.Value.gameObject.GetComponent<EditorTower>().preset;
            Dictionary<string, object> towerOverrides = obj.Value.gameObject.GetComponent<EditorTower>().towerOverrides;
            if (i++ == 0)
            {
                ovverrides[0] = towerOverrides.ContainsKey("Starting units") ? (float)towerOverrides["Starting units"] : preset.product;
                ovverrides[1] = towerOverrides.ContainsKey("Max units") ? (float)towerOverrides["Max units"] : preset.maxUnits;
                ovverrides[2] = towerOverrides.ContainsKey("Unit production") ? (float)towerOverrides["Unit production"] : preset.productProduction;
                ovverrides[3] = towerOverrides.ContainsKey("Cost as an upgrade") ? (float)towerOverrides["Cost as an upgrade"] : preset.cost;
                ovverrides[4] = towerOverrides.ContainsKey("Vulnerability") ? (float)towerOverrides["Vulnerability"] : preset.vulnerability;

            }
            else
            {
                if (ovverrides[0] != (towerOverrides.ContainsKey("Starting units") ? (float)towerOverrides["Starting units"] : preset.product)) ovverrides[0] = float.NaN;
                if (ovverrides[1] != (towerOverrides.ContainsKey("Max units") ? (float)towerOverrides["Max units"] : preset.maxUnits)) ovverrides[1] = float.NaN;
                if (ovverrides[2] != (towerOverrides.ContainsKey("Unit production") ? (float)towerOverrides["Unit production"] : preset.productProduction)) ovverrides[2] = float.NaN;
                if (ovverrides[3] != (towerOverrides.ContainsKey("Cost as an upgrade") ? (float)towerOverrides["Cost as an upgrade"] : preset.cost)) ovverrides[3] = float.NaN;
                if (ovverrides[4] != (towerOverrides.ContainsKey("Vulnerability") ? (float)towerOverrides["Vulnerability"] : preset.vulnerability)) ovverrides[4] = float.NaN;
            }

        }
        Debug.Log(ovverrides[1]);
        i = 0;
        foreach (slider slider in sliders)
        {
            float val = ovverrides[i++];
            SetupSliders(slider, val);
        }
    }

    void SetupSliders(slider slider, float val)
    {
        slider.specialValue[0] = !float.IsNaN(val) ? val : -1;
        slider.specialValue[1] = !float.IsNaN(val) ? val : -1;
        slider.sliderElement.value = !float.IsNaN(val) ? val : slider.sliderElement.maxValue;
        if (float.IsNaN(val))
        {
            slider.SetText("(X_X)");
        }
    }

}
