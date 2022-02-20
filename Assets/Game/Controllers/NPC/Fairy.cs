/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Fairy : MonoBehaviour {

    [SerializeField] public float speed;
    [SerializeField] public float radius;
    [SerializeField] public float pathInterval = 3f;

    [SerializeField, ReadOnly] private Vector3 origin;
    [SerializeField, ReadOnly] private Vector3 target;
    [SerializeField, ReadOnly] private float ticks;

    // Runs once before the first frame.
    void Start() {
        origin = transform.position;
        StartCoroutine(IEPath());
    }

    // Runs once every frame.
    void Update() {
        Vector3 direction = (target - origin).normalized * speed + Mathf.Sin(ticks) * (Quaternion.Euler(0f, 0f, 90f) * (target - origin).normalized) * speed;
        Vector3 velocity = direction * Time.deltaTime;
        transform.position += velocity;
        transform.localRotation = (target - origin).normalized.x > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        ticks += Time.deltaTime;
    }

    private IEnumerator IEPath() {
        while (true) {
            ticks = 0f;
            target = origin + radius * (Vector3)Random.insideUnitCircle;
            yield return new WaitForSeconds(pathInterval);
        }
    }

}
