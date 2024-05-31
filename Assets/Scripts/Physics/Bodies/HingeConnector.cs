/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    [ExecuteInEditMode]
    public class HingeConnector : MonoBehaviour {

        public HingeJoint2D[] hinges;
        // Transform anchor;

        // public float dampening;
        public AnimationCurve dampingCurve;

        public float angleLimit;
        public AnimationCurve limitCurve;

        Vector3[] origins;
        Rigidbody2D[] bodies;

        public bool staticTip;

        void Start() {

            if (Application.isPlaying) {

                origins = new Vector3[hinges.Length];
                bodies = new Rigidbody2D[hinges.Length];
                for (int n=0; n < hinges.Length; n++) {
                    origins[n] = hinges[n].transform.localPosition;
                    bodies[n] = hinges[n].GetComponent<Rigidbody2D>();
                    if (staticTip && n==hinges.Length-1) { bodies[n].bodyType = RigidbodyType2D.Static; }
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

            hinges = new HingeJoint2D[transform.childCount];
            // if (anchor != null && anchor.gameObject != null) {
            //     DestroyImmediate(anchor.gameObject);
            // }
            // anchor = new GameObject("anchor").transform;

            int i = 0;
            foreach (Transform child in transform) {

                HingeJoint2D hinge = child.gameObject.GetComponent<HingeJoint2D>();
                if (hinge == null) {
                    hinge = child.gameObject.AddComponent<HingeJoint2D>();
                }


                hinges[i] = hinge;
                i += 1;

                float r = (float)i / (float)(transform.childCount);

                JointAngleLimits2D _limits = new JointAngleLimits2D();
                _limits.max = angleLimit * r;
                _limits.min = -angleLimit * r;
                hinge.limits = _limits;

                hinge.useLimits = true;

            }

            for (int j = hinges.Length-1; j >= 1; j--) {
                hinges[j].connectedBody = hinges[j-1].GetComponent<Rigidbody2D>();
            }

            // anchor.transform.position = hinges[0].transform.position;
            // anchor.SetParent(transform);

            Rigidbody2D anchorBody = gameObject.GetComponent<Rigidbody2D>();
            if (anchorBody == null) {
                anchorBody = gameObject.AddComponent<Rigidbody2D>();
            }
            hinges[0].connectedBody = anchorBody;

        }

    }

}