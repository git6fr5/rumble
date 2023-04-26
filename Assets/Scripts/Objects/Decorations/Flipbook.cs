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
    public class Flipbook : MonoBehaviour {

        #region Variables.

        // The frame rate that the animation plays at.
        public float m_FrameRate = 3;

        /* --- Components --- */
        
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer = null;

        // The sprites this is currently animating through.
        [SerializeField]
        private Sprite[] m_Animation = null;

        // The sprite we are currently on.
        [SerializeField]
        private int m_CurrentFrame = 0;

        // The ticks until the next frame.
        [SerializeField, ReadOnly]
        private float m_Ticks = 0f;

        #endregion

        void FixedUpdate() {
            Animate(Time.fixedDeltaTime);
        }

        private void Animate(float dt) {
            // Update the ticks.
            m_Ticks += dt;
            int previousFrame = m_CurrentFrame;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * m_FrameRate) % m_Animation.Length;

            // Change the frame.
            if (previousFrame != m_CurrentFrame) {
                m_SpriteRenderer.sprite = m_Animation[m_CurrentFrame];
            }

        }

    }

}
