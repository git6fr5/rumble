/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider) {
        CircleCollider2D temp = collider.GetComponent<CircleCollider2D>();
        if (temp != null) {
            print("entered" + gameObject.name);
        }
    }
    
    void OnTriggerExit2D(Collider2D collider) {
        CircleCollider2D temp = collider.GetComponent<CircleCollider2D>();
        if (temp != null) {
            print("exited" + gameObject.name);
        }
    }

}