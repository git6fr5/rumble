/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
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

        // The sprites while charging a hop.
        [SerializeField]
        private Sprite[] m_ChargeHopAnimation = null;

        // The sprites while hopping
        [SerializeField]
        private Sprite[] m_HopAnimation = null;

        // The sprites while hopping
        [SerializeField]
        private Sprite[] m_FallAnimation = null;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private AudioSnippet m_ChargeHopSound = null;

        // The sounds that plays when hopping.
        [SerializeField]
        private AudioSnippet m_HopSound = null;

        // The effect that plays when releasing the hop.
        [SerializeField]
        private VisualEffect m_OnReleaseHopEffect;

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
            m_ChargeHopSound.Play();
            // m_CircleEffectIndex = Game.Visuals.Effects.PlayCircleEffect(m_ChargeDuration, character.transform, Vector3.zero);

        }

        private void OnStartHop(CharacterController character) {
            character.Default.Enable(character, true, false);

            character.Body.Move(Vector2.up * 2f * PhysicsManager.Settings.collisionPrecision);
            character.Body.SetVelocity(m_Speed * Mathf.Sqrt(m_ChargeTimer.InverseRatio) * Vector2.up);
            character.Body.SetWeight(m_Weight);

            character.Animator.Remove(m_ChargeHopAnimation);
            character.Animator.Push(m_HopAnimation, CharacterAnimator.AnimationPriority.ActionActive);
            
            m_OnReleaseHopEffect.Play();
            m_HopSound.Play();
            m_ChargeHopSound.Stop();
            // Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);

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