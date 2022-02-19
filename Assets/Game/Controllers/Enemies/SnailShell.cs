/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SnailShell : Controller {

    /* --- Static Variables --- */
    public static float ShakeDuration = 0.5f;
    public static float ShakeMinInterval = 1.75f;
    public static float ShakeMaxInterval = 2.75f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] private bool shake = false;

    /* --- Overridden Methods --- */
    // Runs the initialization logic.
    protected override void Init() {
        base.Init(); // Run the base initialization.

        // Set up the patrol path.
        StartCoroutine(IEShake());

    }

    protected override void Think() {
        base.Think();
        Shake();
    }

    /* --- Methods --- */
    private void Shake() {
        if (shake) {
            Vector3 offset = (Vector3)Random.insideUnitCircle * 0.05f;
            mesh.spriteRenderer.material.SetVector("_Offset", offset);
        }
        else {
            mesh.spriteRenderer.material.SetVector("_Offset", Vector3.zero);
        }
    }

    /* --- Coroutines --- */
    IEnumerator IEShake() {
        while (true) {
            shake = true;
            yield return new WaitForSeconds(ShakeDuration);
            jump = true;
            shake = false;
            yield return new WaitForSeconds(Random.Range(ShakeMinInterval, ShakeMaxInterval));
        }
    }

}
