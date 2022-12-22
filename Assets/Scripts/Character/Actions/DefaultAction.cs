/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Input;
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Character.Actions {

    ///<summary>
    /// The default ability for controlling the character.
    ///<summary>
    [System.Serializable]
    public class DefaultAction : CharacterAction {

        #region Variables

        /* --- Constant Variables --- */

        // The factor that slows down rising if not holding the input.
        protected const float RISE_FRICTION = 0.1f;

        // The amount of time before noticing we're falling.
        protected const float COYOTE_BUFFER = 0.08f;

        // The friction applied to a body that is still under coyote time.
        protected const float COYOTE_FRICTION = 0.5f;

        // The amount of hang time.
        protected const float APEX_BUFFER = 0.08f;

        // Holds the player at the m_HangTimer of their jump for a brief moment longer.
        protected const float HANG_FACTOR = 0.95f;

        /* --- Member Variables --- */

        // Whether the character is using the default movement.
        [SerializeField] 
        public bool m_MoveEnabled = true;

        // Whether the character is using the default falling.
        [SerializeField] 
        public bool m_FallEnabled = true;

        // The default speed the character moves at.
        [SerializeField] 
        private float m_Speed = 5f;

        // The default acceleration of the character.
        [SerializeField] 
        private float m_Acceleration = 100f;

        // The default jump height of the character.
        [SerializeField] 
        private float m_Height = 3f;

        // The default time taken to reach the m_HangTimer of the jump.
        [SerializeField] 
        private float m_RisingTime = 1f;

        // The default time taken to fall from the m_HangTimer of the jump.
        [SerializeField] 
        private float m_FallingTime = 0.75f;

        // The maximum speed with which this character can fall.
        [SerializeField] 
        private float m_MaxFallSpeed = 25f;

        // The calculated jump speed based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_JumpSpeed = 0f;

        // The calculated m_Weight based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_Weight = 0f;

        // The calculated sink factor based on the height, rising and falling time.
        // The sink is the factor applied to the m_Weight when falling.
        [SerializeField, ReadOnly] 
        private float m_Sink = 0f;

        // Tracks how long the character has not been on the ground.
        [SerializeField, ReadOnly] 
        private Timer m_CoyoteTimer = new Timer(0f, COYOTE_BUFFER);

        // Tracks how long its been since the character reached the m_HangTimer.
        [SerializeField, ReadOnly] 
        private Timer m_HangTimer = new Timer(0f, APEX_BUFFER);

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);
        }

        // Runs once every frame to check the inputs for this ability.
        public override void InputUpdate(CharacterController controller) {
            if (!m_Enabled) { return; }

            if (controller.Input.Action0.Pressed && m_Refreshed) {
                // The character should jump.
                OnJump(controller);

                // Release the input and reset the refresh.
                controller.Input.Action0.ClearPressBuffer();
                m_Refreshed = false;
            }
            
        }

        // 
        public override void PhysicsUpdate(CharacterController character, float dt) {
            if (!m_Enabled) { return; }
            
            // Refreshing.
            m_Refreshed = character.OnGround || m_CoyoteTimer.Value > 0f;

            // Tick the m_CoyoteTimer timer.
            m_CoyoteTimer.TickDownIfElseReset(dt, !character.OnGround);
            if (m_MoveEnabled) { 
                WhileMoving(character, dt); 
            }
            if (m_FallEnabled) { 
                WhileFalling(character, dt); 
            }
            
        }

        private void OnJump(CharacterController character) {
            // Refresh the jump settings.
            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            character.Body.Move(Vector2.up * 2f * Game.Physics.Collisions.CollisionPrecision);
            character.Body.ClampFallSpeed(0f);
            character.Body.AddVelocity(Vector2.up * m_JumpSpeed);

            // Reset the m_CoyoteTimer ticks.
            m_CoyoteTimer.Stop();
        }

        // Process the physics of this action.
        private void WhileMoving(CharacterController character, float dt) {
            // Cache the target and current velocities.
            float targetSpeed = character.Input.Direction.Horizontal * m_Speed;
            float currentSpeed = character.Body.velocity.x;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            Vector2 velocity = new Vector2(currentSpeed + deltaSpeed, character.Body.velocity.y);
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                velocity = new Vector2(targetSpeed, character.Body.velocity.y);
            }

            // Set the velocity.
            character.Body.SetVelocity(velocity);
            
        }

        // Not really falling, but rather "while default grav acting on this body"
        private void WhileFalling(CharacterController character, float dt) {
            // Set the m_Weight to the default.
            float weight = 1f;

            // If the body is not on the ground.
            if (!character.OnGround) { 
                // And it is rising.
                if (character.Rising) {

                    // Multiply it by its m_Weight.
                    m_Weight = weight;
                    m_HangTimer.Start();

                    // If not holding jump, then rapidly slow the rising body.
                    if (!character.Input.Action0.Held) {
                        character.Body.SetVelocity(new Vector2(character.Body.velocity.x, character.Body.velocity.y * (1f - RISE_FRICTION)));
                    }
                    
                }
                else {
                    // If it is falling, also multiply the sink m_Weight.
                    m_Weight = (weight * m_Sink);

                    // If it is still at its m_HangTimer, factor this in.
                    if (m_HangTimer.Active) {
                        m_Weight *= HANG_FACTOR;
                        m_HangTimer.TickDown(dt);
                    }

                    // If the m_CoyoteTimer timer is still ticking, fall slower.
                    if (m_CoyoteTimer.Active) {
                        m_Weight *= COYOTE_FRICTION;
                    }

                }

            }

            // Set the m_Weight.
            character.Body.SetWeight(m_Weight);

            // Clamp the fall speed at a given value.
            character.Body.ClampFallSpeed(m_MaxFallSpeed);
        }

        // Calculates the speed and m_Weight of the jump.
        public static void RefreshJumpSettings(ref float v, ref float w, ref float s, float h, float t_r, float t_f) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
            s = (t_f * t_f) * w * Mathf.Abs(UnityEngine.Physics2D.gravity.y) / (2f * h);
            s = 1f / s;
        }

    }

}

