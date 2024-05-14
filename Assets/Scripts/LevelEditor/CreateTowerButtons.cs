using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Siccity.GLTFUtility;

public class CreateTowerButtons : MonoBehaviour
{
    public List<TowerButton> towerButtons = new List<TowerButton>();
    public GameObject towerButtonPrefab;
    public slider startingUnitsSlider;




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

                    TowerPresetData presetData = JsonUtility.FromJson<TowerPresetData>(File.ReadAllText(file));//get tower

                    //load icon image
                    Byte[] pngBytes = System.IO.File.ReadAllBytes(dirName + "\\icon.png");
                    Texture2D tt = new Texture2D(52, 52);
                    tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                    tt.alphaIsTransparency = true;
                    tt.name = Path.GetFileName(dirName + "\\icon.png");

                    TowerButton b = Instantiate(towerButtonPrefab, transform).GetComponent<TowerButton>();
                    b.SetTexture(tt);
                    b.presetData = presetData;
                    b.SetName(Path.GetFileName(dirName));
                    Debug.Log(b.GetName());

                    // Load the GLTF file
                    if (presetData.meshPath != null && presetData.meshPath.Length > 0 && presetData.meshPath[0] != "")
                    {
                        foreach (string path in presetData.meshPath)
                        {
                            GameObject result = Importer.LoadFromFile(dirName + "\\" + path);
                            Mesh mesh = result.GetComponent<MeshFilter>().sharedMesh;
                            mesh.name = path;
                            // Debug.Log(mesh.name + " -- " + path);
                            b.AddMesh(mesh, path);
                            //ImportGLTFAsync(dirName + "\\" + path, b);
                        }
                    }
                    b.master = this;



                    b.textName.text = Path.GetFileName(dirName);

                    towerButtons.Add(b);
                    if (i == 0)
                    {
                        b.selecotr.color = new Color(1, 1, 1, 1);
                        EditorManager.Instance.towerPresets = b;
                        EditorOptions.Instance.SelectedTowerPreset(b);

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

    /* void ImportGLTFAsync(string filepath, TowerButton targetObject)
     {
         Importer.ImportGLTFAsync(filepath, new ImportSettings(), (result, animations) => OnFinishAsync(result, animations, targetObject));
     }
 */
    /*void OnFinishAsync(GameObject result, AnimationClip[] animations, TowerButton targetObject)
    {
        targetObject.AddMesh(result.GetComponent<MeshFilter>().sharedMesh);

        Debug.Log("Finished importing " + result.name + " and applied it to " + targetObject.name);
    }*/
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


}
