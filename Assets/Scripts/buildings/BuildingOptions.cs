using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class BuildingOptions : MonoBehaviour
{

    public TowerPresetData preset;



    [System.NonSerialized]
    public BuildingMain building;


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

        /* if (presetCompiler.InitializeBuilding(avalibeUpgrades[index], building.unitController))
         {
             currentUpgrade = avalibeUpgrades[index];
         }
         else
         {
             int i = Random.Range(8, 10);
             SoundManager.Instance.PlayAudioClip(i);
         }*/

    }
    public void UpgradeDirect(int index)
    {

        /*if (presetCompiler.InitializeBuilding(index, building.unitController))
        {
            currentUpgrade = avalibeUpgrades[index];
        }
        else
        {
            int i = Random.Range(8, 10);
            SoundManager.Instance.PlayAudioClip(i);
        }*/
    }

    public void UpgradePreset(int preset)
    {
        //NOT IMPLEMENTED
        /*if (preset.InitializeBuilding(building.unitController))
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
        }*/
    }

    public void Gift(int percent)
    {
        playerSelectionDictionary.Instance.Gift(percent);
    }

}
