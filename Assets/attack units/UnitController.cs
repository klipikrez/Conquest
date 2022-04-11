using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Production))]
[RequireComponent(typeof(Team))]
public class UnitController : MonoBehaviour
{
    public bool checkNeighbours = true;
    public List<UnitController> neighbours;
    public float neighbourCheckRadious = 5f;
    public List<UnitAgent> agents = new List<UnitAgent>();
    public UnitBehaivour unitSteerBehaivour;
    public BuildingBehaviorCompiler UnitBuildingSpawnBehavior;
    [System.NonSerialized]
    public Production production;
    public Team team { get; private set; }
    LineRenderer line;

    public int maxDispatchRate = 5;
    public float timeBetweenDispatches = 0.5f;

    public float unitAcceleration = 10;
    public float unitMaxSPeed = 5;
    public float drag = 0.9f;
    public float updateUnitEvery = 0.1f;

    public float neighbourRadious = 1.5f;
    [Range(0, 1)]
    public float avoidenceRadiousMultiplyer = 0.5f;//max je 1 min je 0
    //public float moveUpdateTimeInterval = 0.05f;
    //float timer = 100;

    float squaredMaxSpeed;
    float squaredNeighbourRadious;
    float squaredAvoidenceRadious;
    float avoidenceRadious;
    public float SuaredAvoidenceRadious { get { return squaredAvoidenceRadious; } }
    public float AvoidenceRadious { get { return avoidenceRadious; } }

    Coroutine attackCoroutine;
    Coroutine lineCoroutine;

    public Mesh CylinderMeshGizmo;

    [System.NonSerialized]
    public bool Paused = false;

    // Start is called before the first frame update
    void Start()
    {
        if (UnitPool.Instance == null)
        {
            Debug.LogError("BUDALO GLUPA, klipice, klipikce, nemas UnitPool.cs nigde :)");
        }
        squaredMaxSpeed = unitMaxSPeed * unitMaxSPeed;
        squaredNeighbourRadious = neighbourRadious * neighbourRadious;
        squaredAvoidenceRadious = squaredNeighbourRadious * avoidenceRadiousMultiplyer * avoidenceRadiousMultiplyer;
        avoidenceRadious = neighbourRadious * avoidenceRadious;
        production = GetComponent<Production>();
        //team = GetComponent<Team>();
        GetTeam();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        //line = gameObject.AddComponent<LineRenderer>();


        if (checkNeighbours)
        {
            CheckNeighbours();
        }

        //CylinderMeshGizmo = (Mesh)Resources.Load("Levels/Cylinder");

    }

    public Team GetTeam()
    {
        if (team == null)
        {
            team = GetComponent<Team>();
        }
        return team;
    }

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;



