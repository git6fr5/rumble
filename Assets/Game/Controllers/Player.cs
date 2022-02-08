/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    /* --- Parameters --- */

    /* --- Properties --- */
    [SerializeField, ReadOnly] private KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] private KeyCode actionKey = KeyCode.J; // The key used to perform the action.

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.
        
        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Get the jump.
        if (Input.GetKeyDown(jumpKey)) {
            jump = true;
        }
        if (Input.GetKey(jumpKey) && airborneFlag == Airborne.Rising) {
            weight *= floatiness;
        }

        // Get the action.
        if (Input.GetKeyDown(actionKey)) {
            action = true;
        }

        // 
        if (transform.position.sqrMagnitude > GameRules.BoundLimit * GameRules.BoundLimit) {
            GameRules.ResetLevel();
        }
    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.
    }

    public void Dismount() {

        Vector2 dismountVector = Vector2.up;
        body.velocity = Vector2.zero;
        weight = 0f;
        think = false;
        StartCoroutine(IEDismount(0.2f));

        IEnumerator IEDismount(float delay) {
            body.velocity = dismountVector * 17.5f;
            int afterImages = 6;
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
            yield return null;
        }

    }

}
