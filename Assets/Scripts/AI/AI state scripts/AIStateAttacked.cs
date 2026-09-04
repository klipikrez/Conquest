using UnityEngine;
[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/Attacked")]
public class AIStateAttacked : AIState
{
    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        Debug.Log("attacked");
        ExecuteRandomBehavior(manager, player);
    }

}
