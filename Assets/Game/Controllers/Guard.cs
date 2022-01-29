/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Guard : Controller {

    /* --- Properties --- */
    [Space(2), Header("Patrol")]
    [SerializeField] public Transform[] patrolPoints; // The paths between which the guard patrols.
    [SerializeField, ReadOnly] private int patrolIndex; // The current point in the path the guard is moving towards.
    [Space(2), Header("Vision")]
    [SerializeField] private Transform cone = null; // The cone transform.
    [SerializeField, ReadOnly] private Vector3 coneScale = Vector3.zero; // The scale of the cone.
    [SerializeField] private float visionAngle = 22.5f; // The angular width of the vision cone.
    [SerializeField] private float visionDistance = 3f; // The radius of the vision cone.
    [Space(2), Header("Pausing")]
    [SerializeField] private float pauseDuration = 1f; // The duration for which the guard pauses at a patrol point.
    [SerializeField, ReadOnly] private float pauseTicks = 0f; // Tracks how long a guard has been paused at a patrol point.

    /* --- Overridden Methods --- */
    // Runs the initialization logic.
    protected override void Init() {
        base.Init(); // Run the base initialization.

        // Set up the patrol path.
        patrolIndex = 0;
        for (int i = 0; i < patrolPoints.Length; i++) {
            patrolPoints[i].transform.SetParent(null);
        }

        // Set up the vision cone.
        coneScale = cone.localScale;
    }

    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        // Perform the action.
        action = true;

        // Process whether this guard is paused.
        if (pauseTicks > 0f) {
            pauseTicks -= Time.deltaTime;
            return;
        }
        else {
            pauseTicks = 0f;
        }
        
        // Move the guard alond its patrol path.
        float targetDirection = patrolPoints[patrolIndex].transform.position.x - transform.position.x;
        if (Mathf.Abs(targetDirection) < GameRules.MovementPrecision) {
            pauseTicks = pauseDuration;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
        else {
            moveDirection = Mathf.Sign(targetDirection);
        }

    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.

        // Adjust the scale of the cone.
        cone.localScale = new Vector3(directionFlag == Direction.Right ? coneScale.x : -coneScale.x, coneScale.y, coneScale.z);
        
        // Get the player position.
        Vector3 playerPosition = GameRules.MainPlayer.transform.position;
        Vector3 playerDisplacement = playerPosition - transform.position;

        // Get the angle and distance to the player.
        Vector3 axis = directionFlag == Direction.Right ? Vector3.right : Vector3.left;
        float angleToPlayer = Vector2.SignedAngle(axis, (Vector2)playerDisplacement);
        float distanceToPlayer = playerDisplacement.magnitude;

        // Check whether the player was seen.
        bool withinAngle = angleToPlayer < visionAngle && angleToPlayer > -visionAngle;
        bool withinDistance = distanceToPlayer < visionDistance;
        if (withinDistance && withinAngle) {
            GameRules.GameOver();
        }

    }

    /* --- Debug --- */
    // Runs once every draw call.
    public void OnDrawGizmos() {
        // Draw the patrol points.
        Gizmos.color = Color.red;
        if (patrolPoints != null) {
            for (int i = 0; i < patrolPoints.Length; i++) {
                Gizmos.DrawWireSphere(patrolPoints[i].position, 0.1f);
            }
        }

        // Draw the vision cone.
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        Vector3 axis = directionFlag == Direction.Right ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 0f, visionAngle) * axis * visionDistance);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 0f, -visionAngle) * axis * visionDistance);
        Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, visionDistance);

    }

}
