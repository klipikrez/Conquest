using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Tymski;
using UnityEngine;

public class LevelInfoManage : MonoBehaviour
{
    public SceneReference editor;
    public SceneReference level;
    public TextMeshProUGUI nameElement;
    public Image image;
    public TextMeshProUGUI flavourTextElement;
    public string levelName;

    public void SetSelectedLevel(string levelName)
    {
        nameElement.text = levelName;
        this.levelName = levelName;

    }

    public void Play()
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
}
