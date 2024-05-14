using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class meshAndName
{
    public Mesh mesh;
    public string name;

}
public class TowerButton : MonoBehaviour
{


    public Image selecotr;
    string BuildingName;
    public RawImage TowerPresetImage;
    public CreateTowerButtons master;
    public TowerPresetData presetData;
    public TextMeshProUGUI textName;
    public List<meshAndName> meshes = new List<meshAndName>();
    public void SetTexture(Texture2D texture2D)
    {

        TowerPresetImage.texture = texture2D;
    }
    // Start is called before the first frame update
    public void AddMesh(Mesh mesh, string meshPath)
    {
        this.meshes.Add(new meshAndName { mesh = mesh, name = meshPath });
    }


    public meshAndName GetRandomMesh()
    {
        return meshes.Count > 0 ? meshes[UnityEngine.Random.Range(0, meshes.Count)] : new meshAndName();
    }

    public void Deselelect()
    {
        //Background.color = new Color(0, 0, 0, 0);
        selecotr.color = new Color(1, 1, 1, 0);
    }

    public void Select()
    {
        //Background.color = new Color(0, 0, 0, 1);

        LoadTowerPresets();
        selecotr.color = new Color(1, 1, 1, 1);
        EditorManager.Instance.towerPresets = this;
    }

    public void LoadTowerPresets()
    {
        //twer placing stuff
        master.DeselectAll();

        EditorOptions.Instance.SelectedTowerPreset(this);
    }

    public void SetName(string name)
    {
        BuildingName = name;
    }

    public string GetName()
    {
        return BuildingName;
    }
}
