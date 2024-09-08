using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    [System.Serializable]
    public struct AIBehaviorChances
    {
        public int chance;
        public AIBehavior behavior;
    }
    //public int[] rngStateChances = new int[3] { 5, 35, 60 };//treba ukupno da bude 100 
    public AIBehaviorChances[] chanceBehaviorsTable;
    //public AIBehaviorAttack attackBehavior;
    //public AIBehaviorDefend defendBehavior;
    //public AIBehaviorExpand expandBehavior;
    public abstract void CalculateMove(AIManager manager, AIPlayer player);
    public int GetAction()
    {
        //calculate ranom number based on chance table
        System.Random r = new System.Random();
        int randz = r.Next(101); /*Random.Range(101);*/
        Debug.Log("" + randz);

        int chanceSum = chanceBehaviorsTable[0].chance;
        for (int i = 0; i < chanceBehaviorsTable.Length; i++)
        {
            if (chanceSum >= randz)
            {
                return i;
            }
            chanceSum += chanceBehaviorsTable[i + 1].chance;
        }

        /*for (int i = 0; i < chanceBehaviorsTable.Length; i++)
        {
            int chance = 0;
            for (int j = 0; j < i; j++)
            {
                chance += chanceBehaviorsTable[j].chance;
                if (chance > randz)
                {
                    return i;
                }
            }

        }*/
        return chanceBehaviorsTable.Length - 1;
    }
}
