using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/Peaceful")]
public class AIStatePeaceful : AIState
{

    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        Debug.Log("peace");
        int action = GetAction();
        chanceBehaviorsTable[action].behavior.ExecuteMove(manager, player);
        /*
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
        }



*/
    }

}
