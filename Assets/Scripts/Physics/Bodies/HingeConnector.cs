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

        void Update() {
            if (Application.isPlaying) {
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