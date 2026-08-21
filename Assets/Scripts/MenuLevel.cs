using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevel : MonoBehaviour
{

    public string levelName = "brki";
    public TextMeshProUGUI text;
    public Image bgImage;
    EditorLevelInfoManage manager;

    public void Initialize(string levelName, EditorLevelInfoManage manager, bool cuttoffLevelName = false)
    {

        this.levelName = levelName;
        text.text = cuttoffLevelName ? levelName.Split(' ')[0] : levelName;
        this.manager = manager;

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "cover.png");

        if (!File.Exists(filePath))
        {
            bgImage.sprite = manager.defaultSprite;
            Debug.Log("Failed to load coverImage for: " + filePath);
            return;
        }


        if (EditorLevelInfoManage.IsImage(filePath))
        {
            Byte[] pngBytes = System.IO.File.ReadAllBytes(filePath);
            Texture2D tt = new Texture2D(52, 52);
            tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                                   //tt.alphaIsTransparency = true;
            tt.name = Path.GetFileName(filePath);

            bgImage.sprite = Sprite.Create(tt, new Rect(0, 0, tt.width, tt.height), new Vector2(0.5f, 0.5f));
            //Destroy(tt, 5f);
        }




    }


    public void Selected()
    {
        manager.SetSelectedLevel(levelName);
        manager.Snap();
    }

}
