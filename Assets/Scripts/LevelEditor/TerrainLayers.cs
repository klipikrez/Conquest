using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLayers : MonoBehaviour
{
    public List<TerrainEditorLayer> layers = new List<TerrainEditorLayer>();
    public Shader[] terrainShaders;
    public Shader[] terrainShadersUnlit;
    public Material terrainMaterial;
    public Material terrainMaterialUnlit;
    public Transform parentSpawn;
    public GameObject prefab;

    public CreateTextureButtons textureButtons;
    public static TerrainLayers Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DeselectAll();
    }
    public void Select(TerrainEditorLayer layer)
    {
        DeselectAll();
        layer.SelectNoUpdate();
    }

    void DeselectAll()
    {
        foreach (TerrainEditorLayer deselect in layers)
        {
            deselect.Deselect();
        }
    }
    public void AddNewLayerButton()
    {
        AddNewLayer();
    }
    public void AddNewLayer(Texture2D tex = null, float scale = 2, string name = "NewLayer")
    {
        TerrainData terrain = EditorManager.Instance.terrain.terrainData;
        List<TerrainLayer> terrainLayersList = new List<TerrainLayer>();

        //set new layer in terrain
        terrainLayersList.AddRange(terrain.terrainLayers);
        TerrainLayer newLayer = new TerrainLayer();
        if (tex != null) { newLayer.diffuseTexture = tex; }
        newLayer.tileSize = new Vector2(scale, scale);
        newLayer.name = name;
        terrainLayersList.Add(newLayer);

        terrain.terrainLayers = terrainLayersList.ToArray();

        //add ui
        TerrainEditorLayer layer = Instantiate(prefab, parentSpawn).GetComponent<TerrainEditorLayer>();
        layers.Add(layer);
        layer.Inicialize(this);
        layer.masterLayer = newLayer;
        layer.SetLayerName(name);
        if (tex != null) layer.SetValues(tex, scale);

        CheckTerrainShader();

    }
    public void SwitchLayersUp(int index)
    {
        if (index == 0) return;
        layers[index].transform.SetSiblingIndex(layers[index].transform.GetSiblingIndex() - 1);
        TerrainEditorLayer temp = layers[index];
        layers[index] = layers[index - 1];
        layers[index - 1] = temp;
    }
    public void SwitchLayersDown(int index)
    {
        if (index == layers.Count - 1) return;
        layers[index].transform.SetSiblingIndex(layers[index].transform.GetSiblingIndex() + 1);
        TerrainEditorLayer temp = layers[index];
        layers[index] = layers[index + 1];
        layers[index + 1] = temp;


    }

    public void OpenTexturesMenu(TerrainEditorLayer layer)
    {
        textureButtons.Open(layer);
    }

    public void CloseTexturesMenu()
    {
        textureButtons.Close();
        CheckTerrainShader();

    }
    public void DeleteLayer(int index)
    {

        TerrainData terrain = EditorManager.Instance.terrain.terrainData;
        List<TerrainLayer> terrainLayersList = new List<TerrainLayer>();

        //remove layer in terrain
        terrainLayersList.AddRange(terrain.terrainLayers);
        TerrainLayer removeLayer = layers[index].masterLayer;
        terrainLayersList.Remove(removeLayer);

        terrain.terrainLayers = terrainLayersList.ToArray();


        Destroy(layers[index].gameObject);
        layers.RemoveAt(index);
        CheckTerrainShader();
    }

    void CheckTerrainShader()
    {
        if (layers.Count <= 4)
        {
            terrainMaterialUnlit.shader = terrainShadersUnlit[0];
            terrainMaterial.shader = terrainShaders[0];
        }
        else
        {
            terrainMaterial.shader = terrainShaders[1];
            terrainMaterialUnlit.shader = terrainShadersUnlit[1];
        }


    }
}
