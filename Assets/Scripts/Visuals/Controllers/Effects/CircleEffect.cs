/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Visuals.Effects;

namespace Platformer.Visuals.Effects {

    ///<summary>
    /// Creates a circle.
    ///<summary>
    [RequireComponent(typeof(LineRenderer))]
    public class CircleEffect : Effect {

        #region Variables.
        
        /* --- Constants --- */

        // The default radius of the circle.
        public const float DEFAULT_RADIUS = 3f;

        // The default duration that the circle lasts for.
        public const float DEFAULT_DURATION = 3f;

        // The default thickness of the circle.
        public const float DEFAULT_THICKNESS = 0.2f;

        // The number of points in the pseudo-circle.
        public const int COUNT = 24;

        /* --- Components --- */

        // The line renderer attached to this gameObject.
        public LineRenderer m_LineRenderer => GetComponent<LineRenderer>();
 
        /* --- Members --- */

        // The radius of this circle.
        public float m_Radius = DEFAULT_RADIUS;

        // The duration that this circle lives.
        public float m_Duration = DEFAULT_DURATION;

        // The cached positions of the points on this circle.
        [HideInInspector]
        private Vector3[] m_CachedPositions = new Vector3[COUNT];

        #endregion

        #region Methods.

        // Plays the circle effect for the given duration.
        public void Play(float duration) {
            m_Duration = duration;
            Play();
        }

        // Plays the circle effect.
        public override void Play() {
            m_Radius = DEFAULT_RADIUS;
            m_LineRenderer.positionCount = COUNT;
            m_LineRenderer.startWidth = DEFAULT_THICKNESS;
            m_LineRenderer.endWidth = DEFAULT_THICKNESS;
            Set();
            base.Play();
        }

        // Runs once every fixed interval.
        private void FixedUpdate() {
            if (m_Duration > 0f) {
                float dr = DEFAULT_RADIUS * Time.fixedDeltaTime / m_Duration;
                m_Radius -= dr;
                if (m_Radius <= 0f) {
                    Stop();
                }
                else {
                    Set();
                }
            }
        }

        // Sets the positions on the line renderer.
        private void Set() {
            for (int i = 0; i < COUNT; i++) {
                float angle = (float)i * 360f / (float)COUNT;
                Vector3 position = m_Radius * (Quaternion.Euler(0f, 0f, angle) * Vector2.right);
                m_CachedPositions[i] = position;
            }
            m_LineRenderer.SetPositions(m_CachedPositions);
        }

        #endregion

    }

}