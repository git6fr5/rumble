using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingSpike : Spike {

    /* --- Components --- */
    [HideInInspector] private Rigidbody2D body;

    /* --- Parameters --- */
    [SerializeField, Range(1.5f, 10f)] private float resetDelay = 3f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] private Vector3 origin;
    [SerializeField, ReadOnly] private bool falling;

    /* --- Unity --- */
    private void Update() {
        CheckFall();
    }

    /* --- Overriden Methods --- */
    public override void Init() {
        base.Init(); // Runs the base initialization.
        
        // Set up the body.
        origin = transform.localPosition;
        falling = false;
        spriteRenderer.enabled = true;
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
    }

    protected override void ProcessCollision(Collider2D collider) {
        base.ProcessCollision(collider);
        if (falling && collider.tag == GameRules.GroundTag) {
            Shatter();
        }
    }

    protected override void CheckAttachToPlatform() {
        Vector3 direction = Vector3.up;
        CheckAttachToPlatform(direction);
    }

    /* --- Methods --- */
    private void CheckFall() {

        if (falling) {
            return;
        }

        // Draw the line
        Vector3 start = transform.position;
        Vector3 direction = Vector3.down;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start + direction * (hurtbox.size.y + GameRules.MovementPrecision), direction, Mathf.Infinity);
        Debug.DrawLine(start, start + direction * 50f, Color.white, Time.deltaTime);

        // Fall if necessary
        bool playerInLineOfSight = false;
        float distanceToPlayer = Mathf.Infinity;
        float distanceToGround = Mathf.Infinity;
        if (hits != null && hits.Length > 0f) {
            for (int i = 0; i < hits.Length; i++) {

                if (hits[i].collider == hurtbox) {
                    i += 1;
                    if (i > hits.Length - 1) {
                        break;
                    }
                }

                float distanceToThing = (transform.position - hits[i].collider.transform.position).sqrMagnitude;
                if (hits[i].collider.tag == GameRules.GroundTag && distanceToThing < distanceToGround) {
                    distanceToGround = distanceToThing;
                }

                Player player = hits[i].collider.GetComponent<Hurtbox>()?.controller.GetComponent<Player>();
                Animal animal = hits[i].collider.GetComponent<Hurtbox>()?.controller.GetComponent<Animal>();
                if ((animal != null && animal.isControlled) || player != null) {
                    playerInLineOfSight = true;
                    distanceToPlayer = distanceToThing;
                }
                
            }
        }

        if (playerInLineOfSight && distanceToPlayer < distanceToGround) {
            Fall();
        }

    }

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

    private void Shatter() {
        // spriteRenderer.enabled = false;
    }


}
