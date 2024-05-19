/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    [RequireComponent(typeof(LineRenderer))]
    public class SendChildPositions : MonoBehaviour {
        
        IPositionArrayReciever positionReciever;
        Vector3[] positions;
        Transform[] children;
        int count = 0;

        void Awake() {
            positionReciever = GetComponent<IPositionArrayReciever>();
            positions = new Vector3[transform.childCount];
            children = new Transform[transform.childCount];
            count = transform.childCount;

            int i = 0;
            foreach (Transform child in transform) {
                children[i] = child;
                i += 1;
            }

        }

        void FixedUpdate() {
            if (positionReciever == null) { return; }

            for (int i = 0; i < count; i++) {
                positions[i] = children[i].localPosition;
            }

            positionReciever.RecievePositions(positions);

        }

    }

}