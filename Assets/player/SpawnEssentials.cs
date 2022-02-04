using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEssentials : MonoBehaviour
{
    public GameObject[] spawnThis = new GameObject[2];
    public GameObject ScenesManagerSpawn;
    private void Awake()
    {
        foreach (GameObject eyBabyWasYarNumbah in spawnThis)
        {
            Instantiate(eyBabyWasYarNumbah);
        }
        if (ScenesManager.Instance == null)
        {
            Instantiate(ScenesManagerSpawn);
        }

    }
}
