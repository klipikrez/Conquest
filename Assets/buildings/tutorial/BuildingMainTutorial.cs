using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMainTutorial : MonoBehaviour
{

    public GameObject rect;
    public GameObject mainCanvas;

    BuildingMain buildingMain;

    private void Start()
    {
        buildingMain = GetComponent<BuildingMain>();
    }

    // Update is called once per frame
    public void UpdateUI()
    {


        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position + buildingMain.UiOffset);
        rect.transform.position = pos;
    }
}
