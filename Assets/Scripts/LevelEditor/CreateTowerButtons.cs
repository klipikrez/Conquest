using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CreateTowerButtons : MonoBehaviour
{
    public List<TowerButton> towerButtons = new List<TowerButton>();
    public GameObject towerButtonPrefab;
    public slider startingUnitsSlider;

    public slider[] sliders;


    // Start is called before the first frame update
    void Start()
    {
        /*foreach (var brushButton in brushButtons)
        {
            Destroy(brushButton.gameObject);
        }*/

        CheckTowerFolder();
        string folderPath = "Assets\\StreamingAssets\\TowerPresets";

        string[] dir = Directory.GetDirectories(folderPath);
        int i = 0;
        foreach (string dirName in dir)
        {
            string[] Files = Directory.GetFiles(dirName); //Getting Text files


            foreach (string file in Files)
            {
                if (IsJson(file))
                {
                    Debug.Log(file);

                    TowerPresetData presetData = JsonUtility.FromJson<TowerPresetData>(File.ReadAllText(file));//update setings json

                    //load icon image
                    Byte[] pngBytes = System.IO.File.ReadAllBytes(dirName + "\\" + presetData.imagePath);
                    Texture2D tt = new Texture2D(52, 52);
                    tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                    tt.alphaIsTransparency = true;
                    tt.name = Path.GetFileName(dirName + "\\" + presetData.imagePath);

                    TowerButton b = Instantiate(towerButtonPrefab, transform).GetComponent<TowerButton>();
                    b.SetTexture(tt);
                    b.presetData = presetData;
                    b.master = this;



                    b.textName.text = Path.GetFileName(dirName);

                    towerButtons.Add(b);
                    if (i == 0)
                    {
                        b.selecotr.color = new Color(1, 1, 1, 1);
                        SelectedTowerPreset(b);
                    }
                    else
                    {
                        b.selecotr.color = new Color(1, 1, 1, 0);
                    }


                    i++;
                    break;
                }
            }
        }

        //DeselectAll();

    }
    private bool IsJson(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".rez";
    }
    public void DeselectAll()
    {
        foreach (TowerButton but in towerButtons)
        {
            but.Deselelect();
        }
    }

    void CheckTowerFolder()
    {
        if (!System.IO.Directory.Exists("Assets/StreamingAssets/TowerPresets"))
        {
            System.IO.Directory.CreateDirectory("Assets/StreamingAssets/TowerPresets");


        }
    }

    public void SelectedTowerPreset(TowerButton btn)
    {

        /*startingUnitsSlider.specialValue[0] = btn.presetData.product;
        startingUnitsSlider.specialValue[1] = btn.presetData.product;
        startingUnitsSlider.sliderElement.value = (btn.presetData.product);*/




        int i = 0;
        foreach (slider slider in sliders)
        {
            float val = -99520;
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
    }
    void SetupSliders(slider slider, float val)
    {
        slider.specialValue[0] = val;
        slider.specialValue[1] = val;
        slider.sliderElement.value = val;
    }
}
