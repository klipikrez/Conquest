using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerrainEditorLayer : MonoBehaviour
{
    public TerrainLayer masterLayer;
    TerrainLayers master;
    public Texture2D tex;
    public Image textureImage;
    public GameObject Selector;
    public TextMeshProUGUI layerName;
    public slider scaleSlider;
    public TMP_InputField imput;
    public GameObject[] normalMode;
    public GameObject[] editMode;
    public void Inicialize(TerrainLayers master)
    {
        this.master = master;
    }

    public void ShiftUp()
    {
        master.SwitchLayersUp(transform.GetSiblingIndex());
    }

    public void ShiftDown()
    {
        master.SwitchLayersDown(transform.GetSiblingIndex());
    }

    public void DeleteSelf()
    {
        if (Selector.activeSelf)
        {
            EditorOptions.Instance.ClearDrawingTexture();
        }
        master.DeleteLayer(transform.GetSiblingIndex());
    }

    public void OpenTexturesMenu()
    {
        master.OpenTexturesMenu(this);
    }

    public void Select()
    {
        master.CloseTexturesMenu();
        master.Select(this);
        EditorOptions.Instance.SelectDrawingTexture(tex);
    }


    public void SelectNoUpdate()
    {
        Selector.SetActive(true);
    }
    public void Deselect()
    {
        Selector.SetActive(false);
    }

    public void SetLayerName(string layerName)
    {
        Debug.Log(layerName);
        this.layerName.text = layerName;
        imput.text = layerName;
        masterLayer.name = layerName;
    }

    public void EditStart()
    {
        master.CloseTexturesMenu();
        foreach (GameObject btn in editMode)
        {
            btn.SetActive(true);
        }
        foreach (GameObject btn in normalMode)
        {
            btn.SetActive(false);
        }
        imput.text = layerName.text;
    }
    public void EditEnd()
    {
        master.CloseTexturesMenu();
        foreach (GameObject btn in editMode)
        {
            btn.SetActive(false);
        }
        foreach (GameObject btn in normalMode)
        {
            btn.SetActive(true);
        }
        layerName.text = imput.text == "" ? "Unnamed layer" : imput.text;
    }

    public void SetTexture(Texture2D tex)
    {
        this.tex = tex;
        textureImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        TerrainData terrain = EditorManager.Instance.terrain.terrainData;
        masterLayer.diffuseTexture = tex;
    }

    public void ChangeLayerScale(float value)
    {
        masterLayer.tileSize = new Vector2(value, value);
    }

    public void SetValues(Texture2D tex, float scale)
    {
        SetTexture(tex);
        scaleSlider.UpdateValue(scale);
    }
}
