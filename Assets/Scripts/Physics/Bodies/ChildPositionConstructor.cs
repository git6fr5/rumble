/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    [ExecuteInEditMode, DefaultExecutionOrder(-1000)]
    public class ChildPositionConstructor : MonoBehaviour {

        public float rScale = 1f;

        public AnimationCurve x;
        public float xScale;
        public AnimationCurve y;
        public float yScale;

        public int count;
        public bool pause;

        Transform[] children;
        Transform[] prevChildren;

        void Update() {
            if (Application.isPlaying || pause) {
                return;
            }

            if (transform.childCount != count || children == null) {
                
                prevChildren = new Transform[transform.childCount];
                int n = 0;
                foreach (Transform child in transform) {
                    prevChildren[n] = child;
                    n++;
                }

                for (int i = 0; i < n; i++) {
                    DestroyImmediate(prevChildren[i].gameObject);
                } 

                children = new Transform[count];
                for (int i = 0; i < count; i++) {
                    Transform child = new GameObject("child " + i.ToString()).transform;
                    child.SetParent(transform);
                    children[i] = child;
                }
            }

            for (int i = 0; i < count; i++) {
                float r = (float)i / (float)(count - 1);
                Vector3 localPosition = new Vector3(x.Evaluate(r) * xScale, y.Evaluate(r) * yScale, 0f);
                if (children[i] != null) {
                    children[i].localPosition = rScale * localPosition;
                }
            }

        }

        void OnDrawGizmos() {
            Transform prev = transform;
            foreach (Transform child in transform) {
                Gizmos.DrawLine(child.position, prev.position);
                prev = child;
            }
        }

    }

}