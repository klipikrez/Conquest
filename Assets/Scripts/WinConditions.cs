using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WinConditions : MonoBehaviour
{
    List<Team> buildings = new List<Team>();

    public int PlayerTeam = 1;

    bool noMoreEnemyTowersLeft = false;
    bool noMorePlayerTowersLeft = false;
    bool GameOver = false;

    // True if there were AI players when the level started.
    bool hadAIPlayersAtStart = false;
    bool inicialized = false;
    [Serializable]
    public struct ValueTimePair
    {
        public float value;
        public float time;

        public ValueTimePair(float value, float time)
        {
            this.value = value;
            this.time = time;
        }
    }

    [SerializeField]
    public Dictionary<int, List<ValueTimePair>> UnitsProduced =
        new Dictionary<int, List<ValueTimePair>>();

    public static WinConditions Instance { get; private set; }

    LevelMenu levelMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(bool hadAIPlayersAtStart)
    {
        levelMenu = FindFirstObjectByType<LevelMenu>();

        // Remember whether the level STARTED with any AI players.
        this.hadAIPlayersAtStart = hadAIPlayersAtStart;
        inicialized = true;
    }

    private void Update()
    {
        if (GameOver || !inicialized || LevelMenu.paused)
            return;
        //Debug.Log($"WinConditions Update: hadAIPlayersAtStart={hadAIPlayersAtStart}");
        CheckWinOrLose();
    }

    private void CheckWinOrLose()
    {
        // -----------------------------------------
        // LOSE CONDITION
        // -----------------------------------------

        if (noMorePlayerTowersLeft)
        {
            if (AIManager.Instance.Player.numberOfUnits <= 0)
            {
                Izgubida();
                return;
            }
        }

        // -----------------------------------------
        // WIN CONDITION
        // -----------------------------------------

        if (hadAIPlayersAtStart)
        {
            // -----------------------------------------
            // LEVEL STARTED WITH AI PLAYERS
            // -----------------------------------------
            //
            // Neutral towers (teamid == 0) don't matter.
            // We only need every AI player to be dead.
            //

            bool allAIPlayersDead = true;
            //Debug.Log($"Checking win condition: {AIManager.Instance.AIPlayers.Count} AI players");
            foreach (AIPlayer ai in AIManager.Instance.AIPlayers)
            {
                //Debug.Log($"Checking AI player {ai.team}: units={ai.numberOfUnits}, buildings={ai.buildings.Count}");
                if (ai.numberOfUnits > 0 || ai.buildings.Count > 0)
                {
                    allAIPlayersDead = false;
                    break;
                }
            }

            if (allAIPlayersDead)
            {
                Pobeda();
            }
        }
        else
        {
            //Debug.Log("Checking win condition: no AI players at start, checking if all towers are taken");
            // -----------------------------------------
            // LEVEL STARTED WITH NO AI PLAYERS
            // -----------------------------------------
            //
            // There were no AI players, so ALL towers
            // must be taken/destroyed, including neutral
            // teamid == 0 towers.
            //

            if (AllTowersTaken())
            {
                Pobeda();
            }
        }
    }

    private bool AllTowersTaken()
    {
        foreach (BuildingMain building in AIManager.Instance.buildings)
        {

            // Any building that isn't owned by the player
            // means there are still towers left to take.
            if (building.team.teamid != PlayerTeam &&
                building.unitDetector.Engage &&
                !building.unitDetector.Imune)
            {
                //Debug.Log("Building not taken: " + building.name);
                return false;
            }
        }

        return true;
    }

    public void AddBuildingTeam(Team team)
    {
        buildings.Add(team);
    }

    public void CheckTeams()
    {
        // -----------------------------------------
        // PLAYER TOWERS
        // -----------------------------------------

        noMorePlayerTowersLeft =
            AIManager.Instance.Player.buildings.Count == 0;


        // -----------------------------------------
        // ENEMY TOWERS
        // -----------------------------------------

        bool enemyTowersLeft = false;

        foreach (AIPlayer ai in AIManager.Instance.AIPlayers)
        {
            if (ai.buildings.Count > 0)
            {
                enemyTowersLeft = true;
                break;
            }
        }

        // Check buildings directly as well.
        foreach (BuildingMain building in AIManager.Instance.buildings)
        {
            if (building == null)
                continue;

            if (building.team.teamid != PlayerTeam &&
                building.unitDetector.Engage &&
                !building.unitDetector.Imune)
            {
                enemyTowersLeft = true;
                break;
            }
        }

        noMoreEnemyTowersLeft = !enemyTowersLeft;
    }

    public void AddProducedUnits(float amount, int team)
    {
        List<ValueTimePair> banalno = new List<ValueTimePair>();

        ValueTimePair josBanalnije =
            new ValueTimePair(amount, LevelMenu.timeSinceStart);

        banalno.Add(josBanalnije);

        if (UnitsProduced.ContainsKey(team))
        {
            UnitsProduced[team].Add(josBanalnije);
        }
        else
        {
            UnitsProduced.Add(team, banalno);
        }
    }

    public void Pobeda()
    {
        Debug.Log("Pobeda");
        if (GameOver)
            return;

        GameOver = true;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudioClip(2);
            SoundManager.Instance.PlayAudioClip(3);
        }

        if (levelMenu != null)
        {
            levelMenu.WinScreen();
        }
    }

    public void Izgubida()
    {
        if (GameOver)
            return;

        GameOver = true;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudioClip(0);
            SoundManager.Instance.PlayAudioClip(1);
        }

        if (levelMenu != null)
        {
            levelMenu.LoseScreen();
        }
    }
}