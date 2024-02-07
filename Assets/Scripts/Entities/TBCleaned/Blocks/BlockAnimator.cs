// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityExtensions;
// // Platformer.
// using Platformer.Objects;
// using Platformer.Objects.Blocks;

// /* --- Definitions --- */
// using Game = Platformer.GameManager;

// namespace Platformer.Objects.Blocks {

//     ///<summary>
//     /// 
//     ///<summary>
//     [RequireComponent(typeof(SpriteRenderer))]
//     public class BlockAnimator : MonoBehaviour {

//         #region Variables.

//         /* --- Components --- */
        
//         // The sprite renderer attached to this component.
//         [SerializeField]
//         private SpriteRenderer m_SpriteRenderer = null;

//         // The block attached to the parent object.
//         private BlockObject m_Block = null;

//         /* --- Members --- */

//         // Caches whether the block was active or not.
//         [SerializeField, ReadOnly]
//         private bool m_CachedActive = false;

//         // The sprites when this block is active.
//         [SerializeField] 
//         private Sprite[] m_ActiveSprites;

//         // The sprites when the block is inactive.
//         [SerializeField] 
//         private Sprite[] m_InactiveSprites;
        
//         // The duration the animation lasts for.
//         [SerializeField] 
//         private float m_BaseFrameRate = 8f;

//         // The duration the animation lasts for.
//         [SerializeField] 
//         private float m_FrameRateVariation = 0.25f;
        
//         // The timer that controls when to move to the next frame.
//         private Timer m_AnimationTimer = new Timer(0f, 0f);

//         // The spritesheet currently being used.
//         private Sprite[] m_CurrentSprites = null;
        
//         // A random frame from the current sprites.
//         private Sprite RandomFrame => m_CurrentSprites[Random.Range(0, m_CurrentSprites.Length)];

//         // The maximum duration a frame can last for.
//         private float m_MaxFrameDuration = 1f;
        
//         // The minimum duration a frame can last for.
//         private float m_MinFrameDuration = 0f;

//         // A random frame duration using the min and max.
//         private float RandomFrameDuration => Random.Range(m_MinFrameDuration, m_MaxFrameDuration);

//         private TransformAnimation m_TransformAnimation = null;

//         #endregion

//         #region Methods.

//         // Runs once before the first frame.
//         public void Initialize(BlockObject block) {
//             // Cache these components.
//             m_Block = block;
            
//             // Start the timer.
//             m_AnimationTimer.Start(RandomFrameDuration);

//             // Set the sprite order.
//             m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.BlockLayer;
//             m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.BlockOrder;

//             // Calculate these values.
//             m_MaxFrameDuration = 1f / ((1f - m_FrameRateVariation) * m_BaseFrameRate);
//             m_MinFrameDuration = 1f / ((1f + m_FrameRateVariation) * m_BaseFrameRate);
//             m_CurrentSprites = m_InactiveSprites;
//             m_CachedActive = false;
            
//         }

//         // Runs once every fixed interval.
//         void FixedUpdate() {
//             // Swap sprites if changing states.
//             if (m_CachedActive != m_Block.Active) {
//                 OnStateChange();
//                 OnFrameChange();
//                 return;
//             }
//             // Grab a new random frame when the timer hits 0.
//             bool finished = m_AnimationTimer.TickDown(Time.fixedDeltaTime);
//             if (finished) {
//                 OnFrameChange();
//             }

//             if (m_TransformAnimation != null) {

//                 transform.Animate(m_TransformAnimation, Time.fixedDeltaTime);
                
//                 if (m_TransformAnimation.AnimationTimer.Ratio >= 1f) {
//                     m_TransformAnimation = null;
//                     transform.Reset();
//                 }
//             }

//         }

//         public void PlayTransformAnimation(TransformAnimation animation) {
//             if (m_TransformAnimation != animation) {
//                 m_TransformAnimation = animation;
//                 m_TransformAnimation.AnimationTimer = new Timer(0f, animation.AnimationTimer.MaxValue);
//             }
//         }

//         private void OnStateChange() {
//             m_CachedActive = m_Block.Active;
//             m_CurrentSprites = m_CachedActive ? m_ActiveSprites : m_InactiveSprites;
//         }

//         private void OnFrameChange() {
//             m_SpriteRenderer.sprite = RandomFrame;
//             m_AnimationTimer.Start(RandomFrameDuration);
//         }

//         #endregion.

//     }

// }