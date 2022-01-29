/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Hurtbox : Container {

    /* --- Parameters --- */
    [SerializeField] private string enemy = "";

    /* --- Properties --- */
    [SerializeField] public Controller controller = null;

    // Initializes the script.
    protected override void Init() {
        base.Init(); // Runs the base initialization.
        target = enemy;

        Transform uppermostParent = transform;
        while (uppermostParent.parent != null) {
            uppermostParent = uppermostParent.parent;
        }
        tag = uppermostParent.tag;
        // Assumes this to be true.
        // controller = uppermostParent.GetComponent<Controller>();
    }

    /* --- Overridden Events --- */
    public override void OnAdd(Collider2D collider) {
        Hurtbox collidedHurtbox = collider.GetComponent<Hurtbox>();
        Controller collidedController = collidedHurtbox.controller;
        if (collidedController != null) {
            int direction = (int)Mathf.Sign(collidedController.transform.position.x - transform.position.x);
            collidedController.Knockback(direction, controller.baseWeight);
            print("Knocking back " + collidedController.name);
        }
    }

    public override void OnRemove(Collider2D collider) {

    }

}
