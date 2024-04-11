/* --- Libraries --- */
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
    [CreateAssetMenu(fileName="HopAction", menuName ="Actions/Hop")]
    public class HopAction : CharacterAction {

        #region Variables.

        // The height to be covered by a fully charged jump.
        [SerializeField] 
        private float m_Height = 8f;
        
        // The time the jump will last.
        [SerializeField] 
        private float m_RisingTime = 0.4f;
        
        // The speed calculated for the jump.
        [SerializeField, ReadOnly] 
        private float m_Speed = 0f;
        
        // The weight calculated for the hop.
        [SerializeField, ReadOnly] 
        private float m_Weight = 0f;

        // The time it takes to fully charge.
        [SerializeField]
        private float m_ChargeDuration = 0.5f;

        // The timer that tracks how much has been charged.
        [SerializeField]
        private Timer m_ChargeTimer = new Timer(0f, 0f);

        // The tracks the increments of charge.
        [SerializeField]
        private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);

        // The sprites while charging a hop.
        [SerializeField]
        private SpriteAnimation m_ChargeHopAnimation = null;

        // The sprites while hopping
        [SerializeField]
        private SpriteAnimation m_HopAnimation = null;

        // The sprites while hopping
        [SerializeField]
        private SpriteAnimation m_FallAnimation = null;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private VisualEffect m_ChargeHopEffect = null;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private AudioSnippet m_ChargeHopSound = null;

        // The sounds that plays when hopping.
        [SerializeField]
        private AudioSnippet m_HopSound = null;

        // The effect that plays when releasing the hop.
        [SerializeField]
        private VisualEffect m_OnReleaseHopEffect = null;

        // An index to the particle that is associated with the charge timer.
        // [SerializeField] 
        // private int m_CircleEffectIndex = -1;

        #endregion


        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            if (!enable && m_ActionPhase == ActionPhase.MidAction) {
                character.Body.ClampRiseSpeed(Mathf.Min(character.Body.velocity.y, character.Default.JumpSpeed));
                OnStartFall(character);
            }

            RefreshHopSettings(ref m_Speed, ref m_Weight, m_Height, m_RisingTime);
            m_ActionPhase = ActionPhase.None;
            // Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);
            m_ChargeTimer.Stop();

            if (enable) {
                if (character.Input.Actions[1].Held) {
                    OnStartCharge(character);

                    // Release the input and reset the refresh.
                    character.Input.Actions[1].ClearPressBuffer();
                    m_Refreshed = false;
                }
            }

            if (!enable) {
                character.Animator.Remove(m_ChargeHopAnimation);
                character.Animator.Remove(m_HopAnimation);
                character.Animator.Remove(m_FallAnimation);
                m_ChargeHopSound.Stop();
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
                OnStartHop(character);

                // Release the input and reset the refresh.
                character.Input.Actions[1].ClearReleaseBuffer();
                m_Refreshed = false;
            }

        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && !m_ChargeTimer.Active ? true : m_Refreshed;
            
            // Charge the hop.
            m_ChargeTimer.TickDown(dt);

            // If in a phase.
            switch (m_ActionPhase) {
                case ActionPhase.PreAction:
                    WhileCharging(character, dt);
                    break;
                case ActionPhase.MidAction:
                    WhileHopping(character, dt);
                    break;
                case ActionPhase.PostAction:
                    WhileFalling(character, dt);
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

            character.Animator.Push(m_ChargeHopAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
            character.Animator.PlayAudioVisualEffect(m_ChargeHopEffect, m_ChargeHopSound);
            m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            // m_CircleEffectIndex = Game.Visuals.Effects.PlayCircleEffect(m_ChargeDuration, character.transform, Vector3.zero);

        }

        private void OnStartHop(CharacterController character) {
            character.Default.Enable(character, true, false);

            character.Body.Move(Vector2.up * 2f * PhysicsManager.Settings.collisionPrecision);
            character.Body.SetVelocity(m_Speed * Mathf.Sqrt(m_ChargeTimer.InverseRatio) * Vector2.up);
            character.Body.SetWeight(m_Weight);

            character.Animator.Remove(m_ChargeHopAnimation);
            character.Animator.Push(m_HopAnimation, CharacterAnimator.AnimationPriority.ActionActive);
            
            if (m_OnReleaseHopEffect != null) { m_OnReleaseHopEffect.Play(); }
            if (m_HopSound != null) { m_HopSound.Play(); }
            if (m_ChargeHopSound != null) { m_ChargeHopSound.Stop(); }
            // Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);
            if (character.Default.Trail != null) { character.Default.Trail.Play(); }

            m_ChargeTimer.Stop();
            m_ActionPhase = ActionPhase.MidAction;

        }

        private void OnStartFall(CharacterController character) {
            character.Default.Enable(character, true);
            character.Animator.Push(m_FallAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
            character.Animator.Remove(m_HopAnimation);
        }

        private void WhileCharging(CharacterController character, float dt) {
            character.Body.ClampRiseSpeed(0f);

            bool chargeIncremented = m_ChargeIncrementTimer.TickDown(dt);
            if (chargeIncremented && m_ChargeTimer.InverseRatio < 1f) {
                character.Animator.PlayAudioVisualEffect(m_ChargeHopEffect, m_ChargeHopSound);
                m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            }

            // float angle = Vector2.Angle(Vector2.up, new Vector2(character.Input.Direction.Horizontal, m_Height / 2f)); 
            // if (character.Input.Direction.Horizontal == 0f) { angle = 0f; }
            // character.Animator.RotateBody(-angle); //  * (m_ChargeTimer.Ratio / 2f + 0.5f)

        }

        private void WhileHopping(CharacterController character, float dt) {
            if (!character.Rising) {
                m_ActionPhase = ActionPhase.PostAction;
                OnStartFall(character);
            }
        }

        private void WhileFalling(CharacterController character, float dt) {
            if (m_Refreshed) {
                character.Animator.Remove(m_FallAnimation);
                m_ActionPhase = ActionPhase.None;
            }
        }

        // Calculates the speed and weight of the jump.
        public static void RefreshHopSettings(ref float v, ref float w, float h, float t_r) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
        }

    }

}