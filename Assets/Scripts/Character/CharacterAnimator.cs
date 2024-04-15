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

        private CharacterController m_Character = null;

        [SerializeField]
        private AudioVisualEffectCollection m_AudioVisualEffectCollection;

        [SerializeField]
        private List<AnimationItem> m_AnimationCollection = new List<AnimationItem>();

        [SerializeField]
        private UnityAnimation[] m_AnimationSheet;

        // Runs once before the first frame.
        void Start() {
            m_Character = transform.parent.GetComponent<CharacterController>();
            m_AnimationSheet = new UnityAnimation[(int)AnimationPriority.Count];
        }

        // Runs once every frame.
        void Update() {
            Animate(Time.deltaTime);
            Rotate();
        }

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        private void Animate(float dt) {
            
        }

        private void Rotate() {
            float currentAngle = transform.eulerAngles.y;
            float angle = m_Character.FacingDirection < 0f ? 180f : m_Character.FacingDirection > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        public void PlayAnimation(string name) {
            // AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            // if (anim != null) {
            //     m_AnimationSheet[(int)anim.priority] = anim.animation;
            // }
        }

        public void StopAnimation(string name) {
            // AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            // if (anim != null && m_AnimationSheet[(int)anim.priority] == anim.animation) {
            //     m_AnimationSheet[(int)anim.priority] = null;
            // }
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


