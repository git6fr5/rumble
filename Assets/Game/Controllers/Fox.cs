/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using ActionState = Controller.ActionState;

/// <summary>
/// 
/// </summary>
public class Fox : Animal {

    /* --- Parameters --- */
    [Space(2), Header("Action")]
    [SerializeField] [Range(0f, 40f)] protected float dashSpeed = 20f;
    [SerializeField] [Range(0f, 0.5f)] protected float dashDuration = 0.1f;
    [HideInInspector] protected Coroutine dashTimer = null;

    /* --- Properties --- */
    [SerializeField, ReadOnly] public bool canDash;

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void ActionCheck() {
        base.ActionCheck(); // Runs the base think.

        pauseEnergy = false;

        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Check whether the fox can dash.
        // If the fox has touched ground, and it is not currently dashing
        // Then the fox is able to dash.
        // Note: After touching ground, the fox resets its ability to dash,
        // Which means it can still do so even after leaving the ground.
        if (!mesh.feetbox.empty && dashTimer == null) {
            canDash = true;
            actionFlag = ActionState.None;
        }
        else {
            pauseEnergy = true;
        }

        if (dashTimer != null) {
            pauseEnergy = true;

            Vector3 offset = (Vector3)Random.insideUnitCircle * 0.05f;
            mesh.spriteRenderer.material.SetVector("_Offset", offset);
            mesh.spriteRenderer.material.SetColor("_AddColor", Color.blue);
        }
        else {
            mesh.spriteRenderer.material.SetVector("_Offset", Vector3.zero);
            mesh.spriteRenderer.material.SetColor("_AddColor", new Color(0f, 0f, 0f, 0f));
        }

        // Check the input.
        if (Input.GetKeyDown(actionKey)) {
            action = true;
        }

    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.
        if (canDash) {
            Dash();
            canDash = false;
            pauseEnergy = true;
            energyTicks += energy * (1f / (float)maxActionCount);
        }

    }

    private void Dash() {

        Vector2 dashVector = new Vector2(Input.GetAxisRaw("Horizontal"), 0f).normalized;
        body.velocity = Vector2.zero;
        weight = 0f;
        think = false;
        dashTimer = StartCoroutine(IEDash(dashDuration));
        actionFlag = ActionState.PreAction;

        IEnumerator IEDash(float delay) {
            yield return new WaitForSeconds(0.15f);
            body.velocity = dashVector * dashSpeed;
            actionFlag = ActionState.Action;
            int afterImages = 2;
            for (int i = 0; i < afterImages; i++) {
                SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                // afterImage.transform.SetParent(transform);
                afterImage.transform.position = mesh.transform.position;
                afterImage.transform.localRotation = mesh.transform.localRotation;
                afterImage.transform.localScale = mesh.transform.localScale;
                afterImage.sprite = mesh.spriteRenderer.sprite;
                afterImage.color = Color.white * 0.5f;
                Destroy(afterImage.gameObject, delay);
                yield return new WaitForSeconds(delay / (float)afterImages);
            }
            think = true;
            actionFlag = ActionState.None;
            yield return (dashTimer = null);
        }

    }
}
