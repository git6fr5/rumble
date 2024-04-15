// TODO: Clean
/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish;
using Gobblefish.Audio;
using Gobblefish.Animation;

using UnityAnimator = UnityEngine.Animator;
using UnityAnimation = UnityEngine.AnimationClip;

namespace Platformer.Character {

    ///<summary>
    /// Animates the character.
    ///<summary>
    // [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterAnimator : MonoBehaviour {

        public enum AnimationPriority {
            // Default.
            DefaultIdle,
            DefaultMoving,
            DefaultJumpRising,
            DefaultJumpFalling,
            // Action Passive.
            ActionPassiveIdle,
            ActionPassiveMoving,
            ActionPassiveRising,
            ActionPassiveFalling,
            // Action Active
            ActionPreActive,
            ActionActive,
            ActionPostActive,
            Count,
        }

        [System.Serializable]
        protected class AnimationItem {
            public string name;
            public AnimationPriority priority;
            public UnityAnimation animation;
            public float playbackSpeed = 1f;
            public Vector2 playbackRange = new Vector2(0f, 1f);
            public bool loop = true;

            public float minTicks => animation == null ? 0f : playbackRange.x * animation.length;
            public float maxTicks => animation == null ? 0f : playbackRange.y * animation.length;

            // public bool pauseAtEnd;
            // public VisualEffect visualEffect;
            // public AudioSnippet audioSnippet;
        }

        private UnityAnimator m_Animator = null;

        private CharacterController m_Character = null;

        [SerializeField]
        private AudioVisualEffectCollection m_AudioVisualEffectCollection;

        [SerializeField]
        private UnityAnimation m_ResetAnimation;

        [SerializeField]
        private List<AnimationItem> m_AnimationCollection = new List<AnimationItem>();

        private AnimationItem[] m_AnimationSheet;

        // Runs once before the first frame.
        void Start() {
            m_Animator = GetComponent<UnityAnimator>();
            m_Character = transform.parent.GetComponent<CharacterController>();
            m_AnimationSheet = new AnimationItem[(int)AnimationPriority.Count];
            // m_Animator.StartPlayback();
        }

        // Runs once every frame.
        void Update() {
            Animate(Time.deltaTime);
            Rotate();
        }

        float ticks = 0f;
        AnimationItem prevAnim;

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        private void Animate(float dt) {
            // Animator.
            AnimationItem anim = GetHighestPriorityAnimation();
            print(anim.name);

            if (anim == null) {
                return;
            }

            if (anim != prevAnim) {
                ticks = anim.minTicks;
                prevAnim = anim;
                m_ResetAnimation.SampleAnimation(gameObject, ticks);
            }

            ticks += anim.playbackSpeed * dt;
            if (ticks >= anim.maxTicks) {
                if (anim.loop) {
                    ticks -= anim.maxTicks;
                }
                else {
                    ticks = anim.maxTicks;
                }
            }

            anim.animation.SampleAnimation(gameObject, ticks);

            // m_Animator.playbackTime	= m_Ticks; // 
            // m_Animator.PlayInFixedTime(_cacheHash, _cacheLayer, m_Ticks);
            // animatePhysics	
            // print(m_Animator.GetCurrentAnimatorStateInfo(_cacheLayer).loop);
            
        }

        private void Rotate() {
            float currentAngle = transform.eulerAngles.y;
            float angle = m_Character.FacingDirection < 0f ? 180f : m_Character.FacingDirection > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        public void PlayAnimation(string name) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null) {
                m_AnimationSheet[(int)anim.priority] = anim;
            }
        }

        public void StopAnimation(string name) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null && m_AnimationSheet[(int)anim.priority] == anim) {
                m_AnimationSheet[(int)anim.priority] = null;
            }
        }

        public void PlayEffect(string name) {

        }

        public void StopEffect(string name) {

        }

        public void PlayAudioVisualEffect(VisualEffect visualEffect, AudioSnippet audioSnippet) {
            if (visualEffect != null) {
                visualEffect.Play();
            }
            if (audioSnippet != null && audioSnippet.clip != null) {
                audioSnippet.Play();
            }
        }

        private AnimationItem GetHighestPriorityAnimation() {
            for (int i = m_AnimationSheet.Length - 1; i >= 0; i--) {
                if (m_AnimationSheet[i] != null) {
                    return m_AnimationSheet[i];
                }
            }
            return m_AnimationSheet[0];
        }

    }

}


