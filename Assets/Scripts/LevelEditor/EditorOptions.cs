using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LevelOptions
{
    public Vector3 playerPos;
    public Vector2[] boundsPoints;
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
    public Texture2D treesBrushImage;
    public TMP_InputField terrainNameInput;
    public GameObject saveTerrainButton;
    public GameObject cancelSetPlayerPosButtons;
    public GameObject modeTabs;
    public GameObject setPlayerSpawnButton;
    public GameObject setBoundsButton;
    public GameObject[] towerOverrideInputs;
    public GameObject terrainLayers;
    public Material brushGraphic;
    public DecalProjector decalProjectorBrush;

    public float brushHeight = 0.51f;
    public float folageDensity = 1;
    public int selectedTexture = -1;
    public float minScale = 1;
    public float maxScale = 1;
    public int selectedTree = 0;
    public float treeSpacing = 3f;
    public slider[] sliders;
    public static EditorOptions Instance;
    TowerButton selectedTowerButton;
    public int team = 0;
    public CreateTeamButtons teamMaster;
    private void Awake()
    {
        Instance = this;
    }

    public void SetBrushImage(Texture2D texture)
    {
        brushGraphic.SetTexture("_Texture", texture);
        BrushImage = texture;

    }

    public void SetBrushImageNoUpdate(Texture2D texture)
    {
        brushGraphic.SetTexture("_Texture", texture);
    }
    public void UpdateBrushSize(float val)
    {
        decalProjectorBrush.size = new Vector3(val, val, decalProjectorBrush.size.z);
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
        terrainLayers.SetActive(false);
        setBoundsButton.SetActive(false);
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
                    terrainLayers.SetActive(true);
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
                    setBoundsButton.SetActive(true);
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

    public void ClearDrawingTexture()
    {
        selectedTexture = -1;
    }
    public void SelectDrawingTexture(Texture2D texture)
    {
        if (texture == null) return;
        TerrainLayer[] terrainLayers = EditorManager.Instance.terrain.terrainData.terrainLayers;

        for (int i = 0; i < terrainLayers.Length; i++)
        {
            if (terrainLayers[i].diffuseTexture == texture)
            {
                selectedTexture = i;
                return;
                //                Debug.Log("" + selectedTexture);
            }
        }
        selectedTexture = -1;
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
            SetupSliders(slider, val, val);// the same valuse 
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
        float[] def = new float[5];
        float[] ovverrides = new float[5];
        int team = -52;
        string presetName = "";

        int i = 0;
        //thiis is to setup sliders based on what towers are selected
        foreach (KeyValuePair<int, GameObject> obj in EditorManager.Instance.editorSelection.selectedDictionary.selected)
        {
            EditorTower tower = obj.Value.gameObject.GetComponent<EditorTower>();
            TowerPresetData preset = tower.preset;
            Dictionary<string, float> towerOverrides = tower.towerOverrides;

            def[0] = preset.product;
            def[1] = preset.maxUnits;
            def[2] = preset.productProduction;
            def[3] = preset.cost;
            def[4] = preset.vulnerability;

            if (i++ == 0)
            {
                ovverrides[0] = towerOverrides.ContainsKey("Starting units") ? (float)towerOverrides["Starting units"] : preset.product;
                ovverrides[1] = towerOverrides.ContainsKey("Max units") ? (float)towerOverrides["Max units"] : preset.maxUnits;
                ovverrides[2] = towerOverrides.ContainsKey("Unit production") ? (float)towerOverrides["Unit production"] : preset.productProduction;
                ovverrides[3] = towerOverrides.ContainsKey("Cost as an upgrade") ? (float)towerOverrides["Cost as an upgrade"] : preset.cost;
                ovverrides[4] = towerOverrides.ContainsKey("Vulnerability") ? (float)towerOverrides["Vulnerability"] : preset.vulnerability;
                presetName = obj.Value.gameObject.GetComponent<EditorTower>().presetName;
                team = tower.team;
            }
            else
            {
                if (ovverrides[0] != (towerOverrides.ContainsKey("Starting units") ? (float)towerOverrides["Starting units"] : preset.product)) ovverrides[0] = float.NaN;
                if (ovverrides[1] != (towerOverrides.ContainsKey("Max units") ? (float)towerOverrides["Max units"] : preset.maxUnits)) ovverrides[1] = float.NaN;
                if (ovverrides[2] != (towerOverrides.ContainsKey("Unit production") ? (float)towerOverrides["Unit production"] : preset.productProduction)) ovverrides[2] = float.NaN;
                if (ovverrides[3] != (towerOverrides.ContainsKey("Cost as an upgrade") ? (float)towerOverrides["Cost as an upgrade"] : preset.cost)) ovverrides[3] = float.NaN;
                if (ovverrides[4] != (towerOverrides.ContainsKey("Vulnerability") ? (float)towerOverrides["Vulnerability"] : preset.vulnerability)) ovverrides[4] = float.NaN;
                if (presetName != obj.Value.gameObject.GetComponent<EditorTower>().presetName) presetName = null;
                if (team != tower.team) team = -52;
            }

        }

        if (presetName != null) CreateTowerButtons.Instance.Select(presetName);

        i = 0;
        foreach (slider slider in sliders)
        {
            float val = ovverrides[i];
            SetupSliders(slider, val, def[i++]);
        }
        if (team != -52)
        {
            teamMaster.SelectNoUpdate(team);
        }
        else
        {
            teamMaster.DeselectAll();
        }


    }

    void SetupSliders(slider slider, float val, float def)
    {
        slider.specialValue[0] = !float.IsNaN(def) ? def : -1;
        slider.specialValue[1] = !float.IsNaN(def) ? def : -1;
        slider.sliderElement.value = !float.IsNaN(val) ? val : slider.sliderElement.maxValue;

        if (float.IsNaN(val))
        {
            slider.SetText("(X_X)");
        }
    }
    public void SetTeam(int i)
    {
        team = i;
    }



}
