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
    public class PlatformVisualPacket {

        public SpriteShape Default;
        public Sprite Single;
        public Sprite Left;
        public Sprite Right;

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

        [SerializeField]
        private PlatformVisualPacket m_VisualPacket;
        public PlatformVisualPacket defaultPacket => m_VisualPacket;

        // The sprite shape controller. attached to this platform.
        [SerializeField]
        protected SpriteRenderer[] m_SpriteRenderers;

        // The spline attached to the sprite shape.
        protected Spline m_Spline = null;

        //
        protected TransformAnimation m_CurrentAnimation;

        [SerializeField]
        private TransformAnimation m_OnPressedAnimation;

        #endregion

        #region Methods.

        // Rendering settings.
        public void Initialize(PlatformObject platform) {
            // Cache components.
            if (m_SpriteShapeRenderer != null) {
                m_Spline = m_SpriteShapeController.spline;
                m_SpriteShapeRenderer.sortingLayerName = Game.Visuals.RenderingLayers.PlatformLayer;
                m_SpriteShapeRenderer.sortingOrder = Game.Visuals.RenderingLayers.PlatformOrder;
                // m_SpriteShapeRenderer.enabled = false;
            }
            if (m_SpriteRenderers != null && m_SpriteRenderers.Length > 0) {
                for (int i = 0; i < m_SpriteRenderers.Length; i++) {
                    m_SpriteRenderers[i].sortingLayerName = Game.Visuals.RenderingLayers.PlatformLayer;
                    m_SpriteRenderers[i].sortingOrder = Game.Visuals.RenderingLayers.PlatformOrder;
                }
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

        // Set the controls from the LDtk files.
        public void SetLength(int length) {
            if (length == 1) {

                m_SpriteRenderers = new SpriteRenderer[1];
                m_SpriteRenderers[0] = new GameObject("sprite_renderer", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                m_SpriteRenderers[0].sprite = m_VisualPacket.Single;
                m_SpriteRenderers[0].transform.SetParent(transform);

                m_SpriteRenderers[0].sharedMaterial = m_SpriteShapeRenderer.sharedMaterial;

                // m_SpriteRenderers[0].transform.localScale = new Vector3(0.9f, 1f, 1f);
                m_SpriteRenderers[0].transform.localPosition = Vector3.zero;

                Destroy(m_SpriteShapeRenderer.gameObject);
                m_SpriteShapeRenderer = null;
                m_SpriteShapeController = null;
                return;

            }
            else if (length == 2) {

                m_SpriteRenderers = new SpriteRenderer[2];
                m_SpriteRenderers[0] = new GameObject("left_sprite_renderer", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                m_SpriteRenderers[0].sprite = m_VisualPacket.Left;
                m_SpriteRenderers[1] = new GameObject("right_sprite_renderer", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                m_SpriteRenderers[1].sprite = m_VisualPacket.Right;

                m_SpriteRenderers[0].sharedMaterial = m_SpriteShapeRenderer.sharedMaterial;
                m_SpriteRenderers[1].sharedMaterial = m_SpriteShapeRenderer.sharedMaterial;

                m_SpriteRenderers[0].transform.SetParent(transform);
                m_SpriteRenderers[1].transform.SetParent(transform);

                m_SpriteRenderers[0].transform.localPosition = Vector3.zero;
                m_SpriteRenderers[1].transform.localPosition = Vector3.right;

                Destroy(m_SpriteShapeRenderer.gameObject);
                m_SpriteShapeRenderer = null;
                m_SpriteShapeController = null;

                return;
            }

            // float spriteLength = (baseLength - 2f) + (2f * capLength);
            // m_SpriteShapeController.transform.localScale = new Vector3((float)baseLength / spriteLength, 1f, 1f);
            // m_SpriteShapeController.transform.localPosition += 0.8f * Vector3.left; //  * (float)baseLength / spriteLength / 2f;

            m_SpriteShapeController.spriteShape = m_VisualPacket.Default;
            
            length -= 2;
            m_Spline.Clear();
            m_Spline.InsertPointAt(0, 0.5f * Vector3.right);
            m_Spline.InsertPointAt(1, (length + 0.5f) * Vector3.right);
            m_Spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            m_Spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        public void SetVisuals(PlatformVisualPacket visualPacket) {
            if (m_SpriteShapeRenderer != null) {
                m_SpriteShapeController.spriteShape = visualPacket.Default;
                return;
            }

            if (m_SpriteRenderers != null) {

                if (m_SpriteRenderers.Length == 1) {
                    m_SpriteRenderers[0].sprite = visualPacket.Single;
                }
                else if (m_SpriteRenderers.Length == 2) {
                    m_SpriteRenderers[0].sprite = visualPacket.Left;
                    m_SpriteRenderers[1].sprite = visualPacket.Right;
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

            if (m_SpriteRenderers != null) {

                if (m_SpriteRenderers.Length == 1) {
                    m_SpriteRenderers[0].material.SetFloat(var, val);
                }
                else if (m_SpriteRenderers.Length == 2) {
                    m_SpriteRenderers[0].material.SetFloat(var, val);
                    m_SpriteRenderers[1].sharedMaterial = m_SpriteRenderers[0].material;
                }

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
