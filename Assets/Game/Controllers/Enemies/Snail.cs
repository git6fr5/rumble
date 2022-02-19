using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Controller {

    // bool falling;


    /* --- Methods --- */
    private void CheckFall() {

        //if (!GameRules.OnScreen(transform.position)) {
        //    return;
        //}

        //if (falling) {
        //    return;
        //}

        //// Draw the line
        //Vector3 start = transform.position;
        //Vector3 direction = Vector3.down;
        //RaycastHit2D[] hits = Physics2D.RaycastAll(start + direction * (hurtbox.size.y + GameRules.MovementPrecision), direction, Mathf.Infinity);

        //// Fall if necessary
        //bool playerInLineOfSight = false;
        //float distanceToPlayer = Mathf.Infinity;
        //float distanceToGround = Mathf.Infinity;
        //if (hits != null && hits.Length > 0f) {
        //    for (int i = 0; i < hits.Length; i++) {

        //        if (hits[i].collider == hurtbox) {
        //            i += 1;
        //            if (i > hits.Length - 1) {
        //                break;
        //            }
        //        }

        //        float distanceToThing = (transform.position - (Vector3)hits[i].point).magnitude;
        //        if (hits[i].collider.tag == GameRules.GroundTag && distanceToThing < distanceToGround) {
        //            distanceToGround = distanceToThing;
        //            print(distanceToGround);
        //        }

        //        Player player = hits[i].collider.GetComponent<Hurtbox>()?.controller.GetComponent<Player>();
        //        Spirit spirit = hits[i].collider.GetComponent<Hurtbox>()?.controller.GetComponent<Spirit>();
        //        if ((spirit != null && spirit.isControlled) || player != null) {
        //            playerInLineOfSight = true;
        //            distanceToPlayer = distanceToThing;
        //        }

        //    }
        //}

        //Debug.DrawLine(start, start + direction * distanceToGround, Color.white, Time.deltaTime);

        //if (playerInLineOfSight && distanceToPlayer < distanceToGround) {
        //    Fall();
        //}

    }

    private void Fall() {

        // Edit the body.
        //falling = true;
        //body.constraints = RigidbodyConstraints2D.FreezeRotation;
        //body.gravityScale = 1f;
        //StartCoroutine(IEReset(resetDelay));

        //// Reset after a delay.
        //IEnumerator IEReset(float delay) {
        //    yield return new WaitForSeconds(delay);
        //    transform.localPosition = origin;
        //    Init();
        //    yield return null;
        //}
    }

    private void Shatter() {
        // spriteRenderer.enabled = false;
    }

}
