using UnityEngine;

[CreateAssetMenu(fileName = "newAIType", menuName = "AI/Type")]
public class AIType : ScriptableObject
{
    public string TypeName = "Default:)";
    public float clockCycleTime = 1;/*in seconds*/
    public AIState[] States;
    public float aggresivnes = 20;

    public void CalculateMove(AIManager manager, AIPlayer player)
    {
        //Debug.Log(player.team);
        States[0].CalculateMove(manager, player);
        //Debug.Log("uspesno");
    }
}