/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    /* --- Components --- */
    [SerializeField] public Hearthbox hearth = null; // The key used to jump.
    [SerializeField] public Spirit spirit = null; // The key used to jump.

    /* --- Parameters --- */

    /* --- Properties --- */
    [SerializeField, ReadOnly] int coins = 0;
    [SerializeField, ReadOnly] private KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] private KeyCode actionKey = KeyCode.J; // The key used to perform the action.

    /* --- Overridden Methods --- */
    protected override void Init() {
        base.Init();
        // GameRules.ResetLevel();
    }

    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.
        
        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Get the jump.
        if (Input.GetKeyDown(jumpKey)) {
            jump = true;
        }
        if (Input.GetKey(jumpKey) && airborneFlag == Airborne.Rising) {
            weight *= floatiness;
        }

        // Get the action.
        if (Input.GetKeyDown(actionKey)) {
            action = true;
        }

        // This needs to be set to the level node.
        //if (transform.position.sqrMagnitude > GameRules.BoundLimit * GameRules.BoundLimit) {
        //    GameRules.ResetLevel();
        //}
        if (body.velocity.y < -200f) {
            GameRules.ResetLevel();
        }
    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.

        actionFlag = ActionState.Action;

        Spiritbox spiritbox = null;

        Vector3 direction = directionFlag == Direction.Right ? Vector3.right : Vector3.left;
        float distance = 1f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + direction * distance, 0.325f);
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].GetComponent<Spiritbox>()!= null) {
                spiritbox = hits[i].GetComponent<Spiritbox>();
                break;
            }
        }

        StartCoroutine(IEAction(direction, distance, spiritbox, (float)mesh.action.Length / GameRules.FrameRate));

    }

    IEnumerator IEAction(Vector3 direction, float distance, Spiritbox spiritbox, float delay) {

        float speed = (distance / delay);
        // print(speed);
        moveDirection = direction.x;
        moveSpeed = speed;
        think = false;

        yield return new WaitForSeconds(delay * 2f / 3f);

        if (spiritbox != null) {
            spiritbox.Possess(this);
        }

        yield return new WaitForSeconds(delay * 1f / 3f);

        if (gameObject.activeSelf) {
            think = true;
            actionFlag = ActionState.None;
        }

        yield return null;
    }

    public void Dismount() {

        Vector2 dismountVector = Vector2.up;
        body.velocity = Vector2.zero;
        weight = 0f;
        think = false;
        spirit = null;
        actionFlag = ActionState.PreAction;
        StartCoroutine(IEDismount(0.2f));

        IEnumerator IEDismount(float delay) {
            body.velocity = dismountVector * 17.5f;
            int afterImages = 6;
            for (int i = 0; i < afterImages; i++) {
                SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                // afterImage.transform.SetParent(transform);
                afterImage.transform.position = mesh.transform.position;
                afterImage.transform.localRotation = mesh.transform.localRotation;
                afterImage.transform.localScale = mesh.transform.localScale;
                afterImage.sprite = mesh.spriteRenderer.sprite;
                afterImage.color = Color.white * 0.5f;
                Destroy(afterImage.gameObject, delay);
                yield return new WaitForSeconds(delay / (float)afterImages);
            }
            think = true;
            actionFlag = ActionState.None;
            yield return null;
        }

    }

    public void CollectCoin() {
        coins += 1;
    }

}
