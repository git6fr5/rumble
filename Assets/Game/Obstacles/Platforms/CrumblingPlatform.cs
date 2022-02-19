/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 
/// </summary>
public class CrumblingPlatform : MovingPlatform {

    /* --- Static Variables --- */
    public static float CrumbleDelay = 1f;
    public static float ResetDelay = 3f;

    /* --- Parameters --- */

    /* --- Properties --- */
    [SerializeField, ReadOnly] private bool crumbling = false;

    /* --- Unity --- */
    private void OnTriggerEnter2D(Collider2D collider) {
        ProcessCollision(collider);
    }


    // Sets the target for this platform.
    protected override void Effect() {
        if (crumbling) {
            Vector3 offset = (Vector3)Random.insideUnitCircle * 0.05f;
            spriteShapeRenderer.materials[1].SetVector("_Offset", offset);
            spriteShapeRenderer.materials[1].SetColor("_AddColor", Color.red * 0.25f);
        }
        else {
            spriteShapeRenderer.materials[1].SetVector("_Offset", Vector4.zero);
            spriteShapeRenderer.materials[1].SetColor("_AddColor", Color.white * 0f);
        }
    }

    /* --- Methods --- */
    private void ProcessCollision(Collider2D collider) {

        // Check for feet.
        Feetbox feetbox = collider.GetComponent<Feetbox>();
        if (feetbox != null) {
            Crumble();
        }

    }

    private void Crumble() {

        // Edit the body.
        crumbling = true;
        StartCoroutine(IECrumble(CrumbleDelay, ResetDelay));

        // Reset after a delay.
        IEnumerator IECrumble(float delayA, float delayB) {
            yield return new WaitForSeconds(delayA);
            spriteShapeRenderer.enabled = false;
            box.enabled = false;
            yield return new WaitForSeconds(delayB);
            crumbling = false;
            spriteShapeRenderer.enabled = true;
            box.enabled = true;
            yield return null;
        }

    }

}
