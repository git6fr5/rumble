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
        // public int pointsPerUnit;
        public int pointCount = 10;

        [SerializeField]
        private Vector3[] currPoints;
        private Vector3[] velocities;
        private Vector3[] linePoints;

        [SerializeField]
        private float m_tension;

        [SerializeField]
        private Vector3 m_contactDirection;

        [SerializeField]
        private float m_damping;

        [SerializeField]
        private int m_depth;

        [SerializeField]
        private float m_jelly_down_offset = 0.65f;

        [SerializeField]
        private float m_max_vel = 1f;

        [SerializeField]
        private float accFactor = 20f;

        private bool loaded = false;

        void Start() {
            if (!loaded) {
                Load(transform.position, Vector3.right, m_contactDirection, m_length, 1f);
            }
        }


        public void Load(Vector3 origin, Vector3 direction, Vector3 contactDir, float length, float width) {
            // pointCount = (int)(length * pointsPerUnit) * 4 + 8;
            pointCount *= 4;

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
                    linePoints[k] = CalculateLinePoint(basePoints, j);
                }

            }

            lineRenderer.positionCount = pointCount / 4;
            lineRenderer.SetPositions(linePoints);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            loaded = true;
            gameObject.SetActive(true);

        }

        void FixedUpdate() {
            RaycastHit2D hit = UnityEngine.Physics2D.Raycast(transform.position, Vector2.right, m_length, m_layerMask);
            m_collisionObj = hit.collider;

            if (m_collisionObj != null) {
                
                float radius = m_collisionObj.GetComponent<CircleCollider2D>().radius;
                Vector3 colPos = m_collisionObj.transform.position - transform.position;
                
                float minSqrDist;
                int closestPointIndex = GetClosestPoint(colPos, out minSqrDist);
                GetAllWithinRadius(radius, colPos, closestPointIndex, minSqrDist);

            }

            for (int i = 0; i < m_depth; i++) {
                UpdateVelocities();
            }

            velocities[0] = Vector3.zero;
            velocities[velocities.Length - 1] = Vector3.zero;
            for (int i = 1; i < pointCount - 1; i++) {
                currPoints[i] += velocities[i] * Time.fixedDeltaTime;
            }

            for (int i = 4; i < pointCount; i+=4) {
                linePoints[i / 4] = CalculateLinePoint(currPoints, i);
            }
            linePoints[0] = m_length * Vector3.right + Vector3.down * m_jelly_down_offset;
            linePoints[linePoints.Length-1] = Vector3.down * m_jelly_down_offset;

            lineRenderer.SetPositions(linePoints); 


        }

        private int GetClosestPoint(Vector3 colPos, out float minSqrDist) {
            
            int closestPointIndex = 1;
            minSqrDist = (colPos - basePoints[closestPointIndex]).sqrMagnitude;
            float sqrDist = 0f;

            for (int i = 2; i < pointCount - 1; i++) {
                sqrDist = (colPos - basePoints[i]).sqrMagnitude;
                if (sqrDist < minSqrDist) {
                    minSqrDist = sqrDist;
                    closestPointIndex = i;
                }

            }
            return closestPointIndex;

        }

        private void GetAllWithinRadius(float radius, Vector3 colPos, int closestPointIndex, float minSqrDist) {
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
            }
        }

        private void UpdateVelocities() {
            for (int i = 2; i < pointCount - 2; i++) {
                Vector3 acceleration = Time.fixedDeltaTime * accFactor * (basePoints[i] - currPoints[i]) / (float)m_depth;
                velocities[i] += m_tension * acceleration;
                velocities[i - 1] -= m_tension * acceleration / 2f;
                velocities[i + 1] -= m_tension * acceleration / 2f;
            }

            for (int i = 0; i < velocities.Length; i++) {
                velocities[i] = velocities[i] - (velocities[i] * m_damping);
                if (velocities[i].sqrMagnitude > m_max_vel * m_max_vel) {
                    velocities[i] = velocities[i].normalized * m_max_vel;
                }    
                else if (velocities[i].sqrMagnitude < 0.01f) {
                    currPoints[i] = basePoints[i];
                    velocities[i] = Vector3.zero;
                }
            }
        }


        public Vector3 CalculateLinePoint(Vector3[] points, int i) {
            return Vector3.down * m_jelly_down_offset + (points[i-4] + points[i-3] + points[i-2] + points[i-1]) / 4f;
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
