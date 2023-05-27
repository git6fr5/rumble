/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Objects.Decorations {

    [RequireComponent(typeof(LineRenderer))]
    public class Tail : MonoBehaviour {

        // The seperation between segments.
        public const float SEGMENT_LENGTH = 0.25f;

        // The line renderer attached to this object.
        private LineRenderer lineRenderer;
        // The head of the tail.
        [SerializeField] private Transform head;
        // The outline of the tail.
        [SerializeField] private LineRenderer outlineRenderer;
        // The outline of the tail.
        [SerializeField] private EdgeCollider2D edgeCollider;
        
        // The number of segments based on the length.
        private int segmentCount;
        // The positions of the points along the tail.
        private Vector3[] currentPositions;
        // The positions of the points along the tail.
        private Vector3[] previousPositions;
        //
        private Vector2[] points;

        // The length of the tail.
        [SerializeField] private float length = 3f;
        // The start width of the tail.
        [SerializeField] private float startWidth = 0.35f;
        // The end width of the tail.
        [SerializeField] private float endWidth = 0.35f;
        // The outline width of the tail.
        [SerializeField] private float outlineWidth = 0.5f;

        // The ticks that control the oscillating motion.
        private float ticks = 0f;

        // The amplitude of the oscillation.
        [SerializeField] private float amplitude = 1f;
        // The period of a single oscillation.
        [SerializeField] private float period = 1f;
        // The stiffness of the tail.
        [SerializeField] private int stiffness = 5;
        // The momentum that stays every update.
        [SerializeField] private float momentumFactor = 0.2f;

        // Runs once before the first frame.
        void Start() {
            // Cache the line renderer.
            lineRenderer = GetComponent<LineRenderer>();
            // Set these up.
            segmentCount = (int)Mathf.Ceil(length / SEGMENT_LENGTH);
            currentPositions = new Vector3[segmentCount];
            previousPositions = new Vector3[segmentCount];
            points = new Vector2[segmentCount];
            for (int i = 0; i < currentPositions.Length; i++) {
                currentPositions[i] = head.transform.position;
                previousPositions[i] = head.transform.position;
                points[i] = (Vector2)head.transform.position;
            }
            ticks = 0f;
            transform.SetParent(null);
            transform.position = Vector3.zero;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            if (segmentCount < 2) {
                return;
            }

            // Momentum(Time.fixedDeltaTime);
            Follow(Time.fixedDeltaTime);
            
            if (character.OnGround) {
                Gravity(Time.fixedDeltaTime);
            }

            Render();
        }

        public float grav = 1f;

        public Platformer.Character.CharacterController character;

        // Simmulates the movement of the tail.
        void Follow(float dt) {
            // Snap the 0 node to the current position adjusted for oscillation.
            ticks += dt;
            Vector3 normal = Quaternion.Euler(0f, 0f, head.transform.eulerAngles.z + 90f) * Vector3.right;
            currentPositions[0] = head.transform.position + amplitude * normal * Mathf.Sin(2f * Mathf.PI * ticks / period);

            // Itterate through the positions.
            for (int i = 1; i < currentPositions.Length; i++) {
                // Keep the nodes together.
                float seperation = (currentPositions[i - 1] - currentPositions[i]).magnitude;
                if (seperation > SEGMENT_LENGTH) {
                    Vector3 direction = (currentPositions[i - 1] - currentPositions[i]).normalized;
                    currentPositions[i] -= (direction * (SEGMENT_LENGTH - seperation));
                }
            }

        }

        void Gravity(float dt) {
            for (int i = 0; i < segmentCount; i++) {
                currentPositions[i] += grav * Vector3.down * dt;
            }
        }

        void Momentum(float dt) {
            for (int i = 0; i < segmentCount; i++) {
                Vector3 velocity = currentPositions[i] - previousPositions[i];
                previousPositions[i] = currentPositions[i];
                currentPositions[i] += velocity * momentumFactor;
            }

            for (int i = 0; i < stiffness; i++) {
                Constraints();
            }
 
        }

        protected void Constraints() {
            currentPositions[0] = Vector3.zero;
            for (int i = 1; i < segmentCount; i++) {
                // Get the distance and direction between the segments.
                float newDist = (currentPositions[i - 1] - previousPositions[i]).magnitude;
                Vector3 direction = (currentPositions[i - 1] - previousPositions[i]).normalized;

                // Get the error term.
                float error = newDist - SEGMENT_LENGTH;
                Vector3 errorVector = direction * error;

                // Adjust the segments by the error term.
                if (i != 1) {
                    currentPositions[i - 1] -= errorVector * 0.5f;
                }
                currentPositions[i] += errorVector * 0.5f;
            }
        }

        // Passes the positions to the line renderer.
        void Render() {
            // Tail
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
            lineRenderer.positionCount = segmentCount;
            lineRenderer.SetPositions(currentPositions);
            // Outline
            
            // Head.
            // if (head != null) {
            //     head.position = currentPositions[0];
            //     head.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector3.right, currentPositions[0] - currentPositions[1]); 
            // }

            for (int i = 0; i < segmentCount; i++) {
                points[i] = (Vector2)(currentPositions[i]);// - head.transform.position);
            }
            edgeCollider.points = points;
            
        }



    }

}