using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateTeamButtons : MonoBehaviour
{
    public List<TextureButton> teamButtons = new List<TextureButton>();



    // Start is called before the first frame update
    void Start()
    {


    }
    private bool IsImage(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
    }
    public void DeselectAll()
    {
        foreach (TextureButton but in teamButtons)
        {
            but.Deselelect();
        }
    }

    void CheckTextureFolder()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/Textures"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Textures");


        }
    }


}
