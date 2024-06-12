/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.Tests {

    ///<summary>
    ///
    ///<summary>
    public class Stem : MonoBehaviour {

        #region Variables.
            
        /* --- Components --- */

        // The sprite shape controller.
        [SerializeField]
        private SpriteShapeController m_SpriteShape = null;

        [SerializeField]
        private Transform m_Head = null;

        /* --- Parameters --- */

        // The cached positions of the fully grown plant.
        [SerializeField, ReadOnly]
        private Vector3[] m_CachedPositions = null;
        public Vector3[] positions => m_CachedPositions;
        
        // The cached lengths of when each point is reached.
        [SerializeField, ReadOnly]
        private float[] m_CachedLengths = null;

        // The cached positions of the points defining the tangent at that point.
        [SerializeField, ReadOnly]
        private Vector3[] m_CachedRightTangents = null;
        [SerializeField, ReadOnly]
        private Vector3[] m_CachedLeftTangents = null;

        // The total number of points on this stem.
        [SerializeField, ReadOnly]
        private int m_TotalPositions = 0;

        // The total length of this stem.
        [SerializeField, ReadOnly]
        private float m_TotalLength = 0;

        public float GetPercentFromIndex(int index) {
            return m_CachedLengths[index] / m_TotalLength;
        }

        // The total length of this stem.
        [SerializeField, ReadOnly]
        private int m_CurrentIndex = 0;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            m_TotalPositions = m_SpriteShape.spline.GetPointCount();

            m_CachedPositions = new Vector3[m_TotalPositions];
            m_CachedRightTangents = new Vector3[m_TotalPositions];
            m_CachedLeftTangents = new Vector3[m_TotalPositions];
            m_CachedLengths = new float[m_TotalPositions];
            for (int i = 0; i < m_TotalPositions; i++) {
                m_CachedPositions[i] = m_SpriteShape.spline.GetPosition(i);
                m_CachedRightTangents[i] = m_SpriteShape.spline.GetRightTangent(i);
                m_CachedLeftTangents[i] = m_SpriteShape.spline.GetLeftTangent(i);
                if (i > 0) {
                    m_TotalLength += (m_CachedPositions[i] - m_CachedPositions[i-1]).magnitude;
                }
                m_CachedLengths[i] = m_TotalLength;
            }

            m_SpriteShape.spline.Clear();

        }

        public void Grow(float percent) {
            if (percent <= 0f) { 
                gameObject.SetActive(false);
            }
            else if (percent > 0f && !gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
            if (percent > 1f) { percent = 1f; }


            // Get the last fully grown index and the length left over.
            int index = GetLastFullyGrownIndex(percent);
            m_CurrentIndex = index; // Caching just to see it.

            float leftover = 0f; 
            if (index < m_TotalPositions) {
                leftover = m_TotalLength * percent - m_CachedLengths[index];
            }

            // Draw the stem based on the above info.
            DrawFullyGrownPoints(index);
            DrawLeftover(leftover);

            PositionHead();
        }

        private void PositionHead() {
            Spline spline = m_SpriteShape.spline;
            int currPoints = spline.GetPointCount();
            if (currPoints <= 0) {
                m_Head.localPosition = Vector3.zero;
                m_Head.localRotation = Quaternion.identity;
            }
            else {
                m_Head.localPosition = spline.GetPosition(currPoints - 1);
                if (currPoints > 2) {
                    float angle = Vector2.SignedAngle(Vector2.up, spline.GetRightTangent(currPoints - 1) - spline.GetLeftTangent(currPoints - 1));
                    m_Head.localRotation = Quaternion.Euler(0f, 0f, angle);
                }
                else {
                    m_Head.localRotation = Quaternion.identity;
                }
            }
        }

        private void DrawFullyGrownPoints(int index) {
            Spline spline = m_SpriteShape.spline;
            int currPoints = spline.GetPointCount();
 
            spline.Clear();
            for (int i = 0; i < index; i++) {
                spline.InsertPointAt(i, m_CachedPositions[i]);
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                spline.SetRightTangent(i, m_CachedRightTangents[i]);
                spline.SetLeftTangent(i, m_CachedLeftTangents[i]);
            }

        }

        private void DrawLeftover(float leftover) {
            if (leftover == 0f) { return; }

            Spline spline = m_SpriteShape.spline;
            int currPoints = spline.GetPointCount();

            if (currPoints < 1) { return; }

            Vector3 position = leftover * (m_CachedPositions[currPoints] - m_CachedPositions[currPoints - 1]) + m_CachedPositions[currPoints];
            
            float totalDistance = (m_CachedPositions[currPoints] - m_CachedPositions[currPoints - 1]).magnitude;
            float currDistance = (position - m_CachedPositions[currPoints - 1]).magnitude;
            float lerp = currDistance / totalDistance;

            Vector3 rightTangent =  lerp * m_CachedRightTangents[currPoints] + (1f - lerp) * m_CachedRightTangents[currPoints - 1];
            Vector3 leftTangent =  lerp * m_CachedLeftTangents[currPoints] + (1f - lerp) * m_CachedLeftTangents[currPoints - 1];

            spline.InsertPointAt(currPoints, position);
            spline.SetTangentMode(currPoints, ShapeTangentMode.Continuous);
            spline.SetRightTangent(currPoints, rightTangent);
            spline.SetLeftTangent(currPoints, leftTangent);

        }

        private int GetLastFullyGrownIndex(float percent) {
            if (percent == 0f) { return 0; }

            for (int i = 0; i < m_TotalPositions; i++) {
                if (percent < m_CachedLengths[i] / m_TotalLength) {
                    return i;
                }
            }
            return m_TotalPositions;
        }

    }

}
