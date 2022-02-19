using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSnail : Snail {

    /* --- Components --- */

    /* --- Parameters --- */
    [SerializeField, Range(1.5f, 10f)] private float resetDelay = 3f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] private Vector3 origin;
    [SerializeField, ReadOnly] private bool falling;

    /* --- Overriden Methods --- */
    protected override void Init() {
        base.Init(); // Runs the base initialization.

        // Set up the body.
        origin = transform.localPosition;
        falling = false;
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
    }

    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.
        CheckFall();
    }

    /* --- Methods --- */
    private void Fall() {

        // Edit the body.
        falling = true;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.gravityScale = 1f;
        StartCoroutine(IEReset(resetDelay));

        // Reset after a delay.
        IEnumerator IEReset(float delay) {
            yield return new WaitForSeconds(delay);
            transform.localPosition = origin;
            Init();
            yield return null;
        }
    }

    private void CheckFall() {

        //if (!GameRules.OnScreen(transform.position)) {
        //    return;
        //}

        if (falling) {
            return;
        }

        float maxDistance = 100f;

        // Draw the line
        Vector3 start = transform.position;
        Vector3 direction = Vector3.down;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start + direction * (1f + GameRules.MovementPrecision), direction, maxDistance);

        // Fall if necessary
        bool playerInLineOfSight = false;
        float distanceToPlayer = maxDistance;
        float distanceToGround = maxDistance;
        if (hits != null && hits.Length > 0f) {
            for (int i = 0; i < hits.Length; i++) {

                float distanceToThing = (transform.position - (Vector3)hits[i].point).magnitude;
                if (hits[i].collider.tag == GameRules.GroundTag && distanceToThing < distanceToGround) {
                    distanceToGround = distanceToThing;
                    // print(distanceToGround);
                }

                Controller controller = hits[i].collider.GetComponent<Hurtbox>()?.controller;
                Player player = controller?.GetComponent<Player>();
                Spirit spirit = controller?.GetComponent<Spirit>();

                if ((player != null || spirit != null && spirit.isControlled)) {
                    playerInLineOfSight = true;
                    distanceToPlayer = distanceToThing;
                }

            }
        }

        Debug.DrawLine(start, start + direction * distanceToGround, Color.white, Time.deltaTime);
        // print(distanceToGround);

        if (playerInLineOfSight && distanceToPlayer < distanceToGround) {
            Fall();
        }

    }

}
