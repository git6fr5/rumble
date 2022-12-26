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
using ShadowBlock = Platformer.Objects.Blocks.ShadowBlock;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class ShadowAction : CharacterAction {

        #region Variables.

        /* --- Constants --- */

        // The leniency on killing the character when they come out of shadow mode.
        public const float COLLISION_LENIENCY = 0.1f;

        /* --- Members --- */

        // The time it takes to fully charge.
        [SerializeField]
        private float m_ShadowModeDuration = 0.5f;

        // The timer that tracks how much has been charged.
        [SerializeField]
        private Timer m_ShadowModeTimer = new Timer(0f, 0f);

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            if (m_ShadowModeTimer.Active) {
                OnEndShadowMode(character);
            }
            m_ActionPhase = ActionPhase.None;
            m_ShadowModeTimer.Stop();

            // if (!enable) {
            //     character.Animator.Remove(m_PredashAnimation);
            // }

        }

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // Dashing.
            if (character.Input.Action1.Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
                // The character should start dashing.
                OnStartShadowMode(character);

                // Release the input and reset the refresh.
                character.Input.Action1.ClearPressBuffer();
                m_Refreshed = false;
            }

            // Dashing.
            if (character.Input.Action1.Released && m_ActionPhase == ActionPhase.MidAction) {
                // The character should start dashing.
                OnEndShadowMode(character);

                // Release the input and reset the refresh.
                character.Input.Action1.ClearReleaseBuffer();
                m_Refreshed = false;
            }
            
        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && !m_ShadowModeTimer.Active ? true : m_Refreshed;
            
            // Tick down the shadow mode.
            bool finished = m_ShadowModeTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.MidAction:
                        OnEndShadowMode(character);
                        break;
                    default:
                        break;
                }

            }
                
            switch (m_ActionPhase) {
                case ActionPhase.MidAction:
                    WhileInShadowMode(character, dt);
                    break;
                default:
                    break;
            }

        }

        private void OnStartShadowMode(CharacterController character) {
            // Disable the collider.
            character.Collider.isTrigger = true;

            // Make it so that you move a little faster and jump a little higher?

            // Start the shadow mode timer.
            m_ShadowModeTimer.Start(m_ShadowModeDuration);
            m_ActionPhase = ActionPhase.MidAction;

            // character.Animator.Push(m_DashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);

        }

        private void OnEndShadowMode(CharacterController character) {
            Debug.Log("end");
            // Reset the collider.
            character.Collider.isTrigger = false;
            
            // Stop the shadow timer.
            m_ShadowModeTimer.Stop();
            m_ActionPhase = ActionPhase.None;

            // Check to see if the character should die.
            float radius = character.Collider.radius - COLLISION_LENIENCY;
            bool touching = Game.Physics.Collisions.Touching(character.Body.position, radius, Game.Physics.CollisionLayers.Ground);
            bool onScreen = Game.Visuals.Camera.IsWithinBounds(character.Body.position);
            if (touching || !onScreen) {
                // character.Die();
                Debug.Log("died");
            }
            
            // character.Animator.Push(m_DashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);

        }

        private void WhileInShadowMode(CharacterController character, float dt) {
            
        }

    }
}