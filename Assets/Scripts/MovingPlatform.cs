using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waitTime = 1f;

    private int currentWaypointIndex = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    private void Awake()
    {
        SetupPlatformEffector();
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length <= 1)
            return;

        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                isWaiting = false;
                waitCounter = 0f;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        if (targetWaypoint == null)
        {
            Debug.LogError("Waypoint is missing!");
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            isWaiting = true;
        }
    }

    private void SetupPlatformEffector()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.usedByEffector = true;
        box.isTrigger = false;

        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        effector.useOneWay = true;
        effector.useOneWayGrouping = true;
        effector.surfaceArc = 180f;
        effector.rotationalOffset = 0f;
    }
}