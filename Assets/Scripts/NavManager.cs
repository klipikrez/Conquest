using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavManager : MonoBehaviour
{

    GameObject[] buildings;
    IDictionary<GameObject, UnitController> buildingControllers = new Dictionary<GameObject, UnitController>();

    public static NavManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update    building
    void Start()
    {
        buildings = GameObject.FindGameObjectsWithTag("building");
        foreach (GameObject building in buildings)
        {
            UnitController controller = building.GetComponent<UnitController>();
            buildingControllers.Add(building, controller);

            SetBuildingNeighbours(controller);
        }
    }

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

    }

    public void SetPauseBuildings(bool val)
    {
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
            Debug.LogError("start and end of path are same");
            return null;
        }

        //search pool
        HashSet<Transform> unexplored = new HashSet<Transform>();

        //shortest path from start to current building
        IDictionary<UnitController, float> distances = new Dictionary<UnitController, float>();

        //parent paths
        IDictionary<Transform, Transform> parents = new Dictionary<Transform, Transform>();

        //initialize
        foreach (GameObject building in buildings)
        {
            distances.Add(new KeyValuePair<UnitController, float>(buildingControllers[building], float.MaxValue));
            parents.Add(new KeyValuePair<Transform, Transform>(building.transform, null));
            unexplored.Add(building.transform);
        }

        UnitController startController = start.GetComponent<UnitController>();

        //start distance = 0
        distances[startController] = 0;

        while (unexplored.Count > 0)
        {
            UnitController check = distances//check this
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

            IList<UnitController> neighbours = check.neighbours;

            foreach (UnitController neighbour in neighbours)
            {
                //distance cost
                float dist = distances[check]
                    + Vector3.Distance(check.transform.position, neighbour.transform.position)  //distance
                    * check.unitMaxSPeed                                                        //max speed
                    * ((check.team.teamid != startController.team.teamid) ? 100 : 1);           //if not same team X100 distance
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
