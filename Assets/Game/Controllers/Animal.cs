/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Animal : Controller {

    /* --- Parameters --- */
    [SerializeField] public Transform[] patrolPoints; // The paths between which the guard patrols.
    [SerializeField] private float pauseDuration = 1f; // The duration for which the guard pauses at a patrol point.
    [SerializeField] protected float energy = 1f; // The duration for which this can be rode.
    [SerializeField] protected int maxActionCount = 3; // The duration for which this can be rode

    /* --- Properties --- */
    [SerializeField, ReadOnly] protected bool pauseEnergy = false; // The duration for which this can be rode.
    [SerializeField, ReadOnly] protected float energyTicks = 1f; // The duration for which this can be rode.
    [SerializeField, ReadOnly] protected float energyTickBuffer = 0.25f; // The duration for which this can be rode.
    [SerializeField, ReadOnly] private int patrolIndex; // The current point in the path the guard is moving towards.
    [SerializeField, ReadOnly] private float pauseTicks = 0f; // Tracks how long a guard has been paused at a patrol point.
    [SerializeField, ReadOnly] protected KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] protected KeyCode dismountKey = KeyCode.K; // The key used to dismount.
    [SerializeField, ReadOnly] protected KeyCode actionKey = KeyCode.J; // The key used to perform the action.
    [SerializeField] public bool isControlled = false;
    [SerializeField] public GameObject controllingObject = null;

    /* --- Overridden Methods --- */
    // Runs the initialization logic.
    protected override void Init() {
        base.Init(); // Run the base initialization.

        // Set up the patrol path.
        patrolIndex = 0;
        for (int i = 0; i < patrolPoints.Length; i++) {
            patrolPoints[i].transform.SetParent(null);
        }

        // Set up the energy.
        energyTicks = 0f;

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
        // controllingObject.transform.position = transform.position;
        ActionCheck();
        ControlSettings();
        bool hasEnergy = Energy();
        if (!hasEnergy) {
            return;
        }

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
        if (transform.position.sqrMagnitude > GameRules.BoundLimit * GameRules.BoundLimit) {
            GameRules.ResetLevel();
        }

    }

    protected bool Energy() {
        bool hasEnergy = true;
        if (!pauseEnergy) {
            energyTicks += Time.deltaTime;
            if (energyTicks > energy) {
                hasEnergy = false;
                if (energyTicks > energy + energyTickBuffer) {
                    Dismount();
                }
            }
        }
        return hasEnergy;
    }

    protected virtual void ControlSettings() {
        // moveSpeed *= strengthFactor;
    }

    private void Dismount() {
        isControlled = false;
        if (controllingObject != null) {
            controllingObject.SetActive(true);
            controllingObject.transform.SetParent(null);
            controllingObject.transform.position = transform.position + 1.5f * Vector3.up;
            Controller controller = controllingObject.GetComponent<Controller>();
            controller.think = true;
            controller.body.velocity = new Vector2(0f, 5f) + body.velocity;
            controllingObject = null;
        }
        gameObject.SetActive(false);
    }

    protected virtual void ActionCheck() {

    }
}
