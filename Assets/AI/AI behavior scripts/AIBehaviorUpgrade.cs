using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIBehavior", menuName = "AI/Behaviors/ECO")]
public class AIBehaviorUpgradeECO : AIBehavior
{
    public float selfAmountUpgrade = 1.1f;
    //upgrades that can be used, in order.
    public int[] avalibeUpgrades = new int[4] { 2, 4, 5, 6 };
    public override bool ExecuteMove(AIManager manager, AIPlayer player)
    {
        bool weDidSomething = false;
        foreach (UnitController tower in player.Towers)
        {
            bool allNeighboursAreFriendly = true;
            foreach (UnitController neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {

                if (neighbor.team.teamid != player.team)
                {
                    allNeighboursAreFriendly = false;
                }
            }
            if (allNeighboursAreFriendly)//if t friendly go ahead
            {
                BuildingOptions options = tower.GetBuildingOptions();
                foreach (int i in avalibeUpgrades)//twst if can be upgraded as desired
                {
                    foreach (int j in options.avalibeUpgrades)
                    {
                        if (i == j)
                        {
                            if (tower.production.product > options.presetCompiler.presets[i].cost * selfAmountUpgrade)
                            {
                                options.UpgradeDirect(i);
                                weDidSomething = true;
                            }
                        }
                    }
                }
            }


        }
        return weDidSomething;//yay you did it :D 

    }



}
