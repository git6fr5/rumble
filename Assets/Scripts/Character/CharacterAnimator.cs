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
            // public VisualEffect visualEffect;
            // public AudioSnippet audioSnippet;
        }

        private UnityAnimator m_Animator = null;

        private CharacterController m_Character = null;

        [SerializeField]
        private AudioVisualEffectCollection m_AudioVisualEffectCollection;

        [SerializeField]
        private List<AnimationItem> m_AnimationCollection = new List<AnimationItem>();

        [SerializeField]
        private UnityAnimation[] m_AnimationSheet;

        // Runs once before the first frame.
        void Start() {
            m_Animator = GetComponent<UnityAnimator>();
            m_Character = transform.parent.GetComponent<CharacterController>();
            m_AnimationSheet = new UnityAnimation[(int)AnimationPriority.Count];
            // m_Animator.StartPlayback();
        }

        // Runs once every frame.
        void Update() {
            Animate(Time.deltaTime);
            Rotate();
        }

        UnityAnimation prevAnim;

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        private void Animate(float dt) {
            // Animator.
            UnityAnimation animation = GetHighestPriorityAnimation();
            print(animation.name);

            if (animation != prevAnim) {
                m_Ticks = 0f;
                prevAnim = animation;
            }
            
            if (animation == null) {
                return;
            }

            m_Ticks += (1f + m_Character.Body.velocity.magnitude / 5f) * dt;

            float length = animation.length; // m_Animator.GetCurrentAnimatorClipInfo(_cacheLayer)[0].clip.length;
            print(length);
            if (m_Ticks >= length) {
                m_Ticks -= length;
            }

            animation.SampleAnimation(gameObject, m_Ticks);

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
            string[] x = name.Split(".");
            if (x.Length >= 2) {
                print(x[0]);
                print(x[1]);
                PlayAnimation(x[0], x[1]);
            }
        }

        float m_Ticks = 0f;
        int _cacheHash;
        int _cacheLayer;

        public void PlayAnimation(string layer, string name) {
            // int stateHash = UnityAnimator.StringToHash(name);
            // int layerIndex = m_Animator.GetLayerIndex(layer);
            // if (m_Animator.HasState(layerIndex, stateHash)) {
            //     // m_Animator.CrossFade(name, 0.5f, -1, float.NegativeInfinity, 0.0f);
            //     m_Animator.Play(stateHash, layerIndex);
            //     _cacheHash = stateHash;
            //     _cacheLayer = layerIndex;
            // }
            
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null) {
                m_AnimationSheet[(int)anim.priority] = anim.animation;
            }
        }

        public void StopAnimation(string name) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null && m_AnimationSheet[(int)anim.priority] == anim.animation) {
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

        public UnityAnimation GetHighestPriorityAnimation() {
            for (int i = m_AnimationSheet.Length - 1; i >= 0; i--) {
                if (m_AnimationSheet[i] != null) {
                    return m_AnimationSheet[i];
                }
            }
            return m_AnimationSheet[0];
        }

    }

}


