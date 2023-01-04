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

        // The friction applied to a body that is still under coyote time.
        protected const float COYOTE_FRICTION = 0.5f;

        // The maximum speed with which this character can fall.
        private float MAX_FALL_SPEED = 25f;

        /* --- Member Variables --- */

        // Whether the character is using the default movement.
        [SerializeField, ReadOnly] 
        public bool m_MoveEnabled = true;

        // Whether the character is using the default falling.
        [SerializeField, ReadOnly] 
        public bool m_FallEnabled = true;

        // Whether the character is using the default falling.
        [SerializeField, ReadOnly] 
        public bool m_DuckEnabled = true;

        // Forcefully clamp internal jumps, usually for prepping external jumps.
        [SerializeField, ReadOnly] 
        public bool m_ClampJump = false;

        // The default speed the character moves at.
        [SerializeField] 
        private float m_Speed = 7.5f;
        public float Speed => m_Speed;

        // The default acceleration of the character.
        [SerializeField] 
        private float m_Acceleration = 75f;

        // The default jump height of the character.
        [SerializeField] 
        private float m_Height = 3.5f;

        // The default time taken to reach the m_HangTimer of the jump.
        [SerializeField] 
        private float m_RisingTime = 0.45f;

        // The default time taken to fall from the m_HangTimer of the jump.
        [SerializeField] 
        private float m_FallingTime = 0.4f;

        // The amount of time before noticing we're falling.
        [SerializeField] 
        protected float m_CoyoteBuffer = 0.12f;

        // The amount of hang time.
        [SerializeField] 
        protected float m_HangBuffer = 0.04f;

        // Holds the player at the apex of their jump for a brief moment longer.
        [SerializeField] 
        protected float m_HangFactor = 0.24f;

        // The calculated jump speed based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_JumpSpeed = 0f;
        public float JumpSpeed => m_JumpSpeed;

        // The calculated m_Weight based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_Weight = 0f;

        // The calculated sink factor based on the height, rising and falling time.
        // The sink is the factor applied to the m_Weight when falling.
        [SerializeField, ReadOnly] 
        private float m_Sink = 0f;

        // Tracks how long the character has not been on the ground.
        [HideInInspector] 
        private Timer m_CoyoteTimer = new Timer(0f, 0f);

        // Tracks how long its been since the character reached the m_HangTimer.
        [HideInInspector] 
        private Timer m_HangTimer = new Timer(0f, 0f);

        // The animation for the character when idle.
        [SerializeField]
        private Sprite[] m_IdleAnimation = null;

        // The animation for the character when moving.
        [SerializeField]
        private Sprite[] m_MovementAnimation = null;

        // The animation for the character when rising.
        [SerializeField]
        private Sprite[] m_RisingAnimation = null;

        // The animation for the character when falling.
        [SerializeField]
        private Sprite[] m_FallingAnimation = null;

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            m_HangTimer = new Timer(0f, m_HangBuffer);
            m_CoyoteTimer = new Timer(0f, m_CoyoteBuffer);
            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            m_MoveEnabled = true;
            m_FallEnabled = true;
            m_ClampJump = false;

            character.Animator.Push(m_IdleAnimation, CharacterAnimator.AnimationPriority.DefaultIdle);
            character.Animator.Remove(m_MovementAnimation);
            character.Animator.Remove(m_RisingAnimation);
            character.Animator.Remove(m_FallingAnimation);
        }

        // When enabling/disabling this ability by movement and falling seperately.
        public void Enable(CharacterController character, bool move, bool fall) {
            Enable(character, true);

            m_MoveEnabled = move;
            m_FallEnabled = fall;
        
        }

        // Runs once every frame to check the inputs for this ability.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // Jumping.
            if (character.Input.Action0.Pressed && m_Refreshed) {
                // The character should jump.
                OnJump(character);

                // Release the input and reset the refresh.
                character.Input.Action0.ClearPressBuffer();
                m_CoyoteTimer.Stop();
                m_Refreshed = false;
            }
            
        }

        // 
        public override void PhysicsUpdate(CharacterController character, float dt) {
            if (!m_Enabled) { return; }

            // Landing.
            if (character.OnGround && !m_Refreshed) {
                OnLand(character);
            }

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
            if (m_DuckEnabled) {
                WhileDucking(character, dt);
            }
            
        }

        private void OnJump(CharacterController character) {
            // Refresh the jump settings.
            if (m_ClampJump) { return; }

            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            character.Body.Move(Vector2.up * 2f * Game.Physics.Collisions.CollisionPrecision);
            // These two lines are the key!!!
            character.Body.ClampFallSpeed(0f); 
            character.Body.AddVelocity(Vector2.up * m_JumpSpeed);

        }

        public void ClampJump(bool clamp) {
            m_ClampJump = clamp;
        }

        public void OnExternalJump(CharacterController character, float jumpSpeed) {
            // Refresh the jump settings.
            // RefreshJumpSettings(ref jumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            character.Body.Move(Vector2.up * 2f * Game.Physics.Collisions.CollisionPrecision);
            // These two lines are the key!!!
            character.Body.ClampFallSpeed(0f); 
            character.Body.SetVelocity(Vector2.up * jumpSpeed);

            character.Input.Action0.ClearPressBuffer();
            m_CoyoteTimer.Stop();
            m_Refreshed = false;

        }

        private void OnLand(CharacterController character) {
            m_ClampJump = false;
            character.Animator.Remove(m_RisingAnimation);
            character.Animator.Remove(m_FallingAnimation);
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

            if (character.Input.Direction.Horizontal != 0f) {
                character.Animator.Push(m_MovementAnimation, CharacterAnimator.AnimationPriority.DefaultMoving);
            }
            else {
                character.Animator.Remove(m_MovementAnimation);
            }
            
        }

        // Not really falling, but rather "while default grav acting on this body"
        private void WhileFalling(CharacterController character, float dt) {
            // Set the m_Weight to the default.
            float weight = m_Weight;

            // If the body is not on the ground.
            if (!character.OnGround) { 
                // And it is rising.
                if (character.Rising) {

                    // Multiply it by its m_Weight.
                    weight = m_Weight;
                    m_HangTimer.Start();

                    // If not holding jump, then rapidly slow the rising body.
                    if (!character.Input.Action0.Held) {
                        character.Body.SetVelocity(new Vector2(character.Body.velocity.x, character.Body.velocity.y * (1f - RISE_FRICTION)));
                    }

                    character.Animator.Push(m_RisingAnimation, CharacterAnimator.AnimationPriority.DefaultJumpRising);
                    
                }
                else {
                    // If it is falling, also multiply the sink m_Weight.
                    weight = (m_Weight * m_Sink);

                    // If it is still at its m_HangTimer, factor this in.
                    if (m_HangTimer.Active) {
                        weight *= m_HangFactor;
                        m_HangTimer.TickDown(dt);
                    }

                    // If the m_CoyoteTimer timer is still ticking, fall slower.
                    if (m_CoyoteTimer.Active) {
                        weight *= COYOTE_FRICTION;
                    }

                    character.Animator.Push(m_FallingAnimation, CharacterAnimator.AnimationPriority.DefaultJumpFalling);

                }

            }

            // Set the m_Weight.
            character.Body.SetWeight(weight);

            // Clamp the fall speed at a given value.
            character.Body.ClampFallSpeed(MAX_FALL_SPEED);
        }

        // While ducking.
        private void WhileDucking(CharacterController character, float dt) {

            int characterLayer = LayerMask.NameToLayer("Characters");
            if (character.Input.Direction.Vertical == -1f && character.gameObject.layer == characterLayer) {
                character.gameObject.layer = LayerMask.NameToLayer("Ducking");
            }
            else if (character.Input.Direction.Vertical != -1f && character.gameObject.layer != characterLayer) {
                character.gameObject.layer = characterLayer;
            }

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

