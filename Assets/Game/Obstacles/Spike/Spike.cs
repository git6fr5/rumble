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
        hurtbox = GetComponent<BoxCollider2D>();
        hurtbox.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();

        CheckAttachToPlatform();
    }

    protected virtual void ProcessCollision(Collider2D collider) {

        // Check for the player or an animal.
        Player player = collider.GetComponent<Hurtbox>()?.controller.GetComponent<Player>();
        Animal animal = collider.GetComponent<Hurtbox>()?.controller.GetComponent<Animal>();

        // If a player or a player controlled animal.
        if ((animal != null && animal.isControlled) || player != null) {
            GameRules.ResetLevel();
        }
        // If just an animal.
        if (animal != null && !animal.isControlled) {
            // Destroy(animal.gameObject);
        }

    }

    protected virtual void CheckAttachToPlatform() {
        Vector3 direction = Vector3.down;
        CheckAttachToPlatform(direction);
    }

    protected void CheckAttachToPlatform(Vector3 direction) {
        Vector3 start = transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start + direction * (hurtbox.size.y + GameRules.MovementPrecision), direction, 1f);
        Debug.DrawLine(start, start + direction * 1f, Color.white, 3f);

        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].collider.GetComponent<Platform>()) {
                transform.SetParent(hits[i].collider.transform);
            }
        }
    }

}
