/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteShapeController))]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour {

    /* --- Variables --- */
    private SpriteShapeController shape;
    private BoxCollider2D box;

    /* --- Parameters --- */
    [SerializeField] protected Transform endPoint = null;
    [SerializeField] private float speed = 0f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] protected Vector3 target = Vector3.zero;
    [SerializeField, ReadOnly] protected List<Controller> container = new List<Controller>();

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        Target();
    }

    // Runs once every frame.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Move(deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Controller controller = collision.gameObject.GetComponent<Controller>();
        if (controller != null && !container.Contains(controller)) {
            container.Add(controller);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        Controller controller = collision.gameObject.GetComponent<Controller>();
        if (controller != null && container.Contains(controller)) {
            container.Remove(controller);
        }
    }

    /* --- Virtual Methods --- */
    // Runs the initialization logic.
    protected virtual void Init() {

        shape = GetComponent<SpriteShapeController>();

        shape.spline.Clear();
        shape.spline.InsertPointAt(0, new Vector3(-.5f, 0f, 0f));
        shape.spline.InsertPointAt(1, endPoint.localPosition + new Vector3(-.5f, 0f, 0f));
        shape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        shape.spline.SetTangentMode(1, ShapeTangentMode.Continuous);

        box = GetComponent<BoxCollider2D>();
        box.size = new Vector2(endPoint.localPosition.x, 1f);
        box.offset = new Vector2((endPoint.localPosition.x - 1f) / 2f, 0f);

        target = transform.position;
    }

    // Sets the target for this platform.
    protected virtual void Target() {
        // 
    }

    /* --- Methods --- */
    // Moves this platform.
    private void Move(float deltaTime) {
        Vector3 velocity = (target - transform.position).normalized * speed;
        transform.position += velocity * deltaTime;
        for (int i = 0; i < container.Count; i++) {
            container[i].transform.position += velocity * deltaTime;
        }
    }

}
