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
    public class StickyAction : AbilityAction {

        #region Variables

        public bool Climbing => m_Enabled && m_StickTicks > 0f && !m_WallJumping;

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private float m_StickTicks;
        [SerializeField] private float m_Duration;

        [SerializeField] private bool m_WallJumping;
        public bool WallJumping => m_Enabled && m_WallJumping;

        // Allows a little leeway for when this character is no longer
        // in contact with the ground.
        [SerializeField] private float m_CoyoteBuffer = 0f;
        [SerializeField, ReadOnly] private float m_CoyoteTicks = 0f;
        public float CoyoteTicks => m_CoyoteTicks; 

        [SerializeField] private float m_CachedDirection;

        [SerializeField] private AudioClip m_StartJumpSound;
        [SerializeField] private AudioClip m_StartClimbSound;
        [SerializeField] private AudioClip m_EndJumpSound;
        [SerializeField] private AudioClip m_WallJumpingSound;


        #endregion

        public override void Enable(CharacterState character, bool enable) {
            base.Enable(character, enable);
            if (enable) {
                if (character.Input.Action1.Held) {
                    Activate(character.Body, character.Input, character);
                }
                m_CoyoteTicks = 0f;
            }
            if (!enable) {
                m_WallJumping = false;
                character.Input.Direction.LockFacing(false);
                m_StickTicks = 0f;
            }
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            // if (input.Action0.Pressed && !m_Refreshed && m_StickTicks != 0f) {
            //     body.velocity = 20f * (new Vector2(-input.Direction.Facing, 1f)).normalized;
            //     state.OverrideMovement(true);
            //     state.OverrideFall(false);
            //     m_WallJumping = true;
            //     m_StickTicks = 0f;
            // }

            if (input.Action0.Pressed && Climbing) {

                state.OverrideMovement(true);
                state.OverrideFall(true);

                Vector2 direction = new Vector2(-m_CachedDirection, 0).normalized;
                body.Move(direction * Game.Physics.MovementPrecision);
                body.SetVelocity(12.5f * direction);
                body.SetWeight(0f);

                m_CachedDirection *= -1f;

                input.Action0.ClearPressBuffer();
                if (state.FacingWall) {
                    input.Direction.ForceFacing(m_CachedDirection);
                }
                input.Direction.LockFacing(true);

                SoundManager.PlaySound(m_StartJumpSound, 0.1f);

                m_WallJumping = true;

                Game.MainPlayer.ExplodeDust.Activate();
                
            }

            if ((state.FacingWall || m_CoyoteTicks > 0f) && m_Refreshed) {
                Debug.Log("trying");
                state.OverrideMovement(true);
                state.OverrideFall(true);

                body.SetVelocity(Vector2.zero);
                body.SetWeight(0f);

                // Clear the inputs.
                input.Action1.ClearPressBuffer();

                m_CachedDirection = state.FacingWall ? input.Direction.Facing : -input.Direction.Facing;

                // Set this on cooldown.
                Timer.Start(ref m_StickTicks, m_Duration);
                SoundManager.PlaySound(m_StartClimbSound, 0.1f);

                m_Refreshed = false;
            }
            
        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            if (!state.FacingWall) {
                Timer.TickDown(ref m_CoyoteTicks, dt);
            }
            else {
                Timer.Start(ref m_CoyoteTicks, m_CoyoteBuffer);
            }

            m_Refreshed = state.OnGround ? true : m_Refreshed;

            if (!input.Action1.Held) {
                state.OverrideMovement(false);
                state.OverrideFall(false);
                m_StickTicks = 0f;
                m_WallJumping = false;
                input.Direction.LockFacing(false);
            }

            if (m_WallJumping) {

                if (!input.Action1.Held || (input.Action1.Held && state.FacingWall)) {
                    m_WallJumping = false;
                    input.Direction.LockFacing(false);
                    SoundManager.PlaySound(m_EndJumpSound, 0.1f);
                }
                else {
                    // body.velocity = new Vector2(body.velocity.x * 1.025f, body.velocity.y);
                    SoundManager.PlaySound(m_WallJumpingSound, 0.1f);
                    return;
                }

            }

            // When ending the dash, halt the body by alot.
            bool finished = Timer.TickDown(ref m_StickTicks, dt);
            if (finished || (!state.FacingWall && m_CoyoteTicks == 0f)) {
                state.OverrideMovement(false);
                state.OverrideFall(false);
                m_WallJumping = false;
                m_StickTicks = 0f;
            }

            
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return true;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Held || input.Action0.Pressed;
        }

        

    }
}