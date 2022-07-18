using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAmountBarCalculator : MonoBehaviour
{
    public AmountBar[] bars;


    public static UnitAmountBarCalculator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateValues()
    {
        float totalUnits = AIManager.Instance.Player.numberOfUnits;

        foreach (AIPlayer ai in AIManager.Instance.AIPlayers)
        {
            totalUnits += ai.numberOfUnits;
        }
        float total = 0; // goes to max 100%

        float currentAmount1 = AIManager.Instance.Player.numberOfUnits / totalUnits;
        bars[0].UpdateValue(total, currentAmount1 + total, AIManager.Instance.Player.numberOfUnits, AIManager.Instance.bbc.buildingBehaviors[1].color);
        total += currentAmount1;

        for (int i = 0; i < AIManager.Instance.AIPlayers.Count; i++)
        {
            if (!AIManager.Instance.AIPlayers[i].isDead)
            {
                float currentAmount = AIManager.Instance.AIPlayers[i].numberOfUnits / totalUnits;
                bars[i + 1].UpdateValue(total, currentAmount + total, AIManager.Instance.AIPlayers[i].numberOfUnits, AIManager.Instance.bbc.buildingBehaviors[i + 2].color);
                total += currentAmount;
            }
            else
            {
                bars[i + 1].UpdateValue(1, 1, 0, Color.white);
            }
        }
    }
}
