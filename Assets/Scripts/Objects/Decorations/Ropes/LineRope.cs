/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Objects.Decorations {

    /// <summary>
    ///
    /// <summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LineRope : MonoBehaviour {

        //
        public LineRenderer m_Line;
        
        //
        protected EdgeCollider2D m_EdgeCollider;

        //
        protected static float SegmentLength  = 6f/16f;

        [HideInInspector] 
        protected int segmentCount; // The number of segments.
        
        [SerializeField] 
        public Transform startpoint; // The width of the rope.
        
        [SerializeField] 
        protected float weight = 1.5f;
        
        [SerializeField] 
        protected int stiffness = 5;
        
        [SerializeField] 
        public float length; // The width of the rope.
        
        [SerializeField] 
        public float width; // The width of the rope.

        [SerializeField] 
        protected Vector3[] currentPositions; // The current positions of the segments.
        
        [SerializeField] 
        protected Vector3[] previousPositions; // The previous positions of the segments.
        
        [SerializeField] 
        protected Vector3[] velocities; // The previous positions of the segments.

        public void Awake() {
            // m_EdgeCollider = GetComponent<EdgeCollider2D>();
            // m_EdgeCollider.isTrigger = false;
            // m_EdgeCollider.edgeRadius = width;
            gameObject.layer = Game.Physics.CollisionLayers.DecorLayer;
            RopeSegments();
        }

        void FixedUpdate() {
            Simulation();
        }

        /* --- Methods --- */
        // Initalizes the rope segments.
        void RopeSegments() {
            // Get the number of segments for a rope of this length.
            segmentCount = (int)Mathf.Ceil(length / SegmentLength);

            // Initialize the rope segments.
            currentPositions = new Vector3[segmentCount];
            previousPositions = new Vector3[segmentCount];
            velocities = new Vector3[segmentCount];
            
            currentPositions[0] = Vector3.zero;
            previousPositions[0] = currentPositions[0];
            velocities[0] = Vector2.zero;

            m_Line.SetPositions(currentPositions);

            for (int i = 1; i < segmentCount; i++) {
                Vector2 offset = SegmentLength * Random.insideUnitCircle.normalized;
                offset.y = -Mathf.Abs(offset.y);
                currentPositions[i] = currentPositions[i - 1] + (Vector3)offset;
                previousPositions[i] = currentPositions[i];
                velocities[i] = new Vector2(0f, 0f);
            }

            m_Line.positionCount = segmentCount;
            m_Line.SetPositions(currentPositions);

        }

        void Simulation() {
            Vector3 forceGravity = new Vector3(0f, -weight, 0f);
            for (int i = 0; i < segmentCount; i++) {
                Vector3 velocity = currentPositions[i] - previousPositions[i];
                previousPositions[i] = currentPositions[i];
                currentPositions[i] += velocity * 0.975f;
                currentPositions[i] += forceGravity * Time.fixedDeltaTime;
                currentPositions[i] += velocities[i] * Time.fixedDeltaTime;
            }

            for (int i = 0; i < velocities.Length; i++) {
                velocities[i] *= 0.65f;
            }

            for (int i = 0; i < stiffness; i++) {
                Constraints();
            }

            m_Line.SetPositions(currentPositions);

 
        }

        protected void Constraints() {
            currentPositions[0] = Vector3.zero;
            for (int i = 1; i < segmentCount; i++) {
                // Get the distance and direction between the segments.
                float newDist = (currentPositions[i - 1] - previousPositions[i]).magnitude;
                Vector3 direction = (currentPositions[i - 1] - previousPositions[i]).normalized;

                // Get the error term.
                float error = newDist - SegmentLength;
                Vector3 errorVector = direction * error;

                // Adjust the segments by the error term.
                if (i != 1) {
                    currentPositions[i - 1] -= errorVector * 0.5f;
                }
                currentPositions[i] += errorVector * 0.5f;
            }
        }

    }

}
