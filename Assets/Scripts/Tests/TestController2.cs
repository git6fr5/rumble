/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character;

namespace Platformer.Tests {

    ///<summary>
    ///
    ///<summary>
    public class TestController2 : MonoBehaviour {

        public float speed;
        public Rigidbody2D body;

        void Update() {
            Vector2 dx = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed * Time.deltaTime);
            transform.position += (Vector3)dx;

            body.position = (Vector2)transform.position;
        }


    }

}