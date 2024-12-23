using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WinConditions : MonoBehaviour
{

    List<Team> buildings = new List<Team>();//added in Buildings Team.cs scripts Start()
    public int PlayerTeam = 1;
    bool noMoreEnwmyTowersLeft = false;
    bool noMorePlayerTowersLeft = false;
    bool GameOver = false;
    [Serializable]
    public struct ValueTimePair
    {
        float value;
        float time;

        public ValueTimePair(float value, float time)
        {
            this.value = value;
            this.time = time;
        }
    }
    [SerializeField]
    public Dictionary<int, List<ValueTimePair>> UnitsProduced = new Dictionary<int, List<ValueTimePair>>();


    public static WinConditions Instance { get; private set; }
    LevelMenu levelMenu;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        levelMenu = GameObject.FindObjectOfType<LevelMenu>();
    }

    private void Update()
    {
        if (noMoreEnwmyTowersLeft || noMorePlayerTowersLeft)
        {
            if (AIManager.Instance.Player.numberOfUnits <= 0)
            {
                if (!GameOver)
                    Izgubida();
            }
            else
            {
                bool win = true;
                foreach (AIPlayer ai in AIManager.Instance.AIPlayers)
                {
                    if (ai.numberOfUnits > 0)
                    {
                        win = false;
                    }
                }
                if (win)
                {
                    if (!GameOver)
                        Pobeda();
                }
            }
        }
    }

    public void AddBuildingTeam(Team team)
    {
        buildings.Add(team);
    }

    public void CheckTeams()
    {
        bool value = true;
        foreach (AIPlayer ai in AIManager.Instance.AIPlayers)
        {

            if (ai.buildings.Count > 0)
            {
                value = false;
            }
        }
        noMorePlayerTowersLeft = AIManager.Instance.Player.buildings.Count == 0 ? true : false;
        noMoreEnwmyTowersLeft = value;

    }

    /*bool Player = true, Enemy = true;
    foreach (Team t in buildings)
    {
        if (t.teamid == PlayerTeam)
        {
            Enemy = false;
        }
        else
        {
            Player = false;
        }
    }
    if (Player ^ Enemy)
    {
        if (Player)
        {
            Pobeda();
        }
        else
        {
            Izgubida();
        }
    }*/


    public void AddProducedUnits(float amount, int team)
    {
        List<ValueTimePair> banalno = new List<ValueTimePair>();
        ValueTimePair josBanalnije = new ValueTimePair(amount, LevelMenu.timeSinceStart);
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
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudioClip(2);
            SoundManager.Instance.PlayAudioClip(3);
        }
        levelMenu.WinScreen();
        GameOver = true;
    }

    public void Izgubida()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudioClip(0);
            SoundManager.Instance.PlayAudioClip(1);
        }

        levelMenu.LoseScreen();
        GameOver = true;
    }

}
