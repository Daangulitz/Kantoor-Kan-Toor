using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RandomNavMover : MonoBehaviour
{
    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private bool aroundStartPosition = true;

    [Header("Crawling rotation")]
    [SerializeField] private float lockedAngleZ = 90f;   // Z is always forced to this value
    [SerializeField] private float xOffset = 0f;         // correction for X (tilt)
    [SerializeField] private float yOffset = 90f;        // correction for Y: turns the head towards the destination (try 90, -90, 180, 0)
    [SerializeField] private float turnSpeed = 5f;

    private NavMeshAgent agent;
    private Vector3 startPos;
    private float waitTimer;
    private float currentX;
    private float currentY;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // we handle rotation ourselves
    }

    void Start()
    {
        startPos = transform.position;
        currentX = transform.eulerAngles.x;
        currentY = transform.eulerAngles.y;
        ApplyRotation();
        GoToRandomPoint();
    }

    void Update()
    {
        RotateTowardsDestination();

        if (agent.pathPending) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;

        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTime)
        {
            waitTimer = 0f;
            GoToRandomPoint();
        }
    }

    void LateUpdate()
    {
        // Force Z every frame, in case anything else changed the rotation
        ApplyRotation();
    }

    void RotateTowardsDestination()
    {
        // Next point on the path (use agent.destination to look straight at the end point instead)
        Vector3 dir = agent.steeringTarget - transform.position;

        if (dir.sqrMagnitude < 0.001f || agent.velocity.sqrMagnitude < 0.01f) return;

        Vector3 look = Quaternion.LookRotation(dir.normalized, Vector3.up).eulerAngles;

        currentX = Mathf.LerpAngle(currentX, look.x + xOffset, turnSpeed * Time.deltaTime);
        currentY = Mathf.LerpAngle(currentY, look.y + yOffset, turnSpeed * Time.deltaTime);
    }

    void ApplyRotation()
    {
        // X and Y follow the destination, Z is always locked
        transform.rotation = Quaternion.Euler(currentX, currentY, lockedAngleZ);
    }

    void GoToRandomPoint()
    {
        Vector3 center = aroundStartPosition ? startPos : transform.position;

        for (int i = 0; i < 30; i++)
        {
            Vector2 circle = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = center + new Vector3(circle.x, 0f, circle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }

        Debug.LogWarning("No NavMesh point found. Is the NavMesh baked near " + center + "?");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 c = Application.isPlaying && aroundStartPosition ? startPos : transform.position;
        Gizmos.DrawWireSphere(c, wanderRadius);
    }
}