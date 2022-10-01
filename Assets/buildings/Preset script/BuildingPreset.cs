using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BuildingMeshType { Default, Defense, Generator, Special };
public enum BuildingType { None, Defense, Generator, Special };

[CreateAssetMenu(menuName = "Building/Presets/Preset")]
public class BuildingPreset : ScriptableObject
{
    public string presetName = "New Building Preset";
    public int[] enableOptions;
    public int size = 1;//max5?
    public float product = 0;//nes nis dobit
    public float cost = 10;//nes nis dobit
    public float productProduction = 0.4f;//increment by 0.4?
    public int maxUnits = 10;//increment by 10?
    public float vulnerability = 0.81f;
    public BuildingMeshType meshType = BuildingMeshType.Default;
    public BuildingType buildingType;
    public bool InitializeBuilding(UnitController controller)
    {

        if (cost <= controller.GetProduction().product)
        {
            //set mesh type
            controller.GetProduction().size = size;
            controller.GetProduction().SubtractProduct(cost);
            controller.GetProduction().productProduction = productProduction;
            controller.GetProduction().maxUnits = maxUnits;
            controller.GetTeam().vulnerability = vulnerability;

            //verovatno nece da ostan eovako isped
            switch (buildingType)
            {
                case BuildingType.None:
                    controller.team.SetMesh(controller.UnitBuildingSpawnBehavior.buildingBehaviors[controller.team.teamid].defaultBuildingMesh[size]);

                    break;
                case BuildingType.Defense:
                    controller.team.SetMesh(controller.UnitBuildingSpawnBehavior.buildingBehaviors[controller.team.teamid].defenseBuildingMesh[size - 1]);

                    break;
                case BuildingType.Generator:
                    controller.team.SetMesh(controller.UnitBuildingSpawnBehavior.buildingBehaviors[controller.team.teamid].productionBuildingMesh[size - 1]);

                    break;
                case BuildingType.Special:
                    controller.team.SetMesh(controller.UnitBuildingSpawnBehavior.buildingBehaviors[controller.team.teamid].specialBuildingMesh[size - 1]);
                    break;
            }
            //            List<int> enableList = new List<int>(enableOptions);
            EnableOptions(controller, enableOptions);
            return true;
        }
        else
        {
            Debug.Log(cost + " " + controller.GetProduction().product);
            return false;
        }
    }

    void EnableOptions(UnitController controller, int[] enable)
    {
        // Debug.Log(enable.Length + "" + controller.gameObject.name);
        Button[] uiElements = controller.GetBuildingMain().otherOptions;
        TextMeshProUGUI[] costTexts = controller.GetBuildingMain().optionsCost;
        foreach (Button uiElement in uiElements)
        {
            uiElement.gameObject.SetActive(false);
        }
        controller.GetBuildingOptions().avalibeUpgrades = enable;

        for (int i = 0; i < enable.Length; i++)
        {

            uiElements[i].gameObject.SetActive(true);
            costTexts[i].text = controller.GetBuildingOptions().presetCompiler.presets[enable[i]].cost.ToString();

        }
        /*
                switch (enable.Length)
                {
                    case 0:
                        break;
                    case 1:
                        uiElements[0].gameObject.SetActive(true);
                        costTexts[0].text = controller.GetBuildingOptions().presetCompiler.presets[enable[0]].cost.ToString();
                        break;
                    case 2:
                        uiElements[0].gameObject.SetActive(true);
                        uiElements[1].gameObject.SetActive(true);
                        costTexts[0].text = controller.GetBuildingOptions().presetCompiler.presets[enable[0]].cost.ToString();
                        costTexts[0].text = controller.GetBuildingOptions().presetCompiler.presets[enable[1]].cost.ToString();
                        break;
                    case 3:
                        uiElements[0].gameObject.SetActive(true);
                        uiElements[1].gameObject.SetActive(true);
                        uiElements[2].gameObject.SetActive(true);
                        costTexts[0].text = controller.GetBuildingOptions().presetCompiler.presets[enable[0]].cost.ToString();
                        costTexts[0].text = controller.GetBuildingOptions().presetCompiler.presets[enable[1]].cost.ToString();
                        costTexts[0].text = controller.GetBuildingOptions().presetCompiler.presets[enable[2]].cost.ToString();
                        break;

                    default:
                        break;
                }*/

    }

}

