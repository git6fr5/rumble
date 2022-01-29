/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Penguin : Controller {

    /* --- Parameters --- */
    [SerializeField, Range(5, 20)] public float slideWeight;

    /* --- Properties --- */
    [SerializeField, ReadOnly] public float momentumDirection;
    [SerializeField, ReadOnly] private KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] private KeyCode actionKey = KeyCode.J; // The key used to perform the action.

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal"); // != 0 ? Input.GetAxisRaw("Horizontal") : momentumDirection;

        // Get the jump.
        if (Input.GetKeyDown(jumpKey)) {
            jump = true;
        }
        if (Input.GetKey(jumpKey) && airborneFlag == Airborne.Rising) {
            weight *= floatiness;
        }

        // Get the action.
        if (Input.GetKey(actionKey)) {
            action = true;
            momentumDirection = moveDirection;
        }
        // Time.timeScale = 1f;

    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.

        // Time.timeScale = 0.5f;
        moveDirection = momentumDirection;
        if (!mesh.feetbox.empty) {
            if (body.velocity.y < GameRules.MovementPrecision) {
                weight *= slideWeight;
            }
            else if (body.velocity.y > GameRules.MovementPrecision) {
                weight *= floatiness;
            }
            else {
                weight = 0f;
            }
        }

    }

}
