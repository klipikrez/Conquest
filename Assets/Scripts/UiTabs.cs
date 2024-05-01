using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct Tab
{
    public Image background;
    public Image checkmark;
}
public class UiTabs : MonoBehaviour
{

    public Tab[] Tabs;

    public void SelectTab(int val)
    {
        foreach (Tab tab in Tabs)
        {
            tab.background.color = Color.white;
            tab.checkmark.color = Color.black;
        }

        Tabs[val].background.color = Color.black;
        Tabs[val].checkmark.color = Color.white;
    }
}
