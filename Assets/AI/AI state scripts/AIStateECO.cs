using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/ECO")]
public class AIStateECO : AIState
{
    public int checkUpgrade = 0;
    public BuildingPreset autoUpgradeIfPossible;
    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        bool weDintDoIt = true;
        foreach (UnitController ctrl in player.Towers)//before everything we see if we can upgrade the default towers
        {
            BuildingOptions options = ctrl.GetBuildingOptions();
            if (options.currentUpgrade == checkUpgrade)
            {
                if (autoUpgradeIfPossible.cost < ctrl.production.product)
                {
                    options.UpgradePreset(autoUpgradeIfPossible);
                    weDintDoIt = false;
                }
            }
        }
        if (weDintDoIt)
        {
            int action = GetAction();
            chanceBehaviorsTable[action].behavior.ExecuteMove(manager, player);
        }       /*
        //za sd samo jedna sttvar
        //Be00GraD
        //Be0-Grad
        int randz = Random.Range(0, 100);
        int randomBehavior = 0;
        //Debug.Log(randz);
        if (randz < rngStateChances[0])
        {
            randomBehavior = 0;
        }
        else
        {
            if (randz < rngStateChances[0] + rngStateChances[1])
            {
                randomBehavior = 1;
            }
            else
            {
                randomBehavior = 2;
            }
        }

        switch (randomBehavior)
        {
            case 0:
                {
                    //Debug.Log("vidi ga gedza napada");
                    attackBehavior.ExecuteMove(manager, player);
                    return;
                }
            case 1:
                {
                    //Debug.Log("zamak");
                    defendBehavior.ExecuteMove(manager, player);
                    return;
                }
            case 2:
                {
                    //Debug.Log("kolums");
                    expandBehavior.ExecuteMove(manager, player);
                    return;
                }
            default:
                {
                    Debug.Log("nes n valja");
                    return;
                }
        }*/




    }
}
