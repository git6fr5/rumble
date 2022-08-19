/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;
using Platformer.Obstacles;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class ShadowAction : AbilityAction {

        #region Variables

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private bool m_PreDashing;
        public bool Predashing => m_PreDashing;
        [SerializeField, ReadOnly] private bool m_Dashing;
        public bool Dashing => m_Dashing;
        [SerializeField, ReadOnly] private Vector2 m_CachedDirection;
        [SerializeField, ReadOnly] private Vector2 m_CachedVelocity;

        // Tracks the timeline of the dash.
        [SerializeField, ReadOnly] private float m_DashTicks;
        [SerializeField] private float m_PreDashBuffer;
        [SerializeField] private float m_DashBuffer;
        [SerializeField] private float m_CooldownBufferTicks;
        public float Cooldown => m_PreDashBuffer + m_DashBuffer + m_CooldownBufferTicks;
        public bool EndPreDash => m_PreDashing && m_DashTicks <= Cooldown - m_PreDashBuffer;
        public bool EndDash => m_Dashing && m_DashTicks <= Cooldown - m_PreDashBuffer - m_DashBuffer;
        public bool HalfwayFinished => m_Dashing && m_DashTicks <= Cooldown - m_PreDashBuffer - m_DashBuffer / 2f;

        // The distance covered by the dash.
        [SerializeField] private float m_DashDistance;
        private float DashSpeed => m_DashDistance / m_DashBuffer;

        [SerializeField] private bool m_Locked = false;
        public bool Locked => m_Locked;
        [SerializeField] private ShadowBlock m_LockedBlock;
        [SerializeField] private float m_LockedTicks = 0f;

        #endregion

        public override void Enable(CharacterState character, bool enable) {
            if (!enable) {
                character.OverrideMovement(false);
                character.OverrideFall(false);
                if (m_Dashing) {
                    character.Body.SetVelocity(m_CachedVelocity);
                    m_Dashing = false;
                }
                m_DashTicks = 0f;
            }
            base.Enable(character, enable);
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            if (m_Locked) { 
                m_CachedDirection = input.Direction.Fly != Vector2.zero ? input.Direction.Fly : m_CachedDirection;
                return;
            }

            // Chain the dash actions.
            state.OverrideMovement(true);
            state.OverrideFall(true);

            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);
            m_CachedDirection = input.Direction.Fly != Vector2.zero ? input.Direction.Fly : m_CachedDirection;
            m_CachedVelocity = body.velocity;

            // Clear the inputs.
            input.Action1.ClearPressBuffer();

            // Set this on cooldown.
            m_PreDashing = true;
            Timer.Start(ref m_DashTicks, Cooldown);
            m_Refreshed = false;
        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            if (m_Locked) { 
                Timer.TickDown(ref m_LockedTicks, dt);
                if (m_LockedTicks == 0f) {
                    state.OverrideMovement(true);
                    state.OverrideFall(true);

                    body.SetVelocity(Vector2.zero);
                    body.SetWeight(0f);
                    // Set this on cooldown.
                    m_PreDashing = true;
                    Timer.Start(ref m_DashTicks, Cooldown);
                    m_Refreshed = false; 
                    m_Locked = false;
                }
                return;
            }

            m_Refreshed = state.OnGround ? true : m_Refreshed;
            Timer.TickDown(ref m_DashTicks, dt);
            
            // When ending the predash, start the dash.
            if (EndPreDash) {
                body.SetVelocity(m_CachedDirection * DashSpeed);
                m_PreDashing = false;
                m_Dashing = true;
            }
            
            // When ending the dash, halt the body by alot.
            if (EndDash) {
                body.SetVelocity(m_CachedVelocity);
                state.OverrideMovement(false);
                state.OverrideFall(false);
                m_Dashing = false;
                if (m_LockedBlock != null) {
                    m_LockedBlock.Unlock();
                }
            }
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Locked || (m_Refreshed && m_DashTicks == 0f);
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return (m_Locked || input.Action1.Pressed);
        }

        public void Lock(CharacterState state, ShadowBlock block) {
            m_LockedTicks = 0.3f;

            if (m_LockedBlock != null) {
                m_LockedBlock.Unlock();
            }
            else {
                m_LockedTicks += 0.125f;
            }

            block.Lock();
            m_LockedBlock = block;
            m_Locked = true;
            state.transform.position = block.transform.position;
            state.Body.SetVelocity(Vector2.zero);

            
        }

    }
}