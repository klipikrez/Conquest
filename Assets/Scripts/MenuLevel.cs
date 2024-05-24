using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tymski;
using UnityEngine;

public class MenuLevel : MonoBehaviour
{

    public string levelName = "brki";
    public TextMeshProUGUI text;
    LevelInfoManage manager;

    public void Initialize(string levelName, LevelInfoManage manager)
    {
        levelName = levelName.Substring(levelName.LastIndexOf('\\') + 1);
        this.levelName = levelName;
        text.text = levelName;
        this.manager = manager;
    }

    public void Selected()
    {
        manager.SetSelectedLevel(levelName);
    }

}
