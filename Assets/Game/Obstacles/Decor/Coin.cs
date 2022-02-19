using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Coin : MonoBehaviour {

    private CircleCollider2D box;
    public Vector2 bobDirection;
    public float bobSpeed;
    // public Level level;

    // Start is called before the first frame update
    void Start() {

        box = GetComponent<CircleCollider2D>();
        box.isTrigger = true;

        StartCoroutine(IEBob());

        IEnumerator IEBob() {
            // yield return new WaitForSeconds(Random.Range(0f, 1f));
            while (true) {
                bobDirection *= -1f;
                yield return new WaitForSeconds(1.5f);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        transform.position += (Vector3)bobDirection.normalized * Time.deltaTime * bobSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        ProcessCollision(collider);
    }

    protected virtual void ProcessCollision(Collider2D collider) {

        // Check for the player or an animal.
        Controller controller = collider.GetComponent<Hurtbox>()?.controller;
        if (controller == null) {
            return;
        }
        Player player = controller.GetComponent<Player>();
        Spirit spirit = controller.GetComponent<Spirit>();

        // If a player or a player controlled animal.
        if (player != null) {
            player.CollectCoin();
            Destroy(gameObject);
        }

        if (spirit != null && spirit.isControlled) {
            Player possessor = spirit.possessor.GetComponent<Player>();
            if (possessor != null) {
                player.CollectCoin();
                Destroy(gameObject);
            }
        }

    }

}
