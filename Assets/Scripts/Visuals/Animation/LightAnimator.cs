/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
using UnityEngine.Rendering.Universal;

namespace Platformer.Visuals.Animation {

    [System.Serializable]
    public class LightAnimation {
        
        [Header("Animation Parameters")]
        public float ticks;
        public float duration;
        private float t => ticks / duration;

        [Header("Color")]
        public Gradient colorGradient;

        [Header("Intensity")]
        public float baseIntensity;
        public float intensityScale;
        public AnimationCurve intensityCurve;
        
        [Header("Inner Radius")]
        public float baseInnerRadius;
        public float innerRadiusScale;
        public AnimationCurve innerRadiusCurve;

        [Header("Outer Radius")]
        public float baseOuterRadius;
        public float outerRadiusScale;
        public AnimationCurve outerRadiusCurve;

        public void Tick(float dt) {
            ticks += Time.fixedDeltaTime;
            if (ticks > duration) {
                ticks -= duration;
            }
        }

        public Color GetColor() {
            return colorGradient.Evaluate(t);
        }

        public float GetIntensity() {
            return baseIntensity + intensityScale * intensityCurve.Evaluate(t);
        }
        
        public float GetInnerRadius() {
            return baseInnerRadius + innerRadiusScale * innerRadiusCurve.Evaluate(t);
        }

        public float GetOuterRadius() {
            return baseOuterRadius + outerRadiusScale * outerRadiusCurve.Evaluate(t);
        }

    }

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Light2D))]
    public class LightAnimator : MonoBehaviour {

        // The idle light animation.
        [SerializeField]
        private LightAnimation m_Animation;

        [SerializeField]
        private bool m_Animate;

        // The light attacted to this.
        private Light2D m_Light;

        // Runs once on instantiation.
        void Start() {
            m_Light = GetComponent<Light2D>();
        }

        void FixedUpdate() {
            if (!m_Animate) { return; }
            Animate(Time.fixedDeltaTime);
        }

        public void Animate(float dt) {
            m_Animation.Tick(dt);
            m_Light.color = m_Animation.GetColor();
            m_Light.intensity = m_Animation.GetIntensity();
            m_Light.pointLightInnerRadius = m_Animation.GetInnerRadius(); 
            m_Light.pointLightOuterRadius = m_Animation.GetOuterRadius();
        }

        public static void Animate(Light2D light, LightAnimation animation, float dt) {
            animation.Tick(dt);
            light.color = animation.GetColor();
            light.intensity = animation.GetIntensity();
            light.pointLightInnerRadius = animation.GetInnerRadius(); 
            light.pointLightOuterRadius = animation.GetOuterRadius();
        }

    }

}
