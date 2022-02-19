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
        spirit.isControlled = true;
        spirit.possessor = possessor;
        if (possessor.GetComponent<Player>() != null) {
            possessor.GetComponent<Player>().spirit = spirit;
        }
        possessor.transform.parent = spirit.transform;
        possessor.transform.localPosition = Vector2.zero;
        possessor.gameObject.SetActive(false);
    }

}
