using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Hearthbox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected CircleCollider2D box;
    [HideInInspector] protected SpriteRenderer spriteRenderer;
    public Level level;

    /* --- Properties --- */
    public bool discovered = false;

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
        box = GetComponent<CircleCollider2D>();
        box.isTrigger = true;
        discovered = false;
    }

    public void Init(Level level) {
        // Set up the collision.
        this.level = level;
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
            player.hearth = this;
            discovered = true;
        }

        if (spirit != null && spirit.isControlled) {
            Player possessor = spirit.possessor.GetComponent<Player>();
            if (possessor != null) {
                possessor.hearth = this;
                discovered = true;
            }
        }

    }

}
