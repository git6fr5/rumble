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
    [SerializeField, ReadOnly] Vector3 origin;

    /* --- Unity --- */
    private void Update() {
        CheckFall();
    }

    /* --- Overriden Methods --- */
    public override void Init() {
        base.Init(); // Runs the base initialization.
        
        // Set up the body.
        origin = transform.position;
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
    }

    /* --- Methods --- */
    private void CheckFall() {

        // Draw the line
        Vector3 start = transform.position;
        Vector3 direction = Vector3.down;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start + direction * (hurtbox.size.y + GameRules.MovementPrecision), direction, Mathf.Infinity);
        Debug.DrawLine(start, start + direction * 50f, Color.white, Time.deltaTime);

        // Fall if necessary
        if (hits != null && hits.Length > 0f) {
            for (int i = 0; i < hits.Length; i++) {
                Player player = hits[i].collider.GetComponent<Hurtbox>()?.controller.GetComponent<Player>();
                Animal animal = hits[i].collider.GetComponent<Hurtbox>()?.controller.GetComponent<Animal>();
                if ((animal != null && animal.isControlled) || player != null) {
                    Fall();
                }
            }
        }
    }

    private void Fall() {

        // Edit the body.
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.gravityScale = 1f;
        StartCoroutine(IEReset(resetDelay));

        // Reset after a delay.
        IEnumerator IEReset(float delay) {
            yield return new WaitForSeconds(delay);
            transform.position = origin;
            Init();
            yield return null;
        }
    }


}
