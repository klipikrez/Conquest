using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{


    public Image selecotr;
    public RawImage TowerPresetImage;
    public CreateTowerButtons master;
    public TowerPresetData presetData;
    public TextMeshProUGUI textName;
    public void SetTexture(Texture2D texture2D)
    {

        TowerPresetImage.texture = texture2D;
    }
    // Start is called before the first frame update
    void Start()
    {

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
    }

    public void LoadTowerPresets()
    {
        //twer placing stuff
        master.DeselectAll();

        master.SelectedTowerPreset(this);
    }
}
