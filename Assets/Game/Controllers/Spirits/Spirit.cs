/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */

/// <summary>
/// 
/// </summary>
public class Spirit : Controller {

    /* --- Components --- */
    public MeshSprite baseMeshSprite;
    public MeshSprite possessMeshSprite;

    /* --- Parameters --- */
    [SerializeField] public Transform[] patrolPoints; // The paths between which the guard patrols.
    [SerializeField] private float pauseDuration = 1f; // The duration for which the guard pauses at a patrol point.

    /* --- Properties --- */
    [SerializeField, ReadOnly] private int patrolIndex; // The current point in the path the guard is moving towards.
    [SerializeField, ReadOnly] private float pauseTicks = 0f; // Tracks how long a guard has been paused at a patrol point.
    [SerializeField, ReadOnly] protected KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] protected KeyCode dismountKey = KeyCode.K; // The key used to dismount.
    [SerializeField, ReadOnly] protected KeyCode actionKey = KeyCode.J; // The key used to perform the action.
    [SerializeField] public bool isControlled = false;
    [SerializeField] public Controller possessor = null;

    /* --- Overridden Methods --- */
    // Runs the initialization logic.
    protected override void Init() {
        base.Init(); // Run the base initialization.

        // Set up the patrol path.
        patrolIndex = 0;
        for (int i = 0; i < patrolPoints.Length; i++) {
            patrolPoints[i].transform.SetParent(null);
        }

    }

    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        if (isControlled) {
            Control();
        }
        else {
            // AI();
        }

    }

    /* --- Methods --- */
    private void AI() {

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

    private void Control() {

        // Checks 
        ControlCheck();
        ActionCheck();

        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Get the jump.
        if (Input.GetKeyDown(jumpKey)) {
            jump = true;
        }
        if (Input.GetKey(jumpKey) && airborneFlag == Airborne.Rising) {
            weight *= floatiness;
        }
        if (Input.GetKey(dismountKey)) {
            Dismount();
        }

        // 
        //if (transform.position.sqrMagnitude > GameRules.BoundLimit * GameRules.BoundLimit) {
        //    GameRules.ResetLevel();
        //}
        if (body.velocity.y < -200f) {
            GameRules.ResetLevel();
        }
    }

    public void Possess(Controller possessor) {
        isControlled = true;
        this.possessor = possessor;
        if (possessor.GetComponent<Player>() != null) {
            possessor.GetComponent<Player>().spirit = this;
        }
        possessor.transform.parent = transform;
        possessor.transform.localPosition = Vector2.zero;
        possessor.gameObject.SetActive(false);

        possessMeshSprite.gameObject.SetActive(true);
        possessMeshSprite.Organize();
    }

    public void Dismount() {

        isControlled = false;
        if (possessor == null) {
            return;
        }

        baseMeshSprite.gameObject.SetActive(true);
        baseMeshSprite.Organize();

        possessor.gameObject.SetActive(true);
        possessor.transform.SetParent(null);
        Player player = possessor.GetComponent<Player>();
        if (player) {
            player.Dismount();
        }
        possessor = null;

        gameObject.SetActive(false);

    }

    protected virtual void ActionCheck() {

    }

    protected virtual void ControlCheck() {

    }

}
