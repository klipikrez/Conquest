using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Production))]
[RequireComponent(typeof(Team))]
public class UnitController : MonoBehaviour
{

    public List<UnitController> neighbours;
    public float neighbourCheckRadious = 5f;

    public UnitAgent unitAgentPrefab;
    public List<UnitAgent> agents = new List<UnitAgent>();
    public UnitBehaivour behaivour;
    Production production;
    public Team team { get; private set; }
    LineRenderer line;

    public int maxDispatchRate = 5;
    public float timeBetweenDispatches = 0.5f;

    public float unitAcceleration = 10;
    public float unitMaxSPeed = 5;
    public float drag = 0.9f;

    public float neighbourRadious = 1.5f;
    [Range(0, 1)]
    public float avoidenceRadiousMultiplyer = 0.5f;//max je 1 min je 0

    float squaredMaxSpeed;
    float squaredNeighbourRadious;
    float squaredAvoidenceRadious;
    public float SuaredAvoidenceRadious { get { return squaredAvoidenceRadious; } }

    Coroutine attackCoroutine;
    Coroutine lineCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        squaredMaxSpeed = unitMaxSPeed * unitMaxSPeed;
        squaredNeighbourRadious = neighbourRadious * neighbourRadious;
        squaredAvoidenceRadious = squaredNeighbourRadious * avoidenceRadiousMultiplyer * avoidenceRadiousMultiplyer;
        production = GetComponent<Production>();
        team = GetComponent<Team>();
        line = gameObject.AddComponent<LineRenderer>();

        CheckNeighbours();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0;i<agents.Count; i++)
        {
            UnitAgent agent = agents[i];

            List<Transform> context = GetNearbyObjects(agent);
            context.Insert(0, agent.TrackPositions.Peek()); //insert na prvo mesto liste track transform

            Vector2 move = behaivour.CalculateMove(agent, context, this);
            move *= unitAcceleration;

            //agents[i].currentMoveDirection *= drag;//drag

            agent.currentMoveDirection += move;//acceleration
            
            if (agent.currentMoveDirection.sqrMagnitude > squaredMaxSpeed)//max speed
            {
                agent.currentMoveDirection = agent.currentMoveDirection.normalized * unitMaxSPeed;
            }

            agent.Move(agent.currentMoveDirection);
        }
    }

    List<Transform> GetNearbyObjects(UnitAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadious, LayerMask.GetMask("unit"));
        foreach(Collider coll in contextColliders)
        {
            if(coll != agent.AgentCollider)
            {
                context.Add(coll.transform);              
            }
        }
        return context;
    }

    void CheckNeighbours()
    {
        foreach(UnitController neighbour in neighbours)
        {
            if(!neighbour.neighbours.Contains(this))
            {
                neighbour.neighbours.Add(this);
            }
        }
    }

    public void StopAttackUnits()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    public void Attack(int percent, Transform attack)
    {

        int amount = (int)(production.product * (percent / 100f));

        if (amount >= 1)
        {
            StopAttackUnits();
            Transform[] path = CalculatePath(transform, attack);
            if (path != null) attackCoroutine = StartCoroutine(SpawnAttackUnits(amount, path, team.teamid));
        }
    }

    public void ContinuousAttack(Transform attack)
    {
        StopAttackUnits();
        Transform[] path = CalculatePath(transform, attack);
        if(path != null) attackCoroutine = StartCoroutine(SpawnContinuousAttackUnits(path, team.teamid));
    }

    public void Gift(int percent, BuildingMain gift)
    {
        int amount = (int)(production.product * (percent / 100f));

        if (amount >= 1)
        {
            StopAttackUnits();
            Transform[] path = CalculatePath(transform, gift.transform);
            if (path != null) attackCoroutine = StartCoroutine(SpawnGiftUnits(amount, path, gift.team.teamid));
        }
    }

    Transform[] CalculatePath(Transform from,Transform to)
    {

        Transform[] path = NavManager.Instance.CalculatePath(from, to);
        
        CalculateLine(path,from);

        return path;
    }

    void CalculateLine(Transform[] points,Transform start)
    {
        Vector3[] positions = new Vector3[points.Length+1];
        positions[0] = start.position;
        for (int i = 0; i < points.Length; i++)
        {
            positions[i+1] = points[i].position;
        }
        line.enabled = true;
        line.SetVertexCount(points.Length+1);
        line.SetPositions(positions);

        if (lineCoroutine != null)
        {
            StopCoroutine(lineCoroutine);
        }

        lineCoroutine = StartCoroutine(DisableLine(line));
    }

    IEnumerator DisableLine(LineRenderer line)
    {
        yield return new WaitForSeconds(1);
        line.enabled = false;
    }

    IEnumerator SpawnAttackUnits(int amount, Transform[] attack, int teamid)
    {
        while (true)
        {
            
            for (int i = 0; i < maxDispatchRate; i++)
            {
                if (production.product-1 <= 0 || amount < 0)
                {
                    yield break;
                }

                production.product--;
                amount--;

                UnitAgent newAgent = UnitPool.Instance.Get();
                InitializeUnit(newAgent, attack, teamid);
                agents.Add(newAgent);
            }
            yield return new WaitForSeconds(timeBetweenDispatches); //cekaj
        }
    }

    IEnumerator SpawnContinuousAttackUnits(Transform[] attack, int teamid)
    {
        while (true)
        {
            if (production.product > maxDispatchRate)
            {
                for (int i = 0; i < maxDispatchRate; i++)
                {

                    production.product--;

                    UnitAgent newAgent = UnitPool.Instance.Get();
                    InitializeUnit(newAgent, attack, teamid);
                    agents.Add(newAgent);
                }
            }
            yield return new WaitForSeconds(timeBetweenDispatches); //cekaj
        }
    }

    IEnumerator SpawnGiftUnits(int amount,Transform[] attack, int teamid)
    {
        while (true)
        {

            for (int i = 0; i < maxDispatchRate; i++)
            {
                if (production.product - 1 <= 0 || amount < 0)
                {
                    yield break;
                }

                production.product--;
                amount--;

                UnitAgent newAgent = UnitPool.Instance.Get();
                InitializeUnit(newAgent, attack, teamid, true);
                agents.Add(newAgent);
            }
            yield return new WaitForSeconds(timeBetweenDispatches); //cekaj
        }
    }

    void InitializeUnit(UnitAgent newAgent,Transform[] path, int teamid, bool isGift = false)
    {
        Vector2 spawnZone = Random.insideUnitCircle * 0.2f;

        newAgent.transform.position = new Vector3(spawnZone.x, 0.1f, spawnZone.y) + transform.position;
        newAgent.gameObject.SetActive(true);

        newAgent.name = teamid.ToString()+ (isGift ? "g" : "n") + " - " + newAgent.id;
        newAgent.AddPath(path);
        newAgent.selfTeam = teamid;
        newAgent.selfCol = team.colors[teamid];
        newAgent.isGift = isGift;
        newAgent.currentMoveDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        newAgent.Initialize(this);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, neighbourCheckRadious);
    }

}
