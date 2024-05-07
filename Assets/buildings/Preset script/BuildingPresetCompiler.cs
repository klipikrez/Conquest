using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Building/Presets/PresetCompiler")]

public class BuildingPresetCompiler : ScriptableObject
{
    public BuildingPreset[] presets;

    public bool InitializeBuilding(int index, UnitController controller, bool free = false)
    {
        return presets[index].InitializeBuilding(controller, free);
    }
}
