/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Controlbox : Container {

    /* --- Static Variables --- */
    private static string ControllableTag = "Controllable";

    /* --- Parameters --- */
    [SerializeField] private bool controllable;

    /* --- Properties --- */
    [SerializeField] public Controller controller = null;

    // Initializes the script.
    protected override void Init() {
        base.Init(); // Runs the base initialization.
        target = controllable ? "" : ControllableTag;
        tag = ControllableTag;

        Transform uppermostParent = transform;
        while (uppermostParent.parent != null) {
            uppermostParent = uppermostParent.parent;
        }
        // Assumes this to be true.
        // controller = uppermostParent.GetComponent<Controller>();

        print("Initialized");
    }

    /* --- Overridden Events --- */
    public override void OnAdd(Collider2D collider) {
        Controlbox collidedControlbox = collider.GetComponent<Controlbox>();
        Animal collidedController = collidedControlbox.controller.GetComponent<Animal>();
        if (collidedController != null) {
            collidedController.isControlled = true;
            collidedController.controllingObject = controller.gameObject;
            controller.transform.parent = collidedController.transform;
            controller.transform.localPosition = Vector2.zero;
            controller.gameObject.SetActive(false);
        }
    }

    public override void OnRemove(Collider2D collider) {

    }

}
