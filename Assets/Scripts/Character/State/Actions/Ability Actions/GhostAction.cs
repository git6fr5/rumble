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
    public class GhostAction : Action {

        #region Variables

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private float m_FlyTicks;
        [SerializeField] private float m_Duration;
        public bool Flying => m_FlyTicks > 0f;

        // The settings of this flying.
        [SerializeField] private float m_Speed;
        [SerializeField] private float m_Acceleration;

        // The sound for this ability.
        [SerializeField] private AudioClip m_FlySound;

        #endregion

        // Enable/disable this ability.
        public override void Enable(CharacterState state, bool enable) {
            base.Enable(character, enable);
            if (enable && character.Input.Action1.Held) {
                Activate(character.Body, character.Input, character);
            }
            else {
                OnEndFly(ref m_FlyTicks, state);
            }
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            OnStartFly(body, input, state);
            input.Action1.ClearPressBuffer();
            m_Refreshed = false;

        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            // Refreshing.
            m_Refreshed = state.OnGround ? true : m_Refreshed;

            // Flying.
            bool finished = Timer.TickDown(ref m_FlyTicks, dt);
            if (Flying) {
                WhileFlying(body, input, dt);
            }
            if (finished || !input.Action1.Held) {
                OnEndFly(state);
            }

        }

        private void OnStartFly(Rigidbody2D body, InputSystem input, CharacterState state) {
            // Set the state.
            state.Default.Disable();

            // Set the body.
            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);            

            // Set this on cooldown.
            // EffectManager.ExplodeEffect(body.position);
            Timer.Start(ref m_FlyTicks, m_Duration);
        }

        // The ghost's active ability, flying.
        private void WhileFlying(Rigidbody2D body, InputSystem input, float dt) {
            Vector2 direction = input.Direction.Fly;
            body.velocity += direction.normalized * m_Acceleration * dt;

            if (body.velocity.magnitude > m_Speed) {
                body.velocity = m_Speed * body.velocity.normalized;
            }

            if (direction == Vector2.zero) {
                body.velocity *= 0.925f;
                if (body.velocity.magnitude <= Game.Physics.MovementPrecision) {
                    body.velocity = Vector2.zero;
                }
            }

            Game.ParticleGrid.Ripple(body.position, 1e5f, 4f, 0.2f, 3);
            SoundManager.PlaySound(m_FlySound, 0.05f);

        }

        // Ends the ghost active ability.
        private void OnEndFly(CharacterState state) {
            state.Default.Enable();
            m_FlyTicks = 0f;
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            return m_Refreshed && m_FlyTicks == 0f;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Pressed;
        }

    }
}