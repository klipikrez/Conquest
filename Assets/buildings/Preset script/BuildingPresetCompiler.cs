using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Building/Presets/PresetCompiler")]

public class BuildingPresetCompiler : ScriptableObject
{

    public BuildingPreset defaultPreset;
    public BuildingPreset[] presetDefensive;
    public BuildingPreset[] presetGenertor;
    public BuildingPreset[] presetSpecial;

    public void InitializeBuilding(int index, UnitController controller)
    {
        switch (index)
        {
            case 0:
                defaultPreset.InitializeBuilding(controller);
                break;
            case 1:
                //defense

                break;
            case 2:
                //generator
                break;
            case 3:
                //special
                break;
            case 4:
                //ally
                break;
        }




    }
}
