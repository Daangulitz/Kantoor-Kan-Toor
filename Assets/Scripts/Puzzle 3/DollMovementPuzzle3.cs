using System.Collections;
using UnityEngine;

public class RandomCrawler : MonoBehaviour
{
    enum State { Crawling, Held, Settling, Stopped }

    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float arriveDistance = 0.05f;
    [SerializeField] private LayerMask obstacleMask;          // optional: walls/furniture the doll should not cross (leave on Nothing to ignore)

    [Header("Ground")]
    [SerializeField] private LayerMask groundMask;            // layer(s) that count as "the ground" (NOT the doll's own layer)
    [SerializeField] private float groundOffset = 0.05f;      // height of the doll's pivot above the floor
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private float maxGroundAngle = 40f;      // steeper surfaces do not count as ground

    [Header("After grabbing")]
    [SerializeField] private float settleTime = 0.5f;         // how long it must lie still before the ground check
    [SerializeField] private float maxSettleWait = 6f;
    [SerializeField] private bool stopForeverIfNotOnGround = true;   // false = it can be grabbed again and resume when put on the ground

    [Header("Crawling rotation")]
    [SerializeField] private float lockedAngleZ = 90f;        // Z is always forced to this value while crawling
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 90f;             // turns the head towards the destination (try 90, -90, 180, 0)
    [SerializeField] private float turnSpeed = 5f;

    private Rigidbody rb;
    private State state = State.Crawling;
    private Vector3 center;
    private Vector3 target;
    private bool hasTarget;
    private float waitTimer;
    private float currentX, currentY;
    private bool dead;                                        // stopped for good

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        center = transform.position;
        currentX = 0f;
        currentY = transform.eulerAngles.y;

        if (rb != null) rb.isKinematic = true;   // we move it ourselves while crawling
        PickTarget();
    }

    // ---- Hook these up in the XR Grab Interactable: Interactable Events ----
    // Select Entered -> OnGrabbed()      Select Exited -> OnReleased()
    public void OnGrabbed()
    {
        if (dead) return;
        StopAllCoroutines();
        state = State.Held;
    }

    public void OnReleased()
    {
        if (dead) return;
        StopAllCoroutines();
        StartCoroutine(SettleRoutine());
    }

    IEnumerator SettleRoutine()
    {
        state = State.Settling;

        // Wait 2 frames so the grab system has finished restoring the Rigidbody, then let it fall
        yield return null;
        yield return null;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Wait until the doll lies still
        Vector3 last = transform.position;
        float still = 0f, total = 0f;
        while (still < settleTime && total < maxSettleWait)
        {
            yield return null;
            total += Time.deltaTime;

            if ((transform.position - last).sqrMagnitude < 0.000001f) still += Time.deltaTime;
            else still = 0f;
            last = transform.position;
        }

        if (IsOnGround())
        {
            // Back on the ground: crawl again from here
            if (rb != null) rb.isKinematic = true;
            center = transform.position;
            currentX = 0f;
            currentY = transform.eulerAngles.y;
            waitTimer = waitTime;
            hasTarget = false;
            state = State.Crawling;
            PickTarget();
        }
        else
        {
            // Not on the ground: stay where it is
            state = State.Stopped;
            if (stopForeverIfNotOnGround) dead = true;
        }
    }

    bool IsOnGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.2f, groundMask, QueryTriggerInteraction.Ignore))
            return Vector3.Angle(hit.normal, Vector3.up) <= maxGroundAngle;
        return false;
    }

    void Update()
    {
        if (state != State.Crawling) return;

        if (!hasTarget)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) PickTarget();
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 pos = transform.position;
        Vector3 flatTarget = new Vector3(target.x, pos.y, target.z);
        Vector3 next = Vector3.MoveTowards(pos, flatTarget, speed * Time.deltaTime);

        // Stay on the floor
        if (Physics.Raycast(next + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1.5f, groundMask, QueryTriggerInteraction.Ignore))
            next.y = hit.point.y + groundOffset;

        transform.position = next;
        RotateTowardsTarget();

        Vector3 diff = target - next;
        diff.y = 0f;
        if (diff.magnitude <= arriveDistance)
        {
            hasTarget = false;
            waitTimer = waitTime;
        }
    }

    void LateUpdate()
    {
        // Force Z only while crawling, so we never fight the grab
        if (state == State.Crawling) ApplyRotation();
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Vector3 look = Quaternion.LookRotation(dir.normalized, Vector3.up).eulerAngles;
        currentX = Mathf.LerpAngle(currentX, look.x + xOffset, turnSpeed * Time.deltaTime);
        currentY = Mathf.LerpAngle(currentY, look.y + yOffset, turnSpeed * Time.deltaTime);
    }

    void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(currentX, currentY, lockedAngleZ);
    }

    void PickTarget()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 circle = Random.insideUnitCircle * wanderRadius;
            Vector3 p = center + new Vector3(circle.x, 0f, circle.y);

            // Find the floor below that point
            if (!Physics.Raycast(p + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 6f, groundMask, QueryTriggerInteraction.Ignore))
                continue;
            if (Vector3.Angle(hit.normal, Vector3.up) > maxGroundAngle)
                continue;

            Vector3 candidate = hit.point + Vector3.up * groundOffset;

            // Skip targets behind obstacles
            if (Physics.Linecast(transform.position + Vector3.up * 0.1f, candidate + Vector3.up * 0.1f,
                                 obstacleMask, QueryTriggerInteraction.Ignore))
                continue;

            target = candidate;
            hasTarget = true;
            return;
        }

        // Nothing valid found: try again after a short wait
        hasTarget = false;
        waitTimer = waitTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 c = Application.isPlaying ? center : transform.position;
        Gizmos.DrawWireSphere(c, wanderRadius);
        if (Application.isPlaying && hasTarget)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target);
        }
    }
}