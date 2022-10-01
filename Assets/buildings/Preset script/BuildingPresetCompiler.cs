using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Building/Presets/PresetCompiler")]

public class BuildingPresetCompiler : ScriptableObject
{
    public BuildingPreset[] presets;/*
    public BuildingPreset defaultPreset;
    public BuildingPreset[] presetDefensive;
    public BuildingPreset[] presetGenertor;
    public BuildingPreset[] presetSpecial;*/

    public bool InitializeBuilding(int index, UnitController controller)
    {


        bool value = false;
        value = presets[index].InitializeBuilding(controller);
        /*
                switch (index)
                {
                    case 0:
                        //default
                        value = defaultPreset.InitializeBuilding(controller);
                        break;
                    case 1:
                        //defense0
                        value = presetDefensive[0].InitializeBuilding(controller);
                        break;
                    case 2:
                        //generator0
                        value = presetGenertor[0].InitializeBuilding(controller);
                        break;
                    case 3:
                        //special(not inplemented)
                        value = presetSpecial[0].InitializeBuilding(controller);
                        break;
                    case 4://gernerator1
                        value = presetGenertor[1].InitializeBuilding(controller);
                        break;
                    case 5://generator2
                        value = presetGenertor[2].InitializeBuilding(controller);
                        break;
       
    } */
        return value;



    }
}
