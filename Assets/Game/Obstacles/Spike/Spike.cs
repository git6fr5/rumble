using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Spike : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected BoxCollider2D hurtbox;

    /* --- Unity --- */
    private void Start() {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        ProcessCollision(collider);
    }

    private static void ProcessCollision(Collider2D collider) {
        // Check for the player or an animal.
        Player player = collider.GetComponent<Hurtbox>()?.controller.GetComponent<Player>();
        Animal animal = collider.GetComponent<Hurtbox>()?.controller.GetComponent<Animal>();

        // If a player or a player controlled animal.
        if ((animal != null && animal.isControlled) || player != null) {
            GameRules.ResetLevel();
        }
        // If just an animal.
        if (animal != null && !animal.isControlled) {
            //
        }
    }

    /* --- Virtual Methods --- */
    public virtual void Init() {
        // Set up the collision.
        hurtbox = GetComponent<BoxCollider2D>();
        hurtbox.isTrigger = true;
    }

}
