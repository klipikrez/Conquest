using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateTextureButtons : MonoBehaviour
{
    public List<TextureButton> textureButtons = new List<TextureButton>();
    public GameObject brushButtonPrefab;



    // Start is called before the first frame update
    void Start()
    {
        /*foreach (var brushButton in brushButtons)
        {
            Destroy(brushButton.gameObject);
        }*/

        CheckTextureFolder();
        string folderPath = "Assets\\StreamingAssets\\Textures";

        string[] Files = Directory.GetFiles(folderPath); //Getting Text files
        List<TerrainLayer> layers = new List<TerrainLayer>();

        int i = 0;
        foreach (string file in Files)
        {
            if (IsImage(file))
            {
                Byte[] pngBytes = System.IO.File.ReadAllBytes(file);
                Texture2D tt = new Texture2D(52, 52);
                tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                tt.alphaIsTransparency = true;
                tt.name = Path.GetFileName(file);
                TextureButton b = Instantiate(brushButtonPrefab, transform).GetComponent<TextureButton>();
                b.SetTexture(tt);
                b.master = this;
                TerrainLayer layer = new TerrainLayer();
                layer.diffuseTexture = tt;
                layers.Add(layer);
                if (i == 0)
                {
                    b.selecotr.color = new Color(1, 1, 1, 1);
                    EditorOptions.Instance.SetBrushImage(tt);
                }
                textureButtons.Add(b);
                i++;
            }

        }
        EditorManager.Instance.SetTerrainTextures(layers.ToArray());
        //DeselectAll();

    }
    private bool IsImage(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
    }
    public void DeselectAll()
    {
        foreach (TextureButton but in textureButtons)
        {
            but.Deselelect();
        }
    }

    void CheckTextureFolder()
    {
        if (!System.IO.Directory.Exists("Assets/StreamingAssets/Textures"))
        {
            System.IO.Directory.CreateDirectory("Assets/StreamingAssets/Textures");


        }
    }


}
