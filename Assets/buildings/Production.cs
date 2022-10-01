using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Production : MonoBehaviour
{
    public int size = 1;
    public float productStartAmountAdd = 0;
    [System.NonSerialized]
    public float product = 0f;
    public float productProduction = 0.5f;
    [System.NonSerialized]
    public int maxUnits = 100;
    public TextMeshProUGUI numberRefrence;
    public Team team { get; private set; }

    public bool Paused = false;


    private void Start()
    {
        AddProduct(productStartAmountAdd);
        team = GetComponent<Team>();
        /*if (WinConditions.Instance != null && team.teamid == WinConditions.Instance.PlayerTeam)
        {
            WinConditions.Instance.AddProducedUnits(product, team.teamid);
        }*/
        //team.meshRenderer.gameObject.transform.localScale *= 0.8f + size * 0.15f;
    }

    public void SetProduct(float value)
    {
        product = value;
        // WinConditions.Instance.AddProducedUnits(product, team.teamid);
    }
    public void SubtractProduct(float value)
    {
        product -= value;
        // WinConditions.Instance.AddProducedUnits(-value, team.teamid);
    }
    public void AddProduct(float value)
    {
        product += value;
        // WinConditions.Instance.AddProducedUnits(value, team.teamid);
    }

    void Update()
    {
        if (!Paused)
        {
            if (product < maxUnits)
            {
                product += productProduction * Time.deltaTime;
                /*Debug.Log(team.teamid);
                if (WinConditions.Instance != null && team.teamid == WinConditions.Instance.PlayerTeam)
                {
                    WinConditions.Instance.AddProducedUnits(productProduction * Time.deltaTime, team.teamid);
                }*/
            }

            if (product > maxUnits + 1)
            {
                product -= (productProduction) * (((product - maxUnits) / 50) + 0.3f) * Time.deltaTime;
            }
            numberRefrence.text = "<mspace=0.6em>" + ((int)product).ToString() + "</mspace>";
        }
    }
}
