/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;
using Gobblefish.Animation;

namespace Platformer.Graphics {

    ///<summary>
    ///
    ///<summary>
    public class PointInDirection : MonoBehaviour {

        public Rigidbody2D body;
        protected Vector3 scale;

        void Awake() {
            scale = transform.localScale;
        }

        void FixedUpdate() {
            float signedAngle = Vector2.SignedAngle(Vector2.right, body.velocity);
            if (signedAngle < -90f || signedAngle > 90f) {
                signedAngle += 180f;
                signedAngle = signedAngle % 360f;
                transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
            Quaternion direction = Quaternion.Euler(0f, 0f, signedAngle);
            transform.localRotation = direction;
        }

    }

}
