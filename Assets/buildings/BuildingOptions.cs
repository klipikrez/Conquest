using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class BuildingOptions : MonoBehaviour
{

    public BuildingPresetCompiler presetCompiler;

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
        presetCompiler.InitializeBuilding(index, GetComponent<UnitController>());
    }

    public void Gift(int percent)
    {
        playerSelectionDictionary.Instance.Gift(percent);
    }

}
