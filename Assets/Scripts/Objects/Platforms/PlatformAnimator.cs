/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Platforms {

    [System.Serializable]
    public class PlatformSpriteData {
        public SpriteShape Default;
        public Sprite Left;
        public Sprite Right;
        public Sprite Single;
    }

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class PlatformAnimator : MonoBehaviour {

        #region Variables.
        
        public const float RELEASED_OFFSET = 1f/16f;

        /* --- Components --- */
        
        // The sprite shape renderer attached to this platform.
        [SerializeField]
        protected SpriteShapeRenderer m_SpriteShapeRenderer = null;
        
        // The sprite shape controller. attached to this platform.
        [SerializeField]
        protected SpriteShapeController m_SpriteShapeController = null;

        // The spline attached to the sprite shape.
        protected Spline m_Spline = null;
        public Spline spline => m_SpriteShapeController.spline;

        // The current animation being played.
        protected TransformAnimation m_CurrentAnimation;

        // The animation that plays when the platform is being pressed.
        [SerializeField]
        private TransformAnimation m_OnPressedAnimation;

        #endregion

        #region Methods.

        // Rendering settings.
        public void Initialize(PlatformObject platform) {
            // Cache components.
            if (m_SpriteShapeRenderer != null) {
                m_Spline = m_SpriteShapeController.spline;
                // m_SpriteShapeRenderer.sortingLayerName = Game.Visuals.RenderingLayers.PlatformLayer;
                // m_SpriteShapeRenderer.sortingOrder = Game.Visuals.RenderingLayers.PlatformOrder;
                // m_SpriteShapeRenderer.enabled = false;
            }

            // Release the platform as the default state.
            m_CurrentAnimation = null;
            transform.Reset();
            OnReleased();
        }

        void FixedUpdate() {
            if (m_CurrentAnimation != null) {
                transform.Animate(m_CurrentAnimation, Time.fixedDeltaTime);
                if (m_CurrentAnimation.AnimationTimer.Ratio >= 1f) {
                    m_CurrentAnimation = null;
                    transform.Reset();
                }
            }

        }

        public void SetMaterialValue(string var, float val) {

            if (m_SpriteShapeRenderer != null) {
                for (int i = 0; i < m_SpriteShapeRenderer.materials.Length; i++) {
                    if (m_SpriteShapeRenderer.materials[i] != null) {
                        m_SpriteShapeRenderer.materials[i].SetFloat(var, val);
                    }
                }
                return;
            }

        }

        // If the platform was just pressed.
        public void OnPressed() {
            m_CurrentAnimation = m_OnPressedAnimation;
            m_CurrentAnimation.AnimationTimer.Set(0f);
        }

        // If the platform was just released.
        public void OnReleased() {
            // m_SpriteShapeController.transform.localPosition = new Vector3(0f, RELEASED_OFFSET, 0f); 
        }

        #endregion

    }
}
