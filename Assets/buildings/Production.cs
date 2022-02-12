using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Production : MonoBehaviour
{
    public int size = 1;
    public float product = 0;
    public float startRandomasationAdd = 30f;
    public float productProduction = 0.5f;
    [System.NonSerialized]
    public float productionDullMultiplyer = 0.2f;
    public float startProductionAdd = 1f;
    public int maxUnits = 100;
    public float startmaxUnitsAdd = 100f;

    public TextMeshProUGUI numberRefrence;
    Team team;

    public bool Paused = false;

    private void Start()
    {
        team = GetComponent<Team>();
        if (team.teamid == WinConditions.Instance.PlayerTeam)
        {
            WinConditions.Instance.AddPlayerProducedUnits(product);
        }
        else
        {
            if (team.teamid == 0)
            {
                //randomize dull unit castles
                product += Mathf.PerlinNoise(transform.position.x * 1000, transform.position.z * 1000) * startRandomasationAdd * size;

                productProduction += Mathf.PerlinNoise(transform.position.x * 1000, transform.position.z * 1000) * startProductionAdd * size;
                maxUnits += (int)((Mathf.PerlinNoise(transform.position.x * 1000, transform.position.z * 1000)) * startmaxUnitsAdd * size);
            }
        }
        team.meshRenderer.gameObject.transform.localScale *= 0.8f + size * 0.15f;
    }

    void Update()
    {
        if (!Paused)
        {
            if (product < maxUnits)
            {
                product += productProduction * productionDullMultiplyer * Time.deltaTime;
                if (team.teamid == WinConditions.Instance.PlayerTeam)
                {
                    WinConditions.Instance.AddPlayerProducedUnits(productProduction * productionDullMultiplyer * Time.deltaTime);
                }
            }

            if (product > maxUnits + 1)
            {
                product -= (productProduction * productionDullMultiplyer / 2) * (((product - maxUnits) / 50) + 0.3f) * Time.deltaTime;
            }
            numberRefrence.text = "<mspace=0.6em>" + ((int)product).ToString() + "</mspace>";
        }
    }
}
