using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateBrushButtons : MonoBehaviour
{
    public List<BrushButton> brushButtons = new List<BrushButton>();
    public GameObject brushButtonPrefab;



    // Start is called before the first frame update
    void Start()
    {
        /*foreach (var brushButton in brushButtons)
        {
            Destroy(brushButton.gameObject);
        }*/

        CheckBrushFolder();
        string folderPath = "Assets\\StreamingAssets\\Blushes";

        string[] Files = Directory.GetFiles(folderPath); //Getting Text files


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
                BrushButton b = Instantiate(brushButtonPrefab, transform).GetComponent<BrushButton>();
                b.SetTexture(tt);
                b.master = this;
                if (i == 0)
                {
                    b.selecotr.color = new Color(1, 1, 1, 1);
                    EditorOptions.Instance.SetBrushImage(tt);
                }
                brushButtons.Add(b);
                i++;
            }

        }

        //DeselectAll();

    }
    private bool IsImage(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
    }
    public void DeselectAll()
    {
        foreach (BrushButton but in brushButtons)
        {
            but.Deselelect();
        }
    }

    void CheckBrushFolder()
    {
        if (!System.IO.Directory.Exists("Assets/StreamingAssets/Blushes"))
        {
            System.IO.Directory.CreateDirectory("Assets/StreamingAssets/Blushes");


        }
    }


}
