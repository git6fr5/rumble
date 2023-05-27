/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Orbs;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    ///
    ///<summary>
    public class OrbAnimator : MonoBehaviour {

        #region Components.
        
        /* --- Constants --- */

        // The frame rate that the animation plays at.
        public const float FRAME_RATE = 2;

        /* --- Components --- */

        // The sprite renderer attached to this gameObject.
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer = null;

        /* --- Parameters --- */

        // The sprites this is currently animating through.
        [SerializeField]
        private Sprite[] m_Animation = null;
        private int m_OriginalLength = 0;

        // The sprite we are currently on.
        [SerializeField]
        private int m_CurrentFrame = 0;

        // The ticks until the next frame.
        [SerializeField, ReadOnly]
        private float m_Ticks = 0f;

        // The idle transform movement.
        [SerializeField]
        private TransformAnimation m_IdleTransformMovement = null;

        #endregion

        public void Initialize(OrbObject orb) {
            // Cache these components.
            // m_Orb = orb;

            // Set the sprite order.
            m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.OrbLayer;
            m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.OrbOrder;

            // Calculate these values.
            Sprite[] animation = new Sprite[4 * m_Animation.Length - 2];
            int index = 0;
            for (int i = 0; i < m_Animation.Length; i++) {
                animation[i] = m_Animation[i];
            }
            index += m_Animation.Length;
            for (int i = 0; i < m_Animation.Length; i++) {
                animation[i + index] = m_Animation[m_Animation.Length - (i + 1)];
            }
            index += m_Animation.Length;
            for (int i = 0; i < m_Animation.Length - 1; i++) {
                animation[i + index] = m_Animation[i + 1];
            }
            index += m_Animation.Length - 1;
            for (int i = 0; i < m_Animation.Length - 1; i++) {
                animation[i + index] = m_Animation[m_Animation.Length - (i + 1)];
            }
            index += m_Animation.Length - 1;

            m_OriginalLength = m_Animation.Length;
            m_Animation = animation;

        }

        // Runs once every fixed interval.
        protected virtual void FixedUpdate()  {
            transform.Animate(m_IdleTransformMovement, Time.fixedDeltaTime);
            Animate(Time.fixedDeltaTime);
        }

        private void Animate(float dt) {
            // Update the ticks.
            m_Ticks += dt;
            int previousFrame = m_CurrentFrame;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * FRAME_RATE) % m_Animation.Length;

            // Change the frame.
            if (previousFrame != m_CurrentFrame) {
                m_SpriteRenderer.sprite = m_Animation[m_CurrentFrame];

                bool flipped = m_CurrentFrame >= m_OriginalLength && m_CurrentFrame < 2f * m_OriginalLength + m_OriginalLength - 1;
                if (flipped) {
                    m_SpriteRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else {
                    m_SpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                }

            }

        }
        
    }
}