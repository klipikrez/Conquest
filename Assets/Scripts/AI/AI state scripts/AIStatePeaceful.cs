using UnityEngine;

[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/Peaceful")]
public class AIStatePeaceful : AIState
{
    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        Debug.Log("peace");
        ExecuteRandomBehavior(manager, player);
    }
}
