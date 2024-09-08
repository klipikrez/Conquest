using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class NavManager : MonoBehaviour
{

    GameObject[] buildings;
    [SerializedDictionary("obj", "main")]
    public SerializedDictionary<GameObject, BuildingMain> buildingMains = new SerializedDictionary<GameObject, BuildingMain>();

    public static NavManager Instance { get; private set; }
    public bool autoInicialize = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (autoInicialize) Inicialize(GameObject.FindGameObjectsWithTag("building"));
    }

    // Start is called before the first frame update    building
    public void Inicialize(GameObject[] inBuildings)
    {
        buildings = inBuildings;
        foreach (GameObject building in buildings)
        {
            BuildingMain controller = building.GetComponent<BuildingMain>();
            if (controller != null) buildingMains.Add(building, controller);

            //SetBuildingNeighbours(controller);
        }
    }/*

    void SetBuildingNeighbours(UnitController building)
    {

        Collider[] neighbourColliders = Physics.OverlapSphere(building.transform.position, building.neighbourCheckRadious, LayerMask.GetMask("building"));
        List<GameObject> neighbours = new List<GameObject>();
        foreach (Collider neighbourCollider in neighbourColliders)
        {
            UnitController neighbourController = neighbourCollider.GetComponent<UnitController>();

            if (neighbourCollider.gameObject != building.gameObject && !building.neighbours.Contains(neighbourController))
            {
                building.neighbours.Add(neighbourController);
            }
        }

    }*/

    public void SetPauseBuildings(bool val)
    {/*
        foreach (GameObject building in buildings)
        {
            buildingControllers[building].Paused = val;
            if (buildingControllers[building].production == null)
            {
                buildingControllers[building].GetComponent<Production>().Paused = val;

            }
            else
            {

                buildingControllers[building].production.Paused = val;

            }
        }*/


        foreach (var building in buildingMains)
        {
            building.Value.unitController.Paused = val;

            building.Value.production.Paused = val;
        }
    }



    public Transform[] CalculatePath(Transform start, Transform end)
    {
        if (start == null || end == null)
        {
            Debug.LogError("missing transform to calculate path");
            return null;
        }
        else if (start == end)
        {
            Debug.LogError("start and end of path are same: " + start + " - " + end);
            return null;
        }

        //search pool
        HashSet<Transform> unexplored = new HashSet<Transform>();

        //shortest path from start to current building
        IDictionary<BuildingMain, float> distances = new Dictionary<BuildingMain, float>();

        //parent paths
        IDictionary<Transform, Transform> parents = new Dictionary<Transform, Transform>();

        //initialize
        foreach (GameObject building in buildings)
        {
            distances.Add(new KeyValuePair<BuildingMain, float>(buildingMains[building], float.MaxValue));
            parents.Add(new KeyValuePair<Transform, Transform>(building.transform, null));
            unexplored.Add(building.transform);
        }

        BuildingMain startBuilding = start.GetComponent<BuildingMain>();

        //start distance = 0
        distances[startBuilding] = 0;

        while (unexplored.Count > 0)
        {
            BuildingMain check = distances//check this
                .Where(x => unexplored.Contains(x.Key.transform))//is in unexplored
                .OrderBy(x => x.Value).First().Key;//order by smallest number first

            if (check.transform == end) //done checking
            {
                if (parents[end] != null) //check if valid path found
                {
                    List<Transform> path = new List<Transform>();
                    Transform current = end;

                    //make list of path transforms in reverse(from end to start)
                    while (current != start)
                    {
                        path.Add(current);
                        current = parents[current];
                    }

                    path.Reverse();
                    return path.ToArray();
                }
                else
                {
                    return null;
                }
            }

            unexplored.Remove(check.transform);

            IList<BuildingMain> neighbours = check.neighbours;

            foreach (BuildingMain neighbour in neighbours)
            {
                //distance cost
                float dist = distances[check]
                    + Vector3.Distance(check.transform.position, neighbour.transform.position)  //distance
                    * check.unitController.unitMaxSPeed                                                        //max speed
                    * ((check.team.teamid != startBuilding.team.teamid) ? 100 : 1);           //if not same team X100 distance
                                                                                              //burn fire hot
                                                                                              //update parent if better total distance is found
                if (dist < distances[neighbour])
                {
                    distances[neighbour] = dist;
                    parents[neighbour.transform] = check.transform;
                }

            }

        }
        return null;
    }

}
