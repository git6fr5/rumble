/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    /// Decorates a level with grass.
    ///<summary>
    public class Jelly : MonoBehaviour {

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private float m_length;

        [SerializeField]
        private Collider2D m_collisionObj;

        [SerializeField]
        private LayerMask m_layerMask;
        
        [SerializeField]
        private Vector3[] basePoints;
        public int pointCount;

        [SerializeField]
        private Vector3[] currPoints;
        private Vector3[] velocities;
        private Vector3[] linePoints;

        [SerializeField]
        private float m_tension;

        [SerializeField]
        private Vector3 m_contactDirection;

        public void Load(Vector3 origin, Vector3 direction, Vector3 contactDir, float length, float width) {

            transform.position = origin;

            m_length = length;
            m_contactDirection = contactDir;

            basePoints = new Vector3[pointCount];
            currPoints = new Vector3[pointCount];
            velocities = new Vector3[pointCount];
            linePoints = new Vector3[pointCount / 4];

            linePoints[0] = m_length * direction + Vector3.down * 0.5f;

            for (int i = pointCount - 1; i >= 0; i--) {
                
                int j = pointCount - (i+1);
                basePoints[j] = ((float)i * m_length / (float)(pointCount - 1)) * direction;
                currPoints[j] = basePoints[j];
                velocities[j] = Vector3.zero;

                if (j % 4 == 0 && j >= 4) {
                    int k = j / 4;
                    linePoints[k] = get_point(basePoints, j);
                }

            }

            // linePoints[linePoints.Length - 1] = m_length * direction + Vector3.down * 0.5f;

            lineRenderer.positionCount = pointCount / 4;
            lineRenderer.SetPositions(linePoints);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            gameObject.SetActive(true);

        }

        void FixedUpdate() {
            RaycastHit2D hit = UnityEngine.Physics2D.Raycast(transform.position, Vector2.right, m_length, m_layerMask);
            m_collisionObj = hit.collider;

            // for (int i = 0; i < pointCount; i++) {
            //     currPoints[i] = basePoints[i];
            // }

            for (int i = 0; i < m_depth; i++) {
                get_vel();
            }

            if (m_collisionObj != null) {
                float radius = m_collisionObj.GetComponent<CircleCollider2D>().radius;
                Vector3 colPos = m_collisionObj.transform.position - transform.position; // + radius * m_contactDirection;

                int closestPointIndex = 1;
                float minSqrDist = (colPos - basePoints[closestPointIndex]).sqrMagnitude;
                float sqrDist = 0f;

                for (int i = 2; i < pointCount - 1; i++) {

                    sqrDist = (colPos - basePoints[i]).sqrMagnitude;
                    if (sqrDist < minSqrDist) {
                        minSqrDist = sqrDist;
                        closestPointIndex = i;
                    }

                }

                // currPoints[closestPointIndex] = colPos + radius * m_contactDirection;
                float o = 0f;
                float sqrDistToCenter = 0f;
                float sqrDistToMin = 0f;
                float minDist = Mathf.Sqrt(minSqrDist);

                for (int i = 1; i < pointCount; i++) {
                    sqrDistToCenter = (basePoints[i] - colPos).sqrMagnitude;
                    sqrDistToMin = (basePoints[i] - basePoints[closestPointIndex]).sqrMagnitude;
                    if (sqrDistToCenter < radius * radius) {
                        o = Mathf.Sqrt(radius * radius - sqrDistToMin);
                        currPoints[i] = basePoints[i] - (minDist - o) * m_contactDirection;
                        velocities[i] = Vector3.zero;
                    }
                    // else if (sqrDistToCenter < 4f * radius * radius) {
                    //     o = Mathf.Sqrt(radius * radius - sqrDistToMin);
                    //     currPoints[i] = basePoints[i] + (minDist - o) * m_contactDirection;
                    //     velocities[i] = Vector3.zero;
                    // }
                }

                // currPoints[closestPointIndex] = colPos;

                // if (currPoints.Length > closestPointIndex+2) {
                //     currPoints[closestPointIndex+1] = (currPoints[closestPointIndex] + currPoints[closestPointIndex+2]) / 2f;
                // }
                // if (closestPointIndex-2 >= 0) {
                //     currPoints[closestPointIndex-1] = (currPoints[closestPointIndex] + currPoints[closestPointIndex-2]) / 2f;
                // }

            }

            for (int i = 1; i < pointCount - 1; i++) {
                currPoints[i] += velocities[i] * Time.fixedDeltaTime;
            }

            for (int i = 4; i < pointCount; i+=4) {
                linePoints[i / 4] = get_point(currPoints, i);
            }
            linePoints[0] = m_length * Vector3.right + Vector3.down * m_jelly_down_offset;
            linePoints[linePoints.Length-1] = Vector3.down * m_jelly_down_offset;

            lineRenderer.SetPositions(linePoints); 


        }

        public Vector3 get_point(Vector3[] points, int i) {
            return Vector3.down * m_jelly_down_offset + (points[i-4] + points[i-3] + points[i-2] + points[i-1]) / 4f;
        }

        [SerializeField]
        private float m_damping;

        [SerializeField]
        private int m_depth;

        [SerializeField]
        private float m_jelly_down_offset = 0.65f;

        private void get_vel() {
            for (int i = 1; i < pointCount - 1; i++) {
                Vector3 acceleration = basePoints[i] - currPoints[i];
                velocities[i] += m_tension * acceleration;
                velocities[i - 1] += m_tension * acceleration / 2f;
                velocities[i + 1] += m_tension * acceleration / 2f;
            }

            for (int i = 0; i < velocities.Length; i++) {
                velocities[i] = (velocities[i] - velocities[i] * m_damping);
                if (velocities[i].sqrMagnitude < 0.01f) {
                    currPoints[i] = basePoints[i];
                    velocities[i] = Vector3.zero;
                }
            }
        }

        void OnDrawGizmos() {
            if (m_collisionObj != null) {
                Gizmos.color = Color.red;
            }
            else {
                Gizmos.color = Color.white;
            }
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * m_length);

            if (currPoints == null || currPoints.Length < 2) { return; }

            for (int i = 4; i < pointCount; i+=4) {
                Vector3 cachePointA = (currPoints[i-4] + currPoints[i-3] + currPoints[i-2] + currPoints[i-1]) / 4f;
                Vector3 cachePointB = (currPoints[i] + currPoints[i+1] + currPoints[i+2] + currPoints[i+3]) / 4f;
                Gizmos.DrawLine(transform.position + cachePointA, transform.position + cachePointB);
            }


        }
        

    }

}
