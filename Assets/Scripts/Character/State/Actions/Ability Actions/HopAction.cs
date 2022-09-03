/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class HopAction : Action {

        #region Variables

        // The different stages of the hop.
        [SerializeField, ReadOnly] private bool m_Charging;
        public bool Charging => m_Charging;
        [SerializeField, ReadOnly] private bool m_Hopping;
        public bool Hopping => m_Hopping;

        // The setting for this jump.
        [SerializeField] private float m_Height;
        [SerializeField] private float m_RisingTime;
        // The calculated settings for this jump.
        [SerializeField, ReadOnly] private float m_Speed;
        [SerializeField, ReadOnly] private float m_Weight;

        // The charge.
        [SerializeField, ReadOnly] private float m_Charge;
        public float Charge => m_Charge;
        [SerializeField] private float m_MaxCharge;
        public float Ratio => m_Charge / m_MaxCharge;
        public float ChargedSpeed => m_Speed * Mathf.Sqrt(m_Charge / m_MaxCharge) * Vector2.up;

        // The sound for this ability.
        [SerializeField] private AudioClip m_ChargeSound;
        [SerializeField] private AudioClip m_ReleaseSound;

        #endregion

        public override void Enable(CharacterState state, bool enable) {
            base.Enable(state, enable);
            if (enable && state.Input.Action1.Held) {
                Activate(state.Body, state.Input, state);
            }
            else if (!enable) {
                OnEndCharge(ref m_Charge, ref m_Charging, ref m_ChargeSound);
                OnEndHop(state);
            }
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            OnStartCharge(body, input, state);
            input.Action1.ClearPressBuffer();
            m_Refreshed = false;

        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            // Refreshing.
            m_Refreshed = state.OnGround ? true : m_Refreshed;

            // Charge the hop.
            if (m_Charging && input.Action1.Held) {
                WhileCharging(body, ref m_Charge, ref m_MaxCharge, dt);
                Game.ParticleGrid.Spin((Vector3)body.position, Ratio * 5e3f, (1f - Ratio) * 5f + 5f, 1f);
            }
            // End the charging and start the hop.
            else if (m_Charging && !input.Action1.Held) {
                RefreshHopSettings(ref m_Speed, ref m_Weight, m_Height, m_RisingTime);
                OnEndCharge(ref m_Charge, ref m_Charging, m_ChargeSound);
                OnStartHop(ref m_Hopping, ChargedSpeed, m_Weight, body, input);
            }

            // End the hopping.
            if (m_Hopping && !body.Rising()) {
                OnEndHop(state);
            }

        }

        private void OnStartCharge(Rigidbody2D body, InputSystem input, CharacterState state) {
            // Set the state.
            state.Default.Disable();

            // Set the body.
            body.SetWeight(0.05f);
            body.SetVelocity(Vector2.zero);
            
            // Set this on cooldown.
            SoundManager.PlaySound(m_ChargeSound, 0.15f);
            m_Charging = true;
            m_Hopping = false;
        }

        private static void WhileCharging(Rigidbody2D body, ref float charge, ref float maxCharge, float dt) {
            Timer.TickUp(ref charge, maxCharge, dt);
            body.ClampRiseSpeed(0f);
        }

        private static void OnStartHop(ref bool hopping, float speed, float weight, Rigidbody2D body, InputSystem input) {
            // Set the state.
            state.OverrideMovement(true);
            state.OverrideFall(false);

            // Set the body.
            body.Move(Vector2.up * Game.Physics.MovementPrecision);
            body.SetVelocity(Vector2.up * speed);
            body.SetWeight(weight);

            // Clear the inputs.
            input.Action1.ClearReleaseBuffer();

            // Set this on cooldown.
            // EffectManager.ExplodeEffect(body.position);
            Game.ParticleGrid.Spin((Vector3)body.position, Ratio * 1e6f, 50f, -1f);
            m_Charge = 0f;
            m_Charging = false;
            m_Hopping = true;
        }

        private static void OnEndHop(CharacterState state) {
            state.Default.Enable();
        }

        private static void OnEndCharge(ref float charge, ref bool charging, AudioClip sfx) {
            charge = 0f;
            charging = false;
            SoundManager.StopSound(sfx);
        }

        // Calculates the speed and weight of the jump.
        public static void RefreshHopSettings(ref float v, ref float w, float h, float t_r) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Refreshed;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Pressed || input.Action1.Released;
        }

    }

}