using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Production))]
[RequireComponent(typeof(UnitController))]
public class UnitDetector : MonoBehaviour
{
    public float checkRadious = 1f;

    Team team;
    Production production;
    UnitController controller;

    void Start()
    {
        team = GetComponent<Team>();
        production = GetComponent<Production>();
        controller = GetComponent<UnitController>();
    }

    void Update()
    {
        Collider[] contextColliders = Physics.OverlapSphere(transform.position, checkRadious, LayerMask.GetMask("unit"));
        foreach (Collider unit in contextColliders)
        {
            UnitAgent unitAgent = unit.GetComponent<UnitAgent>();

            if (unitAgent.Controller != controller)
            {
                if (unitAgent.selfTeam != team.teamid && !unitAgent.isGift) //is enemy and is not gift
                {
                    if (team.teamid >= 2)//znaci samo ai timove
                    {
                        AIManager.Instance.AIPlayers[team.teamid - 2].currentEnemyTeam = unitAgent.selfTeam;//novi neprijatelj je napadac
                    }
                    team.Damage(unitAgent);
                    UnitPool.Instance.ReurnUnitsToPool(unitAgent);
                }
                else
                {

                    if (unitAgent.TrackPositions.Count <= 1)
                    {
                        if (unitAgent.TrackPositions.Peek() == transform)
                        {
                            team.Reinforce();
                            UnitPool.Instance.ReurnUnitsToPool(unitAgent);
                        }
                    }
                    else
                    {
                        if (unitAgent.TrackPositions.Peek() == transform)
                        {
                            unitAgent.GoToNext();
                        }
                    }

                }
            }
        }
    }

    //List<Transform> GetNearbyUnits()
    //{
    //    List<Transform> context = new List<Transform>();
    //    Collider[] contextColliders = Physics.OverlapSphere(transform.position, checkRadious, LayerMask.GetMask("unit"));
    //    foreach (Collider coll in contextColliders)
    //    {
    //        if (coll.name.ToString()[0] != team.teamid)
    //        {
    //            context.Add(coll.transform);
    //        }
    //    }
    //    return context;
    //}

}
