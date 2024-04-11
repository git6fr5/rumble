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
    [CreateAssetMenu(fileName="ChargeDashAction", menuName ="Actions/ChargeDash")]
    public class ChargeDashAction : CharacterAction {

        #region Variables.
        
        // The duration between pressing and moving, gives a little anticapatory feel.
        [SerializeField] 
        protected float m_PredashDuration = 0.16f;

        // The duration under which the character is actually dashing.
        [SerializeField] 
        protected float m_DashDuration = 0.14f;
        
        // A little cooldown after the dash to avoid spam pressing it.
        [SerializeField] 
        protected float m_PostdashDuration = 0.16f;

        // Runs through the phases of the dash cycle.
        [HideInInspector] 
        protected Timer m_DashTimer = new Timer(0f, 0f);

        // The distance covered by a dash.
        [SerializeField] 
        protected float m_DashDistance = 5f;

        // The speed of the actual dash.
        protected float DashSpeed => m_DashDistance / m_DashDuration;

        // The direction the player was facing before the dash started.
        [HideInInspector]
        protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_PredashAnimation = null;

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_DashAnimation = null;

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_PostdashAnimation = null;

        // The visual effect that plays at the start of the dash.
        [SerializeField]
        protected VisualEffect m_StartDashEffect;

        // The visual effect that plays at the start of the dash.
        [SerializeField]
        protected AudioSnippet m_StartDashSound;

        // The visual effect that plays at the start of the dash.
        [SerializeField]
        protected VisualEffect m_EndDashEffect;

        // The time it takes to fully charge.
        [SerializeField]
        private float m_ChargeDuration = 0.5f;

        // The timer that tracks how much has been charged.
        [SerializeField]
        private Timer m_ChargeTimer = new Timer(0f, 0f);

        // The tracks the increments of charge.
        [SerializeField]
        private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);

        // The sounds that plays when charging the hop.
        [SerializeField]
        private VisualEffect m_ChargeDashEffect = null;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private AudioSnippet m_ChargeDashSound = null;

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            if (m_DashTimer.Active) {
                OnStartPostdash(character);
            }
            m_ActionPhase = ActionPhase.None;
            m_DashTimer.Stop();

            if (!enable) {
                character.Animator.Remove(m_PredashAnimation);
                character.Animator.Remove(m_PostdashAnimation);
                character.Animator.Remove(m_DashAnimation);
            }

        }

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // Dashing.
            if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
                // The character should start dashing.
                OnStartCharge(character);

                // Release the input and reset the refresh.
                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }

            // Dashing.
            if (character.Input.Actions[1].Released && m_ActionPhase == ActionPhase.PreAction) {
                // The character should start dashing.
                OnStartDash(character);

                // Release the input and reset the refresh.
                character.Input.Actions[1].ClearReleaseBuffer();
                m_Refreshed = false;
            }

        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && !m_DashTimer.Active && !m_ChargeTimer.Active ? true : m_Refreshed;

            // Tick down the dash timer.
            bool finished = m_DashTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.PreAction:
                        OnStartDash(character);
                        break;
                    case ActionPhase.MidAction:
                        OnStartPostdash(character);
                        break;
                    case ActionPhase.PostAction:
                        OnEndDash(character);
                        break;
                    default:
                        break;
                }

            }

            // Charge the hop.
            m_ChargeTimer.TickDown(dt);
            
            // If in a phase.
            switch (m_ActionPhase) {
                case ActionPhase.PreAction:
                    WhileCharging(character, dt);
                    break;
                case ActionPhase.MidAction:
                    WhileDashing(character, dt);
                    break;
                case ActionPhase.PostAction:
                    WhilePostdashing(character, dt);
                    break;
                default:
                    break;
            }

        }

        private void OnStartCharge(CharacterController character) {
            // Disable other inputs.
            // character.Disable(duration); // lol, why did i do it like this?
            character.Default.Enable(character, false);

            // Stop the body.
            character.Body.Stop();
            character.Body.SetWeight(0.05f);

            // Start the dash timer.
            m_ChargeTimer.Start(m_ChargeDuration);
            m_ActionPhase = ActionPhase.PreAction;

            character.Animator.Push(m_PredashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
            character.Animator.PlayAudioVisualEffect(m_ChargeDashEffect, m_ChargeDashSound);
            m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            // m_CircleEffectIndex = Game.Visuals.Effects.PlayCircleEffect(m_ChargeDuration, character.transform, Vector3.zero);

        }

        protected void OnStartPredash(CharacterController character) {
            // Disable other inputs.
            character.Disable(m_PredashDuration + m_DashDuration);
            character.Default.Enable(character, false);

            // Stop the body.
            character.Body.Stop();

            // Start the dash timer.
            m_DashTimer.Start(m_PredashDuration);
            m_ActionPhase = ActionPhase.PreAction;

            // Set the animation.
            character.Animator.Push(m_PredashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);

        }

        protected virtual void OnStartDash(CharacterController character) {
            // Get the direction the character is facing.
            m_CachedDirection = new Vector2(character.FacingDirection, 0f);
            character.Body.SetVelocity(m_CachedDirection * DashSpeed);

            // Start the dash timer.
            m_DashTimer.Start(m_DashDuration * Mathf.Sqrt(m_ChargeTimer.InverseRatio));
            m_ActionPhase = ActionPhase.MidAction;

            // Replace the animation.
            character.Animator.Remove(m_PredashAnimation);
            character.Animator.Push(m_DashAnimation, CharacterAnimator.AnimationPriority.ActionActive);
            character.Animator.PlayAudioVisualEffect(m_StartDashEffect, m_StartDashSound);
            if (character.Default.Trail != null) { character.Default.Trail.Play(); }

            //
            m_ChargeDashSound.Stop();
            m_ChargeTimer.Stop();
            
        }

        protected virtual void OnStartPostdash(CharacterController character) {
            // Check how to handle the momentum when coming out of the dash.
            if (character.Input.Direction.Horizontal == Mathf.Sign(m_CachedDirection.x)) {
                character.Body.SetVelocity(m_CachedDirection * character.Default.Speed);
                character.Animator.Push(m_PostdashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
            }
            else {
                character.Body.SetVelocity(Vector2.zero);
            }

            // Re-enable control over the character.
            character.Default.Enable(character, true);

            // Replace the animation.
            character.Animator.Remove(m_DashAnimation);
            character.Animator.Push(m_PostdashAnimation, CharacterAnimator.AnimationPriority.ActionPostActive);
            character.Animator.PlayAudioVisualEffect(m_EndDashEffect, null);

            // Start the post-dash (dash cooldown) timer.
            m_DashTimer.Start(m_PostdashDuration);
            m_ActionPhase = ActionPhase.PostAction;
        }

        // End the dash.
        protected virtual void OnEndDash(CharacterController character) {
            character.Animator.Remove(m_PostdashAnimation);
            m_ActionPhase = ActionPhase.None;
            if (character.Default.Trail != null) { character.Default.Trail.Stop(); }

            // character.Animator.RotateBody(0f);
        }

        private void WhileCharging(CharacterController character, float dt) {
            character.Body.ClampRiseSpeed(0f);

            bool chargeIncremented = m_ChargeIncrementTimer.TickDown(dt);
            if (chargeIncremented && m_ChargeTimer.InverseRatio < 1f) {
                character.Animator.PlayAudioVisualEffect(m_ChargeDashEffect, m_ChargeDashSound);
                m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            }

            // m_PredashAnimation.fps = predashAnimBaseFPS + (predashAnimMaxFPS - predashAnimBaseFPS) * m_ChargeTimer.InverseRatio;
            // character.Animator.RotateBody(m_ChargeTimer.InverseRatio * 30f);

        }

        private void WhilePredashing(CharacterController character, float dt) {
            
        }

        private float predashAnimBaseFPS = 4f;
        private float predashAnimMaxFPS = 16f;

        private void WhileDashing(CharacterController character, float dt) {
            if (Mathf.Abs(character.Body.velocity.x) < DashSpeed / 2f || Mathf.Abs(character.Body.velocity.y) > 0.2f) {
                OnStartPostdash(character);
            }
        }

        private void WhilePostdashing(CharacterController character, float dt) {

        }

    }
}