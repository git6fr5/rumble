/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
using UnityEngine.Rendering.Universal;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class DecorationPiece : MonoBehaviour {

        // The idle transform movement.
        [SerializeField]
        private TransformAnimation m_IdleTransformMovement = null;
        public TransformAnimation Animation {
            get { return m_IdleTransformMovement; }
            set { m_IdleTransformMovement = value; }
        }

        // The sprite renderer component attached to this.        
        private SpriteRenderer m_SpriteRenderer = null;
        public SpriteRenderer spriteRenderer => m_SpriteRenderer;

        [SerializeField]
        private Light2D m_Light;

        [SerializeField]
        private float m_LightTicks;

        [SerializeField]
        private float m_LightDuration;

        public float baseLightIntensity;
        
        [SerializeField]
        private AnimationCurve m_LightInnerRadius;

        [SerializeField]
        private AnimationCurve  m_LightOuterRadius;

        [SerializeField]
        private AnimationCurve m_LightIntensity;

        // Runs once on instantiation.
        void Awake() {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            // SpriteShapeRenderer[] spriteShapeRenderers = GetComponentsInChildren<SpriteShapeRenderer>();

        }

        void FixedUpdate() {
            m_SpriteRenderer.transform.Animate(m_IdleTransformMovement, Time.fixedDeltaTime);

            // m_LightTicks += Time.fixedDeltaTime;

            // float ratio = (m_LightTicks / m_LightDuration) % 1f;

            // m_Light.intensity = baseLightIntensity + intensityScale * m_LightIntensity.Evaluate(ratio);
            // m_Light.pointLightInnerRadius = baseInner + innerScale * m_LightInnerRadius.Evaluate(ratio); 
            // m_Light.pointLightOuterRadius = baseOuter + outerScale * m_LightOuterRadius.Evaluate(ratio); 

        }

        public float baseInner;
        public float baseOuter;

        public float intensityScale;
        public float innerScale;
        public float outerScale;

    }

}
