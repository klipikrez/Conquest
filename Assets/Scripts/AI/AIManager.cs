using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPlayer
{
    public int team;
    public AIType aiType;
    public int currentEnemyTeam = -52;
    public float numberOfUnits = 0;
    public float distress;
    public List<BuildingMain> buildings = new List<BuildingMain>();
    public Coroutine repeatingFunction;
    public bool isDead = false;

    public AIPlayer(int team)
    {
        this.team = team;
    }

    public static bool operator ==(AIPlayer a, AIPlayer b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.team == b.team;
    }

    public static bool operator !=(AIPlayer a, AIPlayer b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is AIPlayer other && team == other.team;
    }

    public override int GetHashCode()
    {
        return team.GetHashCode();
    }
}

public class AIManager : MonoBehaviour
{
    //player stats for ai
    public AIPlayer Player = new AIPlayer(1);
    //just a compiler object refrence
    public BuildingBehaviorCompiler bbc;
    //all buildigs in the game
    GameObject[] buildingObjects;
    //unit controllers for getting current number of units for each ai and plaer
    [System.NonSerialized]
    public List<BuildingMain> buildings = new List<BuildingMain>();
    //ai object types
    public AIType[] AITypeByTeam;
    //curently active ais
    public List<AIPlayer> AIPlayers = new List<AIPlayer>();

    //instranca
    public static AIManager Instance { get; private set; }

    private bool inicialized = false;


    private void Awake()
    {
        Instance = this;
    }

    public bool Inicialize()
    {
        bool hasAI = CompileAIs();
        InitiateAITeams();
        inicialized = true;
        return hasAI;
    }

    void Update()
    {
        if (!inicialized || LevelMenu.paused) return;
        UpdateStoredUnitCounts();
        foreach (BuildingMain building in buildings)
        {
            foreach (UnitAgent agent in building.unitController.agents)
            {
                if (agent.selfTeam == 1)
                {
                    Player.numberOfUnits++;
                }
                else
                {
                    AIPlayer ai = GetAIPlayer(agent.selfTeam);
                    if (ai != null)
                    {
                        ai.numberOfUnits++;
                    }
                }
            }
        }

        foreach (AIPlayer ai in AIPlayers)
        {
            if (ai.numberOfUnits <= 0 && !ai.isDead)
            {
                ai.isDead = true;
                StopCoroutine(ai.repeatingFunction);
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayAudioClip(4);
                }
            }
        }
        UnitAmountBarCalculator.Instance.UpdateValues();
    }

    private void UpdateStoredUnitCounts()
    {
        Player.numberOfUnits = GetUnitCount(Player.buildings);
        foreach (AIPlayer ai in AIPlayers)
        {
            ai.numberOfUnits = GetUnitCount(ai.buildings);
        }
    }

    private float GetUnitCount(List<BuildingMain> teamBuildings)
    {
        float unitCount = 0;
        foreach (BuildingMain building in teamBuildings)
        {
            unitCount += building.production.product;
        }
        return unitCount;
    }

    bool CompileAIs()
    {
        Dictionary<int, List<BuildingMain>> numberOfBuildingsPerTeam = new Dictionary<int, List<BuildingMain>>();

        buildingObjects = GameObject.FindGameObjectsWithTag("building");
        foreach (GameObject buildingObject in buildingObjects)
        {
            BuildingMain building = buildingObject.GetComponent<BuildingMain>();
            buildings.Add(building);
            if (building.team.teamid != 0)
            {
                if (building.team.teamid != 1)
                {
                    if (numberOfBuildingsPerTeam.ContainsKey(building.team.teamid))
                    {
                        numberOfBuildingsPerTeam[building.team.teamid].Add(building);
                    }
                    else
                    {
                        numberOfBuildingsPerTeam.Add(building.team.teamid, new List<BuildingMain> { building });
                    }
                }
                else
                {
                    Player.buildings.Add(building);
                }
            }
        }
        foreach (KeyValuePair<int, List<BuildingMain>> team in numberOfBuildingsPerTeam)
        {
            AIPlayers.Add(new AIPlayer(team.Key) { buildings = team.Value });
        }

        AIPlayers.Sort((left, right) => left.team.CompareTo(right.team));

        return AIPlayers.Count > 0;
    }

    public AIPlayer GetAIPlayer(int team)
    {
        return AIPlayers.Find(ai => ai.team == team);
    }

    void InitiateAITeams()
    {
        foreach (AIPlayer ai in AIPlayers)
        {
            if (ai.team >= 0 && ai.team < AITypeByTeam.Length && AITypeByTeam[ai.team] != null)
            {
                ai.aiType = AITypeByTeam[ai.team];
                ai.repeatingFunction = StartCoroutine(AIClockRepeating(ai));
            }
        }
    }
    IEnumerator AIClockRepeating(AIPlayer ai)
    {
        yield return new WaitForSeconds(ai.aiType.clockCycleTime);
        while (true)//  kor
        {
            yield return new WaitForSeconds(ai.aiType.clockCycleTime);
            while (LevelMenu.paused)
            {
                yield return null;
            }
            ai.aiType.CalculateMove(this, ai);
        }
    }

    public void UpdateTeamTowers(BuildingMain tower, int oldTeam, int newTeam)
    {
        if (oldTeam >= 2)
        {
            AIPlayer oldAI = GetAIPlayer(oldTeam);
            if (oldAI != null)
                oldAI.buildings.Remove(tower);
        }
        else
            if (oldTeam == 1)
                Player.buildings.Remove(tower);
        if (newTeam >= 2)
        {
            AIPlayer newAI = GetAIPlayer(newTeam);
            if (newAI != null && !newAI.buildings.Contains(tower))
                newAI.buildings.Add(tower);
        }
        else
            if (newTeam == 1)
                Player.buildings.Add(tower);

    }

}