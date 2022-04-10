using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public int phaze = 1;
    BuildingMain building;
    BuildingMain buildingEmemy;
    public BuildingMainTutorial tutorialBuilding;
    public BuildingMainTutorial tutorialBuildingEnemy;
    private void Awake()
    {

    }
    private void Start()
    {
        building = tutorialBuilding.gameObject.GetComponent<BuildingMain>();
        buildingEmemy = tutorialBuildingEnemy.gameObject.GetComponent<BuildingMain>();
        tutorialBuildingEnemy.rect.SetActive(false);


                    tutorialBuilding.rect.SetActive(true);
                    tutorialBuilding.mainCanvas.SetActive(true);
                    tutorialBuildingEnemy.mainCanvas.SetActive(true);
                    building.enabled = true;
                    buildingEmemy.enabled = true;

    }

    private void Update()
    {
        PhazeChecker();
    }

    public void PhazeChecker()
    {
        switch (phaze)
        {
            case 0:
                if (Input.anyKeyDown)
                {

                }
                break;
            case 1:
                if (building.selected)
                {
                    phaze = 2;
                    tutorialBuilding.rect.SetActive(false);
                    tutorialBuildingEnemy.rect.SetActive(true);

                }
                else
                {
                    tutorialBuilding.UpdateUI();
                }
                break;
            case 2:
                if (building.team.controller.agents.Count > 0)
                {
                    phaze = 3;
                    tutorialBuildingEnemy.rect.SetActive(false);
                }
                else
                {

                    if (!building.selected)
                    {
                        tutorialBuilding.rect.SetActive(true);
                        tutorialBuildingEnemy.rect.SetActive(false);
                        phaze = 1;
                    }
                    else
                    {
                        tutorialBuildingEnemy.UpdateUI();
                    }
                }
                break;
            case 3:

                break;
            default:
                break;
        }
    }

}