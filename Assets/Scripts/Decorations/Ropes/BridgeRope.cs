using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Platformer.
using Platformer.Decorations;

namespace Platformer.Decorations {

    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class BridgeRope : Rope {

        [SerializeField] public Transform endpoint;

        // Runs once on initialization.
        protected override void OnAwake() {
            // Cache these references.
            lineRenderer = GetComponent<LineRenderer>();
            edgeCollider = GetComponent<EdgeCollider2D>();
            edgeCollider.edgeRadius = ropeWidth / 2f;
            edgeCollider.isTrigger = true;
        }

        protected override void Constraints() {
            ropeSegments[0] = Vector2.zero;
            ropeSegments[segmentCount - 1] = endpoint.position - startpoint.position;

            for (int i = 1; i < segmentCount; i++) {
                // Get the distance and direction between the segments.
                float newDist = (ropeSegments[i - 1] - ropeSegments[i]).magnitude;
                Vector3 direction = (ropeSegments[i - 1] - ropeSegments[i]).normalized;

                // Get the error term.
                float error = newDist - SegmentLength;
                Vector3 errorVector = direction * error;

                // Adjust the segments by the error term.
                if (i != 1) {
                    ropeSegments[i - 1] -= errorVector * 0.5f;
                }
                ropeSegments[i] += errorVector * 0.5f;
            }
        }
    
    }

}
