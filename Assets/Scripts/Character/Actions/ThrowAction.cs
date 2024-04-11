// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
using Gobblefish.Animation;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [CreateAssetMenu(fileName="ThrowAction", menuName ="Actions/Throw")]
    public class ThrowAction : CharacterAction {

        #region Variables.
        
        // The speed of the actual bounce.
        [SerializeField] 
        private float m_ThrowSpeed = 12f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_ChargeDuration = 0.5f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_PostbounceWeight = 1.5f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_ThrowDuration = 2f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_PostthrowDuration = 0.2f;

        [SerializeField]
        private Timer m_ChargeTimer = new Timer(0f, 0f);

        [SerializeField]
        private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);

        [SerializeField]
        private Timer m_ThrowTimer = new Timer(0f, 0f);

        // The direction the player was facing before the bounce started.
        [HideInInspector]
        protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_PrebounceAnimation = null;

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_BounceAnimation = null;

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_PostbounceAnimation = null;

        // The visual effect that plays at the start of the bounce.
        [SerializeField]
        protected VisualEffect m_StartBounceEffect;

        // The visual effect that plays at the start of the bounce.
        [SerializeField]
        protected AudioSnippet m_StartBounceSound;

        // The visual effect that plays at the start of the bounce.
        [SerializeField]
        protected VisualEffect m_EndBounceEffect;

        [SerializeField]
        private PhysicsMaterial2D m_BouncyMaterial;

        [SerializeField]
        private PhysicsMaterial2D m_DefaultMaterial;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private VisualEffect m_ChargeEffect = null;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private AudioSnippet m_ChargeSound = null;

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            m_ActionPhase = ActionPhase.None;
            OnEndBounce(character);

            if (!enable) {
                character.Animator.Remove(m_PrebounceAnimation);
                character.Animator.Remove(m_PostbounceAnimation);
                character.Animator.Remove(m_BounceAnimation);
            }

        }

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            if (character.Input.Actions[1].Pressed && m_Refreshed && m_ActionPhase == ActionPhase.None && !character.OnGround) {
                OnStartCharge(character);
                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }

            if (character.Input.Actions[1].Released && m_ActionPhase == ActionPhase.PreAction) {
                OnStartThrow(character);
                character.Input.Actions[1].ClearReleaseBuffer();
                m_Refreshed = false;
            }

        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && m_ActionPhase == ActionPhase.None ? true : m_Refreshed;

            // Tick down the bounce timer.
            bool finished = m_ThrowTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.MidAction:
                        OnStartPostbounce(character);
                        break;
                    case ActionPhase.PostAction:
                        OnEndBounce(character);
                        break;
                    default:
                        break;
                }

            }

            // Charge the hop.
            m_ChargeTimer.TickDown(dt);

            // If in a phase.
            switch (m_ActionPhase) {
                case ActionPhase.MidAction:
                    WhileBouncing(character, dt);
                    break;
                case ActionPhase.PostAction:
                    WhilePostbouncing(character, dt);
                    break;
                default:
                    break;
            }

        }

        private void OnStartCharge(CharacterController character) {
            // Disable other inputs.
            character.Default.Enable(character, false);
            character.Body.Stop();
            character.Body.SetWeight(0.05f);

            // Start the dash timer.
            m_ActionPhase = ActionPhase.PreAction;
            m_ChargeTimer.Start(m_ChargeDuration);
            m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);

            // character.Animator.Push(m_PredashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
            character.Animator.PlayAudioVisualEffect(m_ChargeEffect, m_ChargeSound);
        }
       
        protected virtual void OnStartThrow(CharacterController character) {
            character.Default.Enable(character, false);

            character.Body.SetWeight(0f);
            character.Body.velocity = character.Input.Direction.Normal * m_ThrowSpeed;
            if (character.Input.Direction.Normal == Vector2.zero) {
                character.Body.velocity = Vector2.right * character.FacingDirection * m_ThrowSpeed;
            }

            m_ActionPhase = ActionPhase.MidAction;
            m_ThrowTimer.Start(m_ThrowDuration * m_ChargeTimer.InverseRatio);
            character.Collider.sharedMaterial = m_BouncyMaterial;

            // Replace the animation.
            character.Animator.Remove(m_PrebounceAnimation);
            character.Animator.Push(m_BounceAnimation, CharacterAnimator.AnimationPriority.ActionActive);
            character.Animator.PlayAudioVisualEffect(m_StartBounceEffect, m_StartBounceSound);
            if (character.Default.Trail != null) { character.Default.Trail.Play(); }

            m_ChargeSound.Stop();
        }

        protected void OnStartPostbounce(CharacterController character) {
            m_ThrowTimer.Start(m_ThrowTimer.MaxValue); // m_PostthrowDuration);
            character.Body.SetWeight(m_PostbounceWeight);
            m_ActionPhase = ActionPhase.PostAction;
        }

        // End the bounce.
        protected void OnEndBounce(CharacterController character) {
            character.Default.Enable(character, true);

            character.Animator.Remove(m_BounceAnimation);
            character.Animator.Remove(m_PostbounceAnimation);
            character.Collider.sharedMaterial = m_DefaultMaterial;
            
            m_ThrowTimer.Stop();

            m_ActionPhase = ActionPhase.None;
            if (character.Default.Trail != null) { character.Default.Trail.Stop(); }
        }

        private void WhileCharging(CharacterController character, float dt) {
            character.Body.ClampRiseSpeed(0f);

            bool chargeIncremented = m_ChargeIncrementTimer.TickDown(dt);
            if (chargeIncremented && m_ChargeTimer.InverseRatio < 1f) {
                character.Animator.PlayAudioVisualEffect(null, m_ChargeSound);
                m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            }

            // m_PredashAnimation.fps = predashAnimBaseFPS + (predashAnimMaxFPS - predashAnimBaseFPS) * m_ChargeTimer.InverseRatio;
            // character.Animator.RotateBody(m_ChargeTimer.InverseRatio * 30f);

        }

        private void WhileBouncing(CharacterController character, float dt) {
            // if (character.Body.velocity.y > 0f) { 
            //     // character.Body.velocity *= 1.2f;
            //     m_Refreshed = true;
            //     OnStartPostbounce(character);
            // }
            character.Body.ClampSpeed(m_ThrowSpeed);
        }

        private void WhilePostbouncing(CharacterController character, float dt) {
            // if (character.Body.velocity.y < 0f) {
            //     OnEndBounce(character);
            // }
            
            // if (character.OnGround) { 
            //     OnEndBounce(character);
            // }
            // character.Body.ClampSpeed(m_BounceSpeed * 1.5f);
            character.Body.ClampFallSpeed(DefaultAction.MAX_FALL_SPEED);

        }

    }
}