using UnityEngine;

[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/ECO")]
public class AIStateECO : AIState
{
    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        Debug.Log("eco");
        ExecuteRandomBehavior(manager, player);
    }
}
