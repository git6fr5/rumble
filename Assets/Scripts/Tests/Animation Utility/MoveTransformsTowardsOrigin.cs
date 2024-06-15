/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    public class MoveTransformsTowardsOrigin : MonoBehaviour {

        public HingeJoint2D[] hinges;
        // Transform anchor;

        // public float dampening;
        public AnimationCurve dampingCurve;

        Vector3[] origins;
        Rigidbody2D[] bodies;

        void Start() {

            if (Application.isPlaying) {

                origins = new Vector3[hinges.Length];
                bodies = new Rigidbody2D[hinges.Length];
                for (int n=0; n < hinges.Length; n++) {
                    origins[n] = hinges[n].transform.localPosition;
                    bodies[n] = hinges[n].GetComponent<Rigidbody2D>();
                }

                return;
            }

        }

        void Update() {
            if (Application.isPlaying) {

                for (int n=0; n < hinges.Length; n++) {
                    float r = (float)n / (float)(hinges.Length-1);
                    bodies[n].velocity *= dampingCurve.Evaluate(r);
                    bodies[n].velocity -= (Vector2)(bodies[n].transform.localPosition - origins[n]) * 0.02f;
                }

                return;
            }

        }

    }

}