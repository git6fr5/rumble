/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

namespace Platformer.Visuals.Animation {

    [System.Serializable]
    public class TransformAnimation {
        
        [Header("Animation Parameters")]
        public float ticks;
        public float duration;
        public bool loop;
        private float t => ticks / duration;

        [Header("Position")]
        public Vector2 basePos;
        public Vector2 posScale;
        public AnimationCurve horPosCurve;
        public AnimationCurve vertPosCurve;

        [Header("Stretch")]
        public Vector2 baseStretch;
        public Vector2 strectchScale;
        public AnimationCurve horStretchCurve;
        public AnimationCurve vertStretchCurve;

        [Header("Rotation")]
        public float baseRotation;
        public float rotationScale;
        public AnimationCurve rotationCurve;

        public bool Tick(float dt) {
            ticks += Time.fixedDeltaTime;
            if (ticks > duration) {
                if (loop) {
                    ticks -= duration;
                }
                else {
                    ticks = 0f;
                    return false;
                }
            }
            return true;
        }

        public void Set(float t) {
            ticks = t;
            if (ticks > duration) {
                ticks = ticks % duration;
            }
        }

        public Vector3 GetPosition() {
            return basePos + new Vector2(posScale.x * horPosCurve.Evaluate(t), posScale.y * vertPosCurve.Evaluate(t));
        }

        public Vector3 GetStretch() {
            return baseStretch + new Vector2(strectchScale.x * horStretchCurve.Evaluate(t), strectchScale.y * vertStretchCurve.Evaluate(t));
        }

        public Quaternion GetRotation() {
            return Quaternion.Euler(0f, 0f, baseRotation + rotationScale * rotationCurve.Evaluate(t));
        }

    }

    public class TransformAnimator : MonoBehaviour {

        // The idle transform animation.
        [SerializeField]
        private TransformAnimation m_Animation;
        public TransformAnimation Animation => m_Animation;

        [SerializeField]
        private bool m_Animate = true;

        public void Play() {
            m_Animate = true;
        }

        void FixedUpdate() {
            if (!m_Animate) { return; }
            Animate(Time.fixedDeltaTime);
        }

        public void Animate(float dt) {
            m_Animate = m_Animation.Tick(dt);
            transform.localPosition = m_Animation.GetPosition();
            transform.localRotation = m_Animation.GetRotation();
            transform.localScale = m_Animation.GetStretch();
        }

        public static void Animate(Transform transform, TransformAnimation animation, float dt) {
            animation.Tick(dt);
            transform.localPosition = animation.GetPosition();
            transform.localRotation = animation.GetRotation();
            transform.localScale = animation.GetStretch();
        }

    }

}