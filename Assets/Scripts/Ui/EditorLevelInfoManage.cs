using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TMPro;
using Tymski;
using UnityEngine;

public class EditorLevelInfoManage : MonoBehaviour
{
    public SceneReference editor;
    public SceneReference level;
    public TextMeshProUGUI nameElement;
    public UnityEngine.UI.Image image;
    public Sprite defaultSprite;
    public TextMeshProUGUI flavourTextElement;
    public string levelName;
    protected int levelNumber = -52;

    public virtual void SetSelectedLevel(string levelName, int levelNumber = -52)
    {

        this.levelNumber = levelNumber;
        //        Debug.Log(" - - - -- " + levelName);
        nameElement.text = levelName;
        this.levelName = levelName;

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "cover.png");

        if (!File.Exists(filePath))
        {
            image.sprite = defaultSprite;
            Debug.Log("Failed to load coverImage for: " + filePath);
            return;
        }


        if (IsImage(filePath))
        {
            Byte[] pngBytes = System.IO.File.ReadAllBytes(filePath);
            Texture2D tt = new Texture2D(52, 52);
            tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                                   //tt.alphaIsTransparency = true;
            tt.name = Path.GetFileName(filePath);

            image.sprite = Sprite.Create(tt, new Rect(0, 0, tt.width, tt.height), new Vector2(0.5f, 0.5f));
            //Destroy(tt, 5f);
        }

    }

    public virtual void Play()
    {
        if (levelName == "" || levelName == "Editor") { SoundManager.Instance.PlayDeniedSound(); return; }
        Debug.Log("Play: " + levelName);
        ScenesManager.Instance.LoadLevel(level, levelName);

    }

    public void Edit()
    {
        if (levelName == "") return; Debug.Log("edit: " + levelName);
        ScenesManager.Instance.LoadEditor(editor, levelName);
    }

    public void EditNew()
    {
        Debug.Log("CreateNewLwvwl");
        ScenesManager.Instance.LoadEditor(editor, "Editor");
    }

    public static bool IsImage(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
    }

    public virtual void Snap()
    {
        //throw new NotImplementedException();
    }
}
