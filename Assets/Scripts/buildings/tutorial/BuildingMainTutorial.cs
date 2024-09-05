using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMainTutorial : MonoBehaviour
{

    public GameObject rect;
    public GameObject mainCanvas;

    BuildingUI buildingMain;

    private void Start()
    {
        buildingMain = GetComponent<BuildingUI>();
    }

    // Update is called once per frame
    public void UpdateUI()
    {


        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position + buildingMain.UiOffset);
        rect.transform.position = pos;
    }
}
