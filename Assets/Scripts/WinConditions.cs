using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditions : MonoBehaviour
{

    List<Team> buildings = new List<Team>();//added in Buildings Team.cs scripts Start()
    public int PlayerTeam = 1;
    public float totalUnitsProduced = 0;


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
    public void AddBuildingTeam(Team team)
    {
        buildings.Add(team);
    }

    public void CheckTeams()
    {
        bool Player = true, Enemy = true;
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
        }
    }

    public void AddPlayerProducedUnits(float amount)
    {
        totalUnitsProduced += amount;
    }

    public void Pobeda()
    {
        levelMenu.WinScreen();

    }

    public void Izgubida()
    {
        levelMenu.LoseScreen();
    }

}
