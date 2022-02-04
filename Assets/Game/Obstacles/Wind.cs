/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/* --- Definitions --- */

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Wind : MonoBehaviour {

    public static float WindFastOnInterval;
    public static float WindFastOffInterval;

    public static float WindMidOnInterval;
    public static float WindMidOffInterval;

    /* --- Components --- */
    public ParticleSystem particles;
        
    /* --- Variables --- */
    protected BoxCollider2D box;

    /* --- Parameters --- */
    [SerializeField] public bool alternate = false;
    [SerializeField] public float onInterval = 1f;
    [SerializeField] public float offInterval = 1f;
    [SerializeField] public Vector2 windDirection;
    [SerializeField] public float windSpeed;
    [SerializeField] protected float rate = 50f;
    [SerializeField] protected float particleSpeedFactor = 20f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] protected List<Controller> container = new List<Controller>();

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();

        if (alternate) {
            StartCoroutine(IEAlternate());
        }

        IEnumerator IEAlternate(){
            while (true) {
                enabled = true;
                particles.gameObject.SetActive(true);
                yield return new WaitForSeconds(onInterval);

                enabled = false;
                particles.gameObject.SetActive(false);
                yield return new WaitForSeconds(offInterval);
            }
        }
    }

    // Runs once every frame.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Push(deltaTime);

        var emission = particles.emission;
        emission.rateOverTime = rate;
        
        var main = particles.main;
        main.startSpeed = windSpeed / particleSpeedFactor;

        // v = d / t => t = d / v
        main.startLifetime = 1f / (0.1f * windSpeed / particleSpeedFactor);

        particles.transform.localScale = new Vector3(1f, 1f, 1f) * 0.1f;
        var shape = particles.shape;
        float y_angle = Vector2.SignedAngle(windDirection, Vector2.up);
        shape.rotation = new Vector3(0f, y_angle, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Controller controller = collider.GetComponent<Hurtbox>()?.controller;
        if (controller != null && !container.Contains(controller)) {
            container.Add(controller);
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        Controller controller = collider.GetComponent<Hurtbox>()?.controller;
        if (controller != null && container.Contains(controller)) {
            container.Remove(controller);
        }
    }

    /* --- Virtual Methods --- */
    // Runs the initialization logic.
    protected virtual void Init() {
        box = GetComponent<BoxCollider2D>();
        // box.size = new Vector2(endPoint.localPosition.x, 1f);
        //box.offset = new Vector2((endPoint.localPosition.x - 1f) / 2f, 0f);
        box.isTrigger = true;
    }

    // Pushes the stuff in the container.
    protected virtual void Push(float deltaTime) {
        // 
        for (int i = 0; i < container.Count; i++) {
            if (container[i].body != null) {
                container[i].body.AddForce(windDirection.normalized * windSpeed);
            }
            // container[i].SetMass(0.5f, 2 * deltaTime);
            // container[i].transform.position += (Vector3)windDirection.normalized * windMagnitude * deltaTime;
        }
    }

}
