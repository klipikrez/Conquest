using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BuildingMeshType { Default, Defense, Generator, Special };


[CreateAssetMenu(menuName = "Building/Presets/Preset")]
public class BuildingPreset : ScriptableObject
{
    public int[] enableOptions;
    public Texture2D image;
    public float product = 0;//nes nis dobit
    public float cost = 10;//nes nis dobit
    public float productProduction = 0.4f;//increment by 0.4?
    public int maxUnits = 10;//increment by 10?
    public float vulnerability = 0.81f;
    public Mesh mesh;
    public bool InitializeBuilding(UnitController controller, bool free = false)
    {

        if (cost <= controller.GetProduction().product)
        {
            if (!free)
                controller.GetProduction().SubtractProduct(cost);
            controller.GetProduction().productProduction = productProduction;
            controller.GetProduction().maxUnits = maxUnits;
            controller.GetTeam().vulnerability = vulnerability;

            controller.team.SetMesh(mesh);

            EnableOptions(controller, enableOptions);
            return true;
        }
        else
        {
            Debug.Log(cost + " " + controller.GetProduction().product);
            //            ErrorManager.Instance.SendError("You dont have enough units to upgrade...");
            return false;
        }
    }

    void EnableOptions(UnitController controller, int[] enable)
    {
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
    }
}

