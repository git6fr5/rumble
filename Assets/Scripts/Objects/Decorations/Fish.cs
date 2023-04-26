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
    public class Fish : MonoBehaviour {

        #region Variables.

        // The frame rate that the animation plays at.
        public const float FRAME_RATE = 3;

        // The frame rate that the animation plays at.
        public const int OFFSET = 0;

        /* --- Components --- */
        
        [SerializeField]
        private TransformAnimation m_Animation = null;

        [SerializeField]
        private SpriteRenderer[] m_SpriteRenderers = null;

        // The sprites this is currently animating through.
        [SerializeField]
        private Sprite[] m_SpriteAnimation = null;

        // The sprite we are currently on.
        [SerializeField]
        private int m_CurrentFrame = 0;

        // The ticks until the next frame.
        [SerializeField, ReadOnly]
        private float m_Ticks = 0f;

        #endregion

        void FixedUpdate() {
            transform.Animate(m_Animation, Time.fixedDeltaTime);
            Animate(Time.fixedDeltaTime);
        }

        private void Animate(float dt) {
            // Update the ticks.
            m_Ticks += dt;
            int previousFrame = m_CurrentFrame;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * FRAME_RATE) % m_SpriteAnimation.Length;

            // Change the frame.
            if (previousFrame != m_CurrentFrame) {
                for (int i = 0; i < m_SpriteRenderers.Length; i++) {
                    m_SpriteRenderers[i].sprite = m_SpriteAnimation[(m_CurrentFrame + i * OFFSET) % m_SpriteAnimation.Length];
                }
            }

        }

    }

}
