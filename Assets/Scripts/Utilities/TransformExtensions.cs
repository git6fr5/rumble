/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtensions {

    ///<summary>
    ///
    ///<summary>
    public static class TransformExtensions {

        // Moves an obstacle towards a target.
        public static void Move(this Transform transform, Vector3 destination, float speed, float deltaTime, List<Transform> transforms = null) {
            if (destination == transform.position) {
                return;
            }

            Vector3 displacement = destination - transform.position;
            Vector3 deltaPosition = displacement.normalized * speed * deltaTime;
            if (displacement.magnitude < deltaPosition.magnitude) {
                deltaPosition = displacement;
            }

            transform.position += deltaPosition;
            if (transforms != null) {
                transforms.Drag(deltaPosition);
            }
            
        }

        // Drags a collection of transforms with the obstacle.
        public static void Drag(this List<Transform> transforms, Vector3 deltaPosition) {
            for (int i = 0; i < transforms.Count; i++) {
                transforms[i].position += deltaPosition;
            }
        }

        // Cycles an object around a given origin.
        public static void Cycle(this Transform transform, float ticks, float period, Vector3 origin, Vector2 ellipse) {
            Vector3 horizontal = Vector3.right * ellipse.x * Mathf.Cos(2f * Mathf.PI * ticks / period);
            Vector3 vertical = Vector3.up * ellipse.y * Mathf.Sin(2f * Mathf.PI * ticks / period);
            transform.position = origin + horizontal + vertical;
        }

        // Shakes the obstacle a little bit.
        public static void Shake(this Transform transform, Vector3 origin, float strength) {
            transform.position = origin;
            transform.position += (Vector3)Random.insideUnitCircle * strength;
        }

        // Shakes the obstacle along the horizontal axis.
        public static void HorizontalShake(this Transform transform, Vector3 origin, float strength) {
            transform.position = origin;
            transform.position += (Vector3)Random.insideUnitCircle * strength;
            transform.position = new Vector3(transform.position.x, origin.y, origin.z);
        }

    }
    
}