using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    GameObject[] buildings;
    public class AIPlayer
    {
        public int team;
        public int numberOfUnits;
        public float distress;
        public List<UnitController> Towers = new List<UnitController>();

        public AIPlayer(int team)
        {
            this.team = team;
        }
    }

    public List<AIPlayer> AIPlayers = new List<AIPlayer>();
    private void Start()
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
}
