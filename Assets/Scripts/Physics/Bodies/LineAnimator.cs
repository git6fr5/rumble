/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    [RequireComponent(typeof(LineRenderer))]
    public class LineAnimator : MonoBehaviour, IPositionArrayReciever {

        protected LineRenderer lineRenderer;

        void Awake() {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
        }

        public void RecievePositions(Vector3[] positions) {
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }

    }

}