        //if (timer > moveUpdateTimeInterval)
        //{
        if (!Paused)
        {

            for (int i = 0; i < agents.Count; i++)
            {
                UnitAgent agent = agents[i];
                agent.currentMoveDirection /= (1 + drag * Time.deltaTime);//drag
                agent.Move(agent.currentMoveDirection);
            }
            //}
        }
        //if (timer > moveUpdateTimeInterval)
        //{
        //timer = 0;
        //}
    }

    public void UpdateAgent(UnitAgent agent) // UnitAgent.cs \\ line 52
    {
        if (!Paused)
        {
            /*for (int i = 0; i < agents.Count; i++)
            {
                UnitAgent agent = agents[i];*/
            List<Transform> context = GetNearbyObjects(agent);
            context.Insert(0, agent.TrackPositions.Peek()); //insert na prvo mesto liste track transform

            Vector2 move = unitSteerBehaivour.CalculateMove(agent, context, this);
            move *= unitAcceleration;

            agent.currentMoveDirection += move;//acceleration

            if (agent.currentMoveDirection.sqrMagnitude > squaredMaxSpeed)//max speed
            {
                agent.currentMoveDirection = agent.currentMoveDirection.normalized * unitMaxSPeed;
            }
            //}
        }
    }

    List<Transform> GetNearbyObjects(UnitAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadious, LayerMask.GetMask("unit"));
        foreach (Collider coll in contextColliders)
        {
            if (coll != agent.AgentCollider)
            {
                context.Add(coll.transform);
            }
        }
        return context;
    }

    void CheckNeighbours()
    {
        foreach (UnitController neighbour in neighbours)
        {
            if (!neighbour.neighbours.Contains(this))
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

    public void Attack(int percent, Transform attack, bool dalDaSeVidiOnaLinijaKadSaljesLikoveIzmedjuSmajli = true)
    {

        int amount = (int)(production.product * (percent / 100f));

        if (amount >= 1)
        {
            StopAttackUnits();
            Transform[] path = CalculatePath(transform, attack, dalDaSeVidiOnaLinijaKadSaljesLikoveIzmedjuSmajli);
            if (path != null) attackCoroutine = StartCoroutine(SpawnAttackUnits(amount, path, team.teamid));
        }
    }

    public void ContinuousAttack(Transform attack)
    {
        StopAttackUnits();
        Transform[] path = CalculatePath(transform, attack);
        if (path != null) attackCoroutine = StartCoroutine(SpawnContinuousAttackUnits(path, team.teamid));
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

    Transform[] CalculatePath(Transform from, Transform to, bool dalDaSeVidiOnaLinijaKadSaljesLikoveIzmedjuSmajli = true)
    {
        if (NavManager.Instance != null)
        {
            Transform[] path = NavManager.Instance.CalculatePath(from, to);
            if (path != null)
            {
                CalculateLine(path, from, dalDaSeVidiOnaLinijaKadSaljesLikoveIzmedjuSmajli);
            }
            return path;
        }
        else { Debug.LogError("Alo BRE GLUPALO, nemas navmanagera :)"); }

        return null;
    }

    void CalculateLine(Transform[] points, Transform start, bool dalDaSeVidiOnaLinijaKadSaljesLikoveIzmedjuSmajli)
    {
        if (dalDaSeVidiOnaLinijaKadSaljesLikoveIzmedjuSmajli)
        {
            Vector3[] positions = new Vector3[points.Length + 1];
            positions[0] = start.position;
            for (int i = 0; i < points.Length; i++)
            {
                positions[i + 1] = points[i].position;
            }
            line.enabled = true;
            line.SetVertexCount(points.Length + 1);
            line.SetPositions(positions);

            if (lineCoroutine != null)
            {
                StopCoroutine(lineCoroutine);
            }

            lineCoroutine = StartCoroutine(DisableLine(line));
        }
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
                if (production.product - 1 <= 0 || amount < 0)
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
            if (production.product - 1f >= maxDispatchRate)
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

    IEnumerator SpawnGiftUnits(int amount, Transform[] attack, int teamid)
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

    void InitializeUnit(UnitAgent newAgent, Transform[] path, int teamid, bool isGift = false)
    {
        Vector2 spawnZone = Random.insideUnitCircle * 0.2f;

        newAgent.transform.position = new Vector3(spawnZone.x, 0, spawnZone.y) + transform.position;
        newAgent.gameObject.SetActive(true);

        UnitBuildingSpawnBehavior.InitializeUnit(newAgent, path, this, teamid, isGift, updateUnitEvery);
        //newAgent.name = teamid.ToString()+ (isGift ? "g" : "n") + " - " + newAgent.id;
        //newAgent.AddPath(path);
        //newAgent.selfTeam = teamid;
        //newAgent.selfCol = team.colors[teamid];
        //newAgent.isGift = isGift;
        //newAgent.currentMoveDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        //newAgent.Initialize(this);
    }

    private void OnDrawGizmos()
    {


        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Vector4(0.5f, 0, 1f, 0.3f);
        Gizmos.DrawMesh(CylinderMeshGizmo, transform.position + new Vector3(0, 0.2f, 0), Quaternion.Euler(Vector3.right * -90), 100 * new Vector3(neighbourCheckRadious, neighbourCheckRadious, 1));

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, neighbourCheckRadious);
    }

}
