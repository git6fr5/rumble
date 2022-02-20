using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Shellbox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected BoxCollider2D box;
    [HideInInspector] protected SpriteRenderer spriteRenderer;

    /* --- Unity --- */
    private void Start() {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        ProcessCollision(collider);
    }

    /* --- Virtual Methods --- */
    public virtual void Init() {
        // Set up the collision.
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    protected virtual void ProcessCollision(Collider2D collider) {

        // Check for the player or an animal.
        Controller controller = collider.GetComponent<Hurtbox>()?.controller;
        if (controller == null) {
            return;
        }
        Player player = controller.GetComponent<Player>();
        Spirit spirit = controller.GetComponent<Spirit>();

        // If a player or a player controlled animal.
        if (player != null) {
            GameRules.ResetLevel();
        }

        if (spirit != null) {
            print("Spirit");
        }
        if (spirit != null && spirit.isControlled) {
            spirit.Dismount();
        }

    }

}
