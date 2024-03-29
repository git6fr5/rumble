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

namespace Platformer.Character {

    ///<summary>
    /// Animates the character.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
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

        /* --- Constants --- */

        // The frame rate that the animation plays at.
        public const float FRAME_RATE = 12;

        // The factor by which this stretches.
        public const float STRETCH_FACTOR = 2.5f/2f;

        // The character attached to the parent of this object.
        private CharacterController m_Character = null;

        // The sprite renderer attached to this object.
        private SpriteRenderer m_SpriteRenderer = null;        

        [SerializeField]
        private AudioVisualEffectCollection m_AudioVisualEffectCollection;

        // Okay. Lets see if this works.
        [HideInInspector]
        private Dictionary<AnimationPriority, Sprite[]> m_Spritesheet = new Dictionary<AnimationPriority, Sprite[]>();

        // The sprites this is currently animating through.
        [SerializeField]
        private Sprite[] m_CurrentAnimation = null;

        // The sprite we are currently on.
        [SerializeField, ReadOnly]
        private int m_CurrentFrame = 0;

        // The ticks until the next frame.
        [SerializeField, ReadOnly]
        private float m_Ticks = 0f;

        // The amount this character was stretched last frame.
        [SerializeField, ReadOnly]
        private Vector2 m_CachedStretch = new Vector2(0f, 0f);

        // Runs once before the first frame.
        void Start() {
            m_Character = transform.parent.GetComponent<CharacterController>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Runs once every frame.
        void Update() {
            // Scale(Time.deltaTime);
            Animate(Time.deltaTime);
            Rotate();
        }

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        private void Animate(float dt) {
            // Guard clause to protect from animating with no sprites.
            Sprite[] previousAnimation = m_CurrentAnimation;
            m_CurrentAnimation = GetHighestPriorityAnimation();
            if (!m_CurrentAnimation.Validate<Sprite>()) { return; }

            m_Ticks = previousAnimation == m_CurrentAnimation ? m_Ticks + dt : 0f;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * FRAME_RATE) % m_CurrentAnimation.Length;
            m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];

        }

        public void PlayAudioVisualEffect(VisualEffect visualEffect, AudioSnippet audioSnippet) {
            if (visualEffect != null) {
                visualEffect.Play();
            }
            if (audioSnippet != null && audioSnippet.clip != null) {
                audioSnippet.Play();
            }
        }

        public void PlayAudioVisualEffect(string name) {
            // if (visualEffect != null) {
            //     visualEffect.Play();
            // }
            // if (audioSnippet != null && audioSnippet.clip != null) {
            //     audioSnippet.Play();
            // }
            // m_AudioVisualEffectCollection.Play(transform, name);
        }

        public Sprite[] GetHighestPriorityAnimation() {
            return m_Spritesheet[m_Spritesheet.Keys.Max()];
        }

        public void Push(Sprite[] animation, AnimationPriority priority) {
            if (animation == null || animation.Length == 0) { return; }

            if (m_Spritesheet.ContainsKey(priority)) {
                m_Spritesheet[priority] = animation;
            }
            else {
                m_Spritesheet.Add(priority, animation);
            }
        }

        public void Remove(Sprite[] animation) {
            if (animation == null) { return; }

            List<KeyValuePair<AnimationPriority, Sprite[]>> kvp = m_Spritesheet.Where(kv => kv.Value == animation).ToList();
            foreach(KeyValuePair<AnimationPriority, Sprite[]> kv in kvp) {
                m_Spritesheet.Remove(kv.Key);
            }
        }

        private void Rotate() {
            float currentAngle = transform.eulerAngles.y;
            float angle = m_Character.FacingDirection < 0f ? 180f : m_Character.FacingDirection > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        private void Scale(float dt) {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Vector2 stretch = Vector2.zero;
            if (m_Character.Rising || m_Character.Falling) {
                float x = Mathf.Abs(m_Character.Body.velocity.x) * STRETCH_FACTOR * dt;
                float y = Mathf.Abs(m_Character.Body.velocity.y) * STRETCH_FACTOR * dt;
                stretch = new Vector2((x - y) / 2f, y - x);
                transform.localScale += (Vector3)(stretch + m_CachedStretch);
            }
            m_CachedStretch = stretch;
        }

    }

}


