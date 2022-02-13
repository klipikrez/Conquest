using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AIPlayer
{
    public int team;
    public int currentEnemyTeam;
    public int numberOfUnits;
    public float distress;
    public List<UnitController> Towers = new List<UnitController>();

    public AIPlayer(int team)
    {
        this.team = team;
    }
}

public class AIManager : MonoBehaviour
{
    GameObject[] buildings;
    public AIType[] AITypeByTeam;
    public List<AIPlayer> AIPlayers = new List<AIPlayer>();


    public static AIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CompileAIs();
        InitiateAITeams();
    }

    void CompileAIs()
    {
        Dictionary<int, List<UnitController>> numberOfTowersPerTeam = new Dictionary<int, List<UnitController>>();

        buildings = GameObject.FindGameObjectsWithTag("building");
        foreach (GameObject building in buildings)
        {

            UnitController controller = building.GetComponent<UnitController>();
            if (controller.GetTeam().teamid != 0 && controller.GetTeam().teamid != 1)//znaci da nije siv i player
            {


                if (numberOfTowersPerTeam.ContainsKey(controller.GetTeam().teamid))// checks if team already exists in numberOfTowersPerTeam
                {
                    //if AIPlayers List contains, then add tower to numberOfTowersPerTeam dictionary
                    numberOfTowersPerTeam[controller.GetTeam().teamid].Add(controller);
                }
                else
                {
                    //else add new AIPlayer object to AIPlayers list
                    numberOfTowersPerTeam.Add(controller.GetTeam().teamid, new List<UnitController>());
                    numberOfTowersPerTeam[controller.GetTeam().teamid].Add(controller);
                }
            }
        }
        foreach (var temp in numberOfTowersPerTeam)
        {
            AIPlayer playerTmp = new AIPlayer(temp.Key);

            foreach (var tmp2 in temp.Value)
            {
                playerTmp.Towers.Add(tmp2);
            }

            AIPlayers.Add(playerTmp);
        }
    }

    void InitiateAITeams()
    {
        foreach (AIPlayer ai in AIPlayers)
        {
            //Debug.Log(ai.team + "    " + AITypeByTeam.Length + "   " + (ai.team <= AITypeByTeam.Length - 1));
            if (ai.team <= AITypeByTeam.Length - 1)//check if team ai exists
            {
                //Debug.Log(AITypeByTeam[ai.team].name);
                StartCoroutine(AIClockRepeating(ai));// :( mrzim corutine
            }
        }
    }
    IEnumerator AIClockRepeating(AIPlayer ai)
    {
        while (true)//  kor
        {
            yield return new WaitForSeconds(AITypeByTeam[ai.team].clockCycleTime);
            //Debug.Log(AITypeByTeam[ai.team].clockCycleTime);
            AITypeByTeam[ai.team].CalculateMove(this, ai);
        }
    }

    public void UpdateTeamTowers(UnitController tower, int oldTeam, int newTeam)
    {



        if (oldTeam >= 3)
            AIPlayers[oldTeam - 2].Towers.Remove(tower);
        AIPlayers[newTeam - 2].Towers.Add(tower);
    }

}
