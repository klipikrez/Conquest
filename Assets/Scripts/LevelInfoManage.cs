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
        if (levelName == "") return;
        ScenesManager.Instance.Load(level);

    }

    public void Edit()
    {
        if (levelName == "") return;
        ScenesManager.Instance.LoadEditor(editor, levelName);
    }
}
