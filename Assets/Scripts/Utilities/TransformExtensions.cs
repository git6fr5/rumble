/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtensions {

    ///<summary>
    ///
    ///<summary>
    public static class TransformExtensions {

        public static void Animate(this Transform transform, TransformAnimation animation, float deltaTime) {

            if (animation.Loop) {
                animation.AnimationTimer.Loop(deltaTime);
            }
            else {
                animation.AnimationTimer.TickUp(deltaTime);
            }

            // Position.
            transform.localPosition = animation.Position;
            // Scale.
            transform.localScale = animation.Scale;
            // Rotation.
            transform.localRotation = animation.Rotation;

        }

        public static void SetAnimation(this Transform transform, TransformAnimation animation, float time) {

            animation.AnimationTimer.Set(time, false);

            // Position.
            transform.localPosition = animation.Position;
            // Scale.
            transform.localScale = animation.Scale;
            // Rotation.
            transform.localRotation = animation.Rotation;

        }

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

        // Moves an obstacle towards a target.
        public static void Rotate(this Transform transform, float speed, float deltaTime) {
            transform.eulerAngles += Vector3.forward * speed * deltaTime;
        }

        // Moves an obstacle towards a target.
        public static void RotateTowards(this Transform transform, float angle, float speed, float deltaTime) {
            float amount = Mathf.Abs(angle - transform.eulerAngles.z);
            float direction = Mathf.Sign(angle - transform.eulerAngles.z);
            float deltaAngle = speed * deltaTime;

            if (amount < deltaAngle) {
                transform.eulerAngles += Vector3.forward * angle;
            }
            else {
                transform.eulerAngles += Vector3.forward * direction * deltaAngle;
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