/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Blocks;

namespace Platformer.Objects.Blocks {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(BlockObject)), RequireComponent(typeof(SpriteRenderer))]
    public class BlockAnimator : MonoBehaviour {

        #region Variables.

        /* --- Components --- */
        
        // The sprite renderer attached to this component.
        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();

        // The block attached to this component.
        private BlockObject m_Block => GetComponent<BlockObject>();

        /* --- Members --- */

        // Caches whether the block was active or not.
        [SerializeField, ReadOnly]
        private bool m_CachedActive = false;

        // The sprites when this block is active.
        [SerializeField] 
        private Sprite[] m_ActiveSprites;

        // The sprites when the block is inactive.
        [SerializeField] 
        private Sprite[] m_InactiveSprites;
        
        // The duration the animation lasts for.
        [SerializeField] 
        private float m_BaseFrameRate = 8f;

        // The duration the animation lasts for.
        [SerializeField] 
        private float m_FrameRateVariation = 0.25f;
        
        // The timer that controls when to move to the next frame.
        [HideInInspector]
        private Timer m_AnimationTimer = new Timer(0f, 0f);

        // The spritesheet currently being used.
        private Sprite[] CurrentSprites => m_Block.Active ? m_ActiveSprites : m_InactiveSprites;

        // A random frame from the current sprites.
        private Sprite RandomFrame => CurrentSprites[Random.Range(0, CurrentSprites.Length)];

        // The maximum duration a frame can last for.
        private float MaxFrameDuration => 1f / ((1f - m_FrameRateVariation) * m_BaseFrameRate);
        
        // The minimum duration a frame can last for.
        private float MinFrameDuration => 1f / ((1f + m_FrameRateVariation) * m_BaseFrameRate);

        // A random frame duration using the min and max.
        private float RandomFrameDuration => Random.Range(m_FrameRateVariation, MaxFrameDuration);

        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            m_AnimationTimer.Start(RandomFrameDuration);
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            // Swap sprites if changing states.
            if (m_CachedActive != m_Block.Active) {
                m_SpriteRenderer.sprite = RandomFrame;
                m_AnimationTimer.Start(RandomFrameDuration);
            }
            // Grab a new random frame when the timer hits 0.
            bool finished = m_AnimationTimer.TickDown(Time.fixedDeltaTime);
            if (finished) {
                m_SpriteRenderer.sprite = RandomFrame;
                m_AnimationTimer.Start(RandomFrameDuration);
            }
        }

        #endregion.

    }

}