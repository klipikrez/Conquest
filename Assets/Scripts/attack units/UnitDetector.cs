using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Production))]
[RequireComponent(typeof(UnitController))]
public class UnitDetector : MonoBehaviour
{
    public float checkRadious = 1f;
    public bool Engage = true;
    public bool Imune = false;

    public VisualEffect buildingCrumble;
    [System.NonSerialized]
    public BuildingMain building;


    void Update()
    {
        Collider[] contextColliders = Physics.OverlapSphere(transform.position, checkRadious, LayerMask.GetMask("unit"));
        foreach (Collider unit in contextColliders)
        {
            UnitAgent unitAgent = unit.GetComponent<UnitAgent>();
            if (Engage)
            {
                if (unitAgent.Controller != building.unitController)
                {
                    if (unitAgent.selfTeam != building.team.teamid && !unitAgent.isGift) //is enemy and is not gift
                    {
                        if (!Imune)
                        {
                            if (AIManager.Instance != null && building.team.teamid >= 2)//znaci samo ai timove
                            {
                                AIManager.Instance.AIPlayers[building.team.teamid - 2].currentEnemyTeam = unitAgent.selfTeam;//novi neprijatelj je napadac
                            }
                            building.team.Damage(unitAgent);
                            UnitPool.Instance.ReurnUnitsToPool(unitAgent);
                            buildingCrumble.SendEvent("burst");
                            if (SoundManager.Instance != null)
                            {
                                SoundManager.Instance.PlayTowerSound(transform.position);
                            }
                        }
                        else
                        {
                            UnitPool.Instance.ReurnUnitsToPool(unitAgent);
                        }
                    }
                    else
                    {

                        if (unitAgent.TrackPositions.Count <= 1)
                        {
                            if (unitAgent.TrackPositions.Peek() == transform)
                            {
                                building.team.Reinforce();
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
            else
            {
                if (unitAgent.TrackPositions.Peek() == transform)
                {
                    unitAgent.GoToNext();
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
