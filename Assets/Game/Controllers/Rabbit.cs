/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Rabbit : Animal {

    /* --- Parameters --- */
    [Space(2), Header("Action")]
    [SerializeField, Range(0f, 100f)] protected float maxHop = 20f;
    [SerializeField, Range(0f, 3f)] protected float maxCharge = 0.5f;
    [SerializeField, Range(0f, 0.25f)] protected float hopDelay = 0.05f;
    [SerializeField, Range(0f, 20f)] protected float hopAirSpeed  = 15f;


    /* --- Properties --- */
    [SerializeField, ReadOnly] public float charge;
    [SerializeField, ReadOnly] public bool canHop;
    [SerializeField, ReadOnly] protected bool justHopped = false;
    [HideInInspector] protected Coroutine hopTimer = null;

    /* --- Overridden Methods --- */
    protected override void ActionCheck() {
        base.ActionCheck();

        pauseEnergy = false;

        // Check that we're on the ground.
        if (!mesh.feetbox.empty && !justHopped) {
            canHop = true;
        }
        else {
            pauseEnergy = true;
        }

        // Check if the action is currently being performed.
        if (Input.GetKey(actionKey) && canHop) {
            charge += Time.deltaTime;
            moveSpeed = 0f;
            if (charge > maxCharge) {
                charge = maxCharge;
            }
            pauseEnergy = true;
        }

        // Check the input.
        if (Input.GetKeyUp(actionKey)) {
            action = true;
            justHopped = true;
            pauseEnergy = true;
        }
        
        mesh.spriteRenderer.material.SetColor("_AddColor", Color.red * charge / maxCharge);

        Vector3 offset = (Vector3)Random.insideUnitCircle * 0.05f;
        mesh.spriteRenderer.material.SetVector("_Offset", offset * charge / maxCharge);
        // GameRules.CameraShake(0.05f * charge / maxCharge, 0.05f);

    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.
        if (canHop) {
            Hop();
            canHop = false;
            pauseEnergy = true;
            energyTicks += energy * (1f / (float)maxActionCount);
        }
    }

    private void Hop() {
        body.velocity = new Vector2(body.velocity.x, body.velocity.y + maxHop * Mathf.Sqrt(charge / maxCharge));
        charge = 0f;
        justHopped = true;
        hopTimer = StartCoroutine(IEHop(hopDelay));

        IEnumerator IEHop(float delay) {
            yield return new WaitForSeconds(delay);
            justHopped = false;
            yield return (hopTimer = null);
        }
    }

    protected override void ControlSettings() {
        //
        if (!canHop) {
            moveSpeed = hopAirSpeed;
        }
    }

}
