using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct Tab
{
    public Image background;
    public Image checkmark;
    public GameObject info;
}
public class UiTabs : MonoBehaviour
{

    public Tab[] Tabs;

    private void Start()
    {
        SelectTab(-52);
    }
    public void SelectTab(int val)
    {
        foreach (Tab tab in Tabs)
        {
            tab.background.color = Color.white;
            tab.checkmark.color = Color.black;
            if (tab.info != null) tab.info.SetActive(false);
        }
        if (val < 0) return;
        if (Tabs[val].info != null) Tabs[val].info.SetActive(true);
        Tabs[val].background.color = Color.black;
        Tabs[val].checkmark.color = Color.white;
    }
}
