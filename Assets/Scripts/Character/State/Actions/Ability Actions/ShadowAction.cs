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
    public class ShadowAction : Action {

        #region Variables
        
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

                if (m_LockedBlock != null) {
                    m_LockedBlock.Unlock();
                }
                m_LockedTicks = 0f;
                m_Locked = false;

            }
            base.Enable(character, enable);
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            if (m_Locked) {
                WhileLocked(body, state, dt);
            }
            else {
                base.Refresh(body, input, state, dt);
            }
            
        }

        public void OnStartLock(CharacterState state, ShadowBlock block) {
            m_LockedTicks = 0.125f;

            if (m_LockedBlock != null) {
                m_LockedBlock.Unlock();
            }
            else {
                m_LockedTicks += 0.3f;
            }

            Game.ParticleGrid.Implode(state.Body.position, 1e5f, 4f, 0.75f);

            block.Lock();
            m_LockedBlock = block;
            m_Locked = true;
            state.transform.position = block.transform.position;
            state.Body.SetVelocity(Vector2.zero);
            
        }

        private void OnEndLock() {
            if (m_LockedBlock != null) {
                m_LockedBlock.Unlock();
            }
        }

        private void WhileLocked(Rigidbody2D body, CharacterState state, float dt) {
            bool finished = Timer.TickDown(ref m_LockedTicks, dt);
            if (finished) {
                OnStartDash();
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

        

    }
}