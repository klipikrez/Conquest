using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingMeshType { Default, Defense, Generator, Special };
public enum BuildingType { None, Defense, Generator, Special };

[CreateAssetMenu(menuName = "Building/Presets/Preset")]
public class BuildingPreset : ScriptableObject
{
    public string presetName = "New Building Preset";
    public int size = 1;//max5?
    public float product = 0;//nes nis dobit
    public float cost = 10;//nes nis dobit
    public float productProduction = 0.4f;//increment by 0.4?
    public int maxUnits = 10;//increment by 10?
    public float vulnerability = 0.81f;
    public BuildingMeshType meshType = BuildingMeshType.Default;
    public BuildingType buildingType;
    public void InitializeBuilding(UnitController controller)
    {

        switch (buildingType)
        {
            case BuildingType.None:

                break;
            case BuildingType.Defense:

                break;
            case BuildingType.Generator:

                break;
            case BuildingType.Special:

                break;
        }
    }
}

