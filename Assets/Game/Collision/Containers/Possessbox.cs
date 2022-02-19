/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Possessbox : MonoBehaviour {

    /* --- Components --- */
    [SerializeField] public Controller possessor;
    [HideInInspector] protected BoxCollider2D box;

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
        Spiritbox spiritbox = collider.GetComponent<Spiritbox>();
        if (spiritbox != null) {
            spiritbox.Possess(possessor);
            return;
        }

    }
}
