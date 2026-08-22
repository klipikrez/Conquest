using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoManage : EditorLevelInfoManage
{

    public List<Sprite> dressingSprites;
    public Sprite defaultDressingSprite;
    public SnapToScrollViewItem snapToScrollViewItem;
    public override void SetSelectedLevel(string levelName)
    {
        //        Debug.Log(" - - - -- " + levelName);
        nameElement.text = levelName;
        this.levelName = levelName;

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "cover.png");


        if (Regex.Match(levelName, @"^\d+-").Success)
        {
            int levelSet = int.Parse(Regex.Match(levelName, @"^\d+-").Value.TrimEnd('-'));
            //Debug.Log("Level Set: " + levelSet + "for level: " + levelName);
            //Debug.Log(levelSet + " is less than or equal to dressingSprites.Count: " + dressingSprites.Count);
            if (dressingSprites.Count >= levelSet)
            {

                image.sprite = dressingSprites[levelSet];
                image.SetNativeSize();
                return;
            }

        }
        image.sprite = defaultDressingSprite;
        image.SetNativeSize();

    }
    public override void Snap()
    {
        snapToScrollViewItem.SnapToItem(levelName);
    }

}
