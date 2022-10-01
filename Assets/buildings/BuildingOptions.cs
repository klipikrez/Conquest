using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class BuildingOptions : MonoBehaviour
{

    public BuildingPresetCompiler presetCompiler;
    public BuildingPreset overridePreset;
    UnitController controller;
    public int[] avalibeUpgrades;
    public int currentUpgrade;
    private void Start()
    {
        controller = GetComponent<UnitController>();
        if (overridePreset == null)
        {
            currentUpgrade = 0;
            controller.GetProduction().AddProduct(presetCompiler.presets[0].cost);
            //Upgrade(0);//initialize building as default if ovveride is null
            presetCompiler.InitializeBuilding(0, controller);
            controller.GetProduction().AddProduct(presetCompiler.presets[0].product);

        }
        else
        {
            int index = 0;//matematka
            foreach (BuildingPreset preset in presetCompiler.presets)
            {
                if (preset == overridePreset)
                {
                    currentUpgrade = index;//find current ubgrade?
                    break;
                }
                index++;
            }

            controller.GetProduction().AddProduct(overridePreset.cost);
            overridePreset.InitializeBuilding(controller);
            controller.GetProduction().AddProduct(overridePreset.product);


        }
        if (controller.team.teamid == 0)
        {//if neutral dont produce
            controller.GetProduction().productProduction /= 100f;
        }

    }

    public void FullAttack(int percent)
    {
        playerSelectionDictionary.Instance.Attack(playerSelectionDictionary.Instance.optionsActive.transform, percent);
    }

    public void ContinuousAttack()
    {
        playerSelectionDictionary.Instance.ContinuousAttack();
    }

    public void Upgrade(int index)
    {

        if (presetCompiler.InitializeBuilding(avalibeUpgrades[index], controller))
        {
            currentUpgrade = avalibeUpgrades[index];
        }
        else
        {
            int i = Random.Range(8, 10);
            SoundManager.Instance.PlayAudioClip(i);
        }

    }
    public void UpgradeDirect(int index)
    {

        if (presetCompiler.InitializeBuilding(index, controller))
        {
            currentUpgrade = avalibeUpgrades[index];
        }
        else
        {
            int i = Random.Range(8, 10);
            SoundManager.Instance.PlayAudioClip(i);
        }
    }

    public void UpgradePreset(BuildingPreset preset)
    {
        if (preset.InitializeBuilding(controller))
        {
            int index = 0;//matematka
            foreach (BuildingPreset preset2 in presetCompiler.presets)
            {
                if (preset2 == overridePreset)
                {
                    currentUpgrade = index;
                    break;
                }
                index++;
            }
        }
        else
        {
            int i = Random.Range(8, 10);
            SoundManager.Instance.PlayAudioClip(i);
        }
    }

    public void Gift(int percent)
    {
        playerSelectionDictionary.Instance.Gift(percent);
    }

}
