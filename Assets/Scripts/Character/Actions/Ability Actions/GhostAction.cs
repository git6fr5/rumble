/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Input;
using Platformer.Utilities;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class GhostAction : AbilityAction {

        #region Variables

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private float m_GhostTicks;
        [SerializeField] private float m_Duration;

        #endregion

        public override void Enable(CharacterState character, bool enable) {
            base.Enable(character, enable);
            if (enable) {
                if (character.Input.Action1.Held) {
                    Activate(character.Body, character.Input, character);
                }
            }
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            // if (m_GhostTicks > 0f) {
            //     state.OverrideMovement(false);
            //     state.OverrideFall(false);
            //     m_GhostTicks = 0f;
            //     input.Action1.ClearPressBuffer();
            //     return;
            // }

            // state.Disable(Cooldown - m_CooldownBufferTicks);
            state.OverrideMovement(true);
            state.OverrideFall(true);

            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);

            // Clear the inputs.
            input.Action1.ClearPressBuffer();

            // Set this on cooldown.
            Timer.Start(ref m_GhostTicks, m_Duration);
            m_Refreshed = false;

            Game.MainPlayer.ExplodeDust.Activate();


        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            m_Refreshed = state.OnGround ? true : m_Refreshed;
            bool finished = Timer.TickDown(ref m_GhostTicks, dt);
            
            // When ending the dash, halt the body by alot.
            if (finished || !input.Action1.Held) {
                state.OverrideMovement(false);
                state.OverrideFall(false);
                m_GhostTicks = 0f;
                
                // m_Enabled = false;
            }

        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Refreshed;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Pressed;
        }

        

    }
}