using UnityEngine;

public abstract class AIState : ScriptableObject
{
    [System.Serializable]
    public struct AIBehaviorChances
    {
        public int chance;
        public AIBehavior behavior;
    }
    public AIBehaviorChances[] chanceBehaviorsTable;

    public abstract void CalculateMove(AIManager manager, AIPlayer player);

    protected bool ExecuteRandomBehavior(AIManager manager, AIPlayer player)
    {
        int action = GetAction();
        if (action < 0 || chanceBehaviorsTable[action].behavior == null)
        {
            return false;
        }

        return chanceBehaviorsTable[action].behavior.ExecuteMove(manager, player);
    }

    public int GetAction()
    {
        if (chanceBehaviorsTable == null || chanceBehaviorsTable.Length == 0)
        {
            return -1;
        }

        int randomValue = Random.Range(0, 101);
        int chanceTotal = 0;
        for (int i = 0; i < chanceBehaviorsTable.Length; i++)
        {
            chanceTotal += chanceBehaviorsTable[i].chance;
            if (randomValue < chanceTotal)
            {
                return i;
            }
        }

        return chanceBehaviorsTable.Length - 1;
    }
}