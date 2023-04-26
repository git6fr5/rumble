/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class Spring : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        private const float VARIATION = 0.2f;

        /* --- Components --- */

        // The sprite shape controller.
        [SerializeField]
        private SpriteShapeController m_SpriteShape = null;
        
        // The head that this follows.
        [SerializeField]
        private Transform m_Head;

        // Cache the start position to access it easier.
        [SerializeField, ReadOnly]
        public Vector3 m_StartPosition;

        // Cache the end position to access it easier.
        [SerializeField, ReadOnly]
        public Vector3 m_EndPosition;

        // Cache the positions to be able to manipulate them.
        [SerializeField, ReadOnly]
        private List<Vector3> m_RelativePositions;
        
        [SerializeField, ReadOnly]
        public float m_TotalLength;

        /* --- Parameters --- */

        #endregion

        // void Start() {
        //     Activate(Vector3.zero, 10f, Vector3.down);
        // }

        // Runs once before the first frame.
        public void Activate(Vector3 offset, float distance, Vector3 direction) {
            // Cache the spline to reference easier. Yes, yes, ik.
            Spline spline = m_SpriteShape.spline;
            m_TotalLength = 0f;

            // Cache the tangent
            Vector3 tangent = Quaternion.Euler(0f, 0f, 90f) * direction;

            // Clear and start the spline and the list.
            transform.SetParent(m_Head.parent);
            transform.localPosition = Vector3.zero;

            spline.Clear();
            spline.InsertPointAt(0, m_Head.localPosition + offset);
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            m_RelativePositions = new List<Vector3>();
            m_RelativePositions.Add(m_Head.localPosition + offset);
            m_StartPosition = m_Head.localPosition + offset;

            // Create the points.
            int i = 1;
            float y = 0f;
            while (y < distance) {

                m_EndPosition = m_StartPosition + direction * y + tangent * Random.Range(-VARIATION, VARIATION);
                spline.InsertPointAt(i, m_EndPosition);
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);

                i += 1;
                y += Random.Range(1.5f * VARIATION, 3f * VARIATION);

                m_RelativePositions.Add(m_EndPosition);

            }

            for (int j = 0; j < m_RelativePositions.Count; j++) {
                m_RelativePositions[j] -= m_EndPosition;
            }

            m_TotalLength = y;

            // Activate the object.
            gameObject.SetActive(true);

        }

        void FixedUpdate() {
            Compress();
        }

        void Compress() {
            Spline spline = m_SpriteShape.spline;
            float y = m_StartPosition.y - m_Head.localPosition.y;
            float compression = (m_TotalLength - y) / m_TotalLength;
            // Relative to the bottom most point.
            int count = m_RelativePositions.Count;
            for (int i = 0; i < count; i++) {
                Vector3 compressedPos = new Vector3(m_RelativePositions[i].x, compression * m_RelativePositions[i].y, m_RelativePositions[i].z); 
                spline.SetPosition(i, compressedPos + m_EndPosition);
            }

        }
        
    }

}
