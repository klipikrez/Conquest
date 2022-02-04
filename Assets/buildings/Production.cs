using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Production : MonoBehaviour
{

    public float product = 0;
    public float productProduction = 2;
    public int maxUnits = 100;

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
    }

    void Update()
    {
        if (!Paused)
        {
            if (product < maxUnits)
            {
                product += productProduction * Time.deltaTime;
                if (team.teamid == WinConditions.Instance.PlayerTeam)
                {
                    WinConditions.Instance.AddPlayerProducedUnits(productProduction * Time.deltaTime);
                }
            }

            if (product > maxUnits + 1)
            {
                product -= (productProduction / 2) * (((product - maxUnits) / 50) + 0.3f) * Time.deltaTime;
            }
            numberRefrence.text = "<mspace=0.6em>" + ((int)product).ToString() + "</mspace>";
        }
    }
}
