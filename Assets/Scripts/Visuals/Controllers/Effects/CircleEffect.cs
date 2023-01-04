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
    /// Controls the particle effects in the game.
    ///<summary>
    [RequireComponent(typeof(LineRenderer))]
    public class CircleEffect : Effect {

        public LineRenderer m_LineRenderer => GetComponent<LineRenderer>();

        public const float MAX_RADIUS = 3f;

        public const float DEFAULT_DURATION = 3f;

        public const int COUNT = 24;

        public float m_Radius = MAX_RADIUS;

        public float m_Duration = DEFAULT_DURATION;

        [HideInInspector]
        private Vector3[] m_CachedPositions = new Vector3[COUNT];

        public override void Play() {
            m_Radius = MAX_RADIUS;
            Set();
            base.Play();
        }

        public void Play(float duration) {
            m_Duration = duration;
            Play();
        }

        void FixedUpdate() {
            if (m_Duration > 0f) {
                float dr = MAX_RADIUS * Time.fixedDeltaTime / m_Duration;
                m_Radius -= dr;
                if (m_Radius <= 0f) {
                    Stop();
                }
                else {
                    Set();
                }
            }
        }

        public void Set() {
            for (int i = 0; i < COUNT; i++) {
                float angle = (float)i * 360f / (float)COUNT;
                Vector3 position = m_Radius * (Quaternion.Euler(0f, 0f, angle) * Vector2.right);
                m_CachedPositions[i] = position;
            }
            m_LineRenderer.SetPositions(m_CachedPositions);
        }

    }

}