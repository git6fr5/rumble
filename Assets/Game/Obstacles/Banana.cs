using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour {

    public Vector2 bobDirection;
    public float bobSpeed;
    public Level level;

    // Start is called before the first frame update
    void Start() {

        StartCoroutine(IEBob());

        IEnumerator IEBob() {
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

}
