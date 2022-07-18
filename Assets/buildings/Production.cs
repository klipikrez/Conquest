using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Production : MonoBehaviour
{
    public int size = 1;
    public float product = 0;
    public float productProduction = 0.5f;
    [System.NonSerialized]
    public int maxUnits = 100;
    public TextMeshProUGUI numberRefrence;
    Team team;

    public bool Paused = false;


    private void Start()
    {
        team = GetComponent<Team>();
        if (WinConditions.Instance != null && team.teamid == WinConditions.Instance.PlayerTeam)
        {
            WinConditions.Instance.AddProducedUnits(product, team.teamid);
        }
        team.meshRenderer.gameObject.transform.localScale *= 0.8f + size * 0.15f;
    }

    void Update()
    {
        if (!Paused)
        {
            if (product < maxUnits)
            {
                product += productProduction * Time.deltaTime;
                if (WinConditions.Instance != null && team.teamid == WinConditions.Instance.PlayerTeam)
                {
                    WinConditions.Instance.AddProducedUnits(productProduction * Time.deltaTime, team.teamid);
                }
            }

            if (product > maxUnits + 1)
            {
                product -= (productProduction) * (((product - maxUnits) / 50) + 0.3f) * Time.deltaTime;
            }
            numberRefrence.text = "<mspace=0.6em>" + ((int)product).ToString() + "</mspace>";
        }
    }
}
