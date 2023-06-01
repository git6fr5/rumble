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
    public class AnimationEvent {
        public string eventName;
        public int priority;
        public float ticks;
        public float duration;
        public bool loop;
        public bool bypassTransform;
        public TransformAnimation transformAnimation;
        public bool bypassSprite;
        public SpriteAnimation spriteAnimation;
        public bool bypassLight;
        public LightAnimation lightAnimation;
    }

    ///<summary>
    ///
    ///<summary>
    public class Animator : MonoBehaviour {

        // The idle light animation.
        [SerializeField]
        private AnimationEvent[] m_Events;

        // The current animations.
        private AnimationEvent[] m_CurrentAnimations;

        [SerializeField]
        private bool m_Animate;

        [SerializeField]
        private Transform m_Transform;
        private bool m_AnimateTransform;

        [SerializeField]
        private SpriteRenderer m_SpriteRenderer;
        private bool m_AnimateSprite;

        [SerializeField]
        private Light2D m_Light;
        private bool m_AnimateLight;

        void Start() {
            m_AnimateTransform = m_Transform != null;
            m_AnimateSprite = m_SpriteRenderer != null;    
            m_AnimateLight = m_Light != null;

            int maxPriority = 1;
            for (int i = 0; i < m_Events.Length; i++) {
                if (m_Events[i].priority + 1 > maxPriority) {
                    maxPriority = m_Events[i].priority + 1;
                }
            }
            m_CurrentAnimations = new AnimationEvent[maxPriority];

        }

        public void PushAnimation(string eventName, bool loop, float duration = -1f) {
            for (int i = 0; i < m_Events.Length; i++) {
                if (m_Events[i].eventName == eventName) {
                    
                    if (duration == -1f) {
                        m_Events[i].transformAnimation.duration = m_Events[i].duration;
                        if (m_Events[i].spriteAnimation.sprites != null && m_Events[i].spriteAnimation.sprites.Length > 0) {
                            m_Events[i].spriteAnimation.fps = m_Events[i].duration / m_Events[i].spriteAnimation.sprites.Length;
                        }
                        m_Events[i].lightAnimation.duration = m_Events[i].duration;
                    }
                    else {
                        m_Events[i].transformAnimation.duration = duration;
                        if (m_Events[i].spriteAnimation.sprites != null && m_Events[i].spriteAnimation.sprites.Length > 0) {
                            m_Events[i].spriteAnimation.fps = duration / m_Events[i].spriteAnimation.sprites.Length;
                        }
                        m_Events[i].lightAnimation.duration = duration;
                    }

                    m_Events[i].loop = loop;
                    m_Events[i].ticks = !loop ? duration == -1f ? m_Events[i].duration : duration : -1f; 
                    m_CurrentAnimations[m_Events[i].priority] = m_Events[i];


                }
            }
        }

        public void RemoveAnimation(string eventName) {
            for (int i = 0; i < m_CurrentAnimations.Length; i++) {
                if (m_CurrentAnimations[i] != null && m_CurrentAnimations[i].eventName == eventName) {
                    m_CurrentAnimations[i] = null;
                }
            }
        }

        private (TransformAnimation, SpriteAnimation, LightAnimation) GetCurrentAnimations() {
            TransformAnimation transformAnim = null;
            SpriteAnimation spriteAnim = null;
            LightAnimation lightAnim = null;

            bool gotSprite = !m_AnimateSprite;
            bool gotTransform = !m_AnimateTransform;
            bool gotLight = !m_AnimateLight;

            for (int i = m_CurrentAnimations.Length - 1; i >= 0; i -= 1) {
                if (m_CurrentAnimations[i] != null) {
                    

                    if (!gotTransform && !m_CurrentAnimations[i].bypassTransform) {
                        transformAnim = m_CurrentAnimations[i].transformAnimation;
                    }
                    if (!gotSprite && !m_CurrentAnimations[i].bypassSprite) {
                        spriteAnim = m_CurrentAnimations[i].spriteAnimation;
                    }
                    if (!gotLight && !m_CurrentAnimations[i].bypassLight) {
                        lightAnim = m_CurrentAnimations[i].lightAnimation;
                    }
                    
                    gotSprite = !m_AnimateSprite || transformAnim != null;
                    gotTransform = !m_AnimateTransform || spriteAnim != null;
                    gotLight = !m_AnimateLight || lightAnim != null;
                    
                    if (!m_CurrentAnimations[i].loop) {
                        m_CurrentAnimations[i].ticks -= Time.fixedDeltaTime;
                        if (m_CurrentAnimations[i].ticks <= 0f) {
                            RemoveAnimation(m_CurrentAnimations[i].eventName);
                        } 
                    }
                    
                    if (gotSprite && gotTransform && gotLight) {
                        return (transformAnim, spriteAnim, lightAnim);
                    }
                }
            }
            return (transformAnim, spriteAnim, lightAnim);
	    }

        void FixedUpdate() {
            if (!m_Animate) { return; }
            Animate(Time.fixedDeltaTime);
        }

        void Animate(float dt) {
            (TransformAnimation, SpriteAnimation, LightAnimation) animations = GetCurrentAnimations(); 
            if (m_AnimateTransform && animations.Item1 != null) { TransformAnimator.Animate(m_Transform, animations.Item1, dt); }
            if (m_AnimateSprite && animations.Item2 != null) { SpriteAnimator.Animate(m_SpriteRenderer, animations.Item2, dt); }
            if (m_AnimateLight && animations.Item3 != null) { LightAnimator.Animate(m_Light, animations.Item3, dt); }
        }

    }

}
