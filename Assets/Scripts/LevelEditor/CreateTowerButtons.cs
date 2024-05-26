using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using GLTFast;
using GLTFast.Addons;

public class CreateTowerButtons : MonoBehaviour
{
    public Dictionary<string, TowerButton> towerButtons = new Dictionary<string, TowerButton>();
    public GameObject towerButtonPrefab;
    public slider startingUnitsSlider;

    public static CreateTowerButtons Instance;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        /*foreach (var brushButton in brushButtons)
        {
            Destroy(brushButton.gameObject);
        }*/

        CheckTowerFolder();
        string folderPath = Application.dataPath + "/StreamingAssets/TowerPresets";

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
                    Byte[] pngBytes = System.IO.File.ReadAllBytes(dirName + "/icon.png");
                    Texture2D tt = new Texture2D(52, 52);
                    tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                                           // tt.alphaIsTransparency = true;
                    tt.name = Path.GetFileName(dirName + "/icon.png");

                    TowerButton b = Instantiate(towerButtonPrefab, transform).GetComponent<TowerButton>();
                    b.SetTexture(tt);
                    b.presetData = presetData;
                    b.SetName(Path.GetFileName(dirName));


                    b.master = this;



                    b.textName.text = Path.GetFileName(dirName);

                    towerButtons.Add(b.GetName(), b);

                    // Load the GLTF file
                    if (presetData.meshPath != null && presetData.meshPath.Length > 0 && presetData.meshPath[0] != "")
                    {
                        foreach (string path in presetData.meshPath)
                        {
                            AsyncLoadMesh(dirName, path, b, i);

                        }
                    }

                    i++;
                    break;
                }
            }
        }

        //DeselectAll();

    }

    private async void AsyncLoadMesh(string dirName, string path, TowerButton b, int i)
    {
        byte[] data = File.ReadAllBytes(dirName + "/" + path);
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(dirName + "/" + path)
            );

        if (success)
        {
            Mesh mesh = gltf.GetMeshes()[0];
            mesh.name = path;
            b.AddMesh(mesh, path);

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
        }
        else
        {
            Debug.Log("nevalja::" + dirName + "/" + path);
        }

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
        foreach (KeyValuePair<string, TowerButton> but in towerButtons)
        {
            but.Value.Deselelect();
        }
    }

    public void Select(string name)
    {


        LoadTowerPresets(towerButtons[name]);
        towerButtons[name].selecotr.color = new Color(1, 1, 1, 1);
        EditorManager.Instance.towerPresets = towerButtons[name];
    }

    public void LoadTowerPresets(TowerButton btn)
    {
        //twer placing stuff
        DeselectAll();

        EditorOptions.Instance.SelectedTowerPreset(btn);
    }

    void CheckTowerFolder()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/TowerPresets"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/TowerPresets");
        }
    }


}
