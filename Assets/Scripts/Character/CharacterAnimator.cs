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

        /* --- Constants --- */

        // The frame rate that the animation plays at.
        public const float FRAME_RATE = 12;

        // The factor by which this stretches.
        public const float STRETCH_FACTOR = 2.5f/2f;

        // The character attached to the parent of this object.
        private CharacterController m_Character = null;

        // The sprite renderer attached to this object.
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer = null;

        [SerializeField]
        private SpriteRenderer m_Foreleg = null;

        [SerializeField]
        private SpriteRenderer m_Backleg = null;

        [SerializeField]
        private SpriteRenderer m_Body;        

        [SerializeField]
        private AudioVisualEffectCollection m_AudioVisualEffectCollection;

        // Okay. Lets see if this works.
        [HideInInspector]
        private SpriteAnimation[] m_Spritesheet;

        // The sprites this is currently animating through.
        [SerializeField]
        private SpriteAnimation m_CurrentAnimation = null;

        [SerializeField]
        private float m_Tiredness = 0f;

        // The amount this character was stretched last frame.
        [SerializeField, ReadOnly]
        private Vector2 m_CachedStretch = new Vector2(0f, 0f);

        [SerializeField]
        private Gobblefish.Animation.TransformAnimator m_TirednessAnimator;

        // Runs once before the first frame.
        void Start() {
            m_Character = transform.parent.GetComponent<CharacterController>();
            // m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Spritesheet = new SpriteAnimation[(int)AnimationPriority.Count];
        }

        // Runs once every frame.
        void Update() {
            Scale(Time.deltaTime);
            Animate(Time.deltaTime);
            Rotate();
            SetTiredness(Time.deltaTime);
        }

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        private void Animate(float dt) {
            // Guard clause to protect from animating with no sprites.
            // SpriteAnimation previousAnimation = m_CurrentAnimation;
            m_CurrentAnimation = GetHighestPriorityAnimation();

            // if (!m_CurrentAnimation.Validate<Sprite>()) { return; }

            // m_Ticks = previousAnimation == m_CurrentAnimation ? m_Ticks + dt : 0f;
            // m_CurrentFrame = (int)Mathf.Floor(m_Ticks * FRAME_RATE) % m_CurrentAnimation.Length;
            m_CurrentAnimation.Tick(dt);

            int halfNextFrame = m_CurrentAnimation.currentFrame;
            if (m_CurrentAnimation.sprites.Length > 0) {
                halfNextFrame += (int)Mathf.Ceil(m_CurrentAnimation.sprites.Length / 2f);
                halfNextFrame = halfNextFrame % m_CurrentAnimation.sprites.Length;
            }
            else {
                return;
            }

            m_Foreleg.sprite =  m_CurrentAnimation.GetFrame();
            m_Backleg.sprite = m_CurrentAnimation.sprites[halfNextFrame];

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

        public void SetBody(Sprite sprite) {
            m_Body.sprite = sprite;
        }

        public SpriteAnimation GetHighestPriorityAnimation() {
            for (int i = m_Spritesheet.Length - 1; i >= 0; i--) {
                if (m_Spritesheet[i] != null) {
                    return m_Spritesheet[i];
                }
            }
            return m_Spritesheet[0];
        }

        public void Push(SpriteAnimation animation, AnimationPriority priority) {
            if (animation == null) { return; }
            m_Spritesheet[(int)priority] = animation;
        }

        public void Remove(SpriteAnimation animation) {
            if (animation == null) { return; }

            for (int i = m_Spritesheet.Length - 1; i >= 0; i--) {
                if (m_Spritesheet[i] == animation) {
                    m_Spritesheet[i] = null;
                }
            }
        }

        private void Rotate() {
            float currentAngle = transform.eulerAngles.y;
            float angle = m_Character.FacingDirection < 0f ? 180f : m_Character.FacingDirection > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        public void RotateBody(float angle) {
            Quaternion q = Quaternion.Euler(0f, 0f, angle);
            m_Body.transform.localRotation = q;
            m_Foreleg.transform.localRotation = q;
            m_Backleg.transform.localRotation = q;
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

        const float BASE_TIREDNESS = 3f;
        const float MAX_TIREDNESS = 2.8f;

        const float MIN_MOVING_FACTOR = 1f;

        const float PERCENT_TIREDNESS_GAINED_IN_A_SECOND = 0.15f;
        const float PERCENT_TIREDNESS_LOST_IN_A_SECOND = 0.5f;

        private void SetTiredness(float dt) {
            if (m_Character.Body.velocity.sqrMagnitude > 0.2f) {
                if (m_Tiredness < MIN_MOVING_FACTOR) {
                    m_Tiredness = MIN_MOVING_FACTOR;
                }
                m_Tiredness += PERCENT_TIREDNESS_GAINED_IN_A_SECOND * dt;
            }
            else {
                m_Tiredness -= PERCENT_TIREDNESS_LOST_IN_A_SECOND * dt;
            }

            m_Tiredness = m_Tiredness < 0f ? 0f : m_Tiredness > 1f ? 1f : m_Tiredness;

            m_TirednessAnimator.SetDuration(BASE_TIREDNESS - MAX_TIREDNESS * m_Tiredness);

        }

    }

}


