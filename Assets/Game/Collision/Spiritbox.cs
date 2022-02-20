/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Spiritbox : MonoBehaviour {

    /* --- Components --- */
    [SerializeField] public Spirit spirit;
    [HideInInspector] protected BoxCollider2D box;

    /* --- Unity --- */
    private void Start() {
        Init();
    }

    /* --- Virtual Methods --- */
    public virtual void Init() {
        // Set up the collision.
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;

    }

    public void Possess(Controller possessor) {
        spirit.Possess(possessor);
    }

}
