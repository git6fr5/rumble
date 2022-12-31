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
using ShadowBlock = Platformer.Objects.Blocks.GhostBlock;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class GhostAction : CharacterAction {

        #region Variables.

        // The time it takes to fully charge.
        [SerializeField]
        private float m_GhostModeDuration = 4f;

        // The speed the ghost hand moves at.
        [SerializeField]
        private float m_GhostAcceleration = 100f;

        // The acceleration with which the ghost hand moves at.
        [SerializeField]
        private float m_GhostSpeed = 12f;

        // The timer that tracks how much has been charged.
        [SerializeField]
        private Timer m_GhostTimer = new Timer(0f, 0f);

        // The ghost soul.
        [SerializeField]
        private Rigidbody2D m_GhostHand;

        #endregion

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // Dashing.
            if (character.Input.Action1.Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
                // The character should start dashing.
                OnStartGhostMode(character);

                // Release the input and reset the refresh.
                character.Input.Action1.ClearPressBuffer();
                m_Refreshed = false;
            }

            // Dashing.
            if (character.Input.Action1.Released && m_ActionPhase == ActionPhase.MidAction) {
                // The character should start dashing.
                OnEndGhostMode(character);

                // Release the input and reset the refresh.
                character.Input.Action1.ClearReleaseBuffer();
                m_Refreshed = false;
            }

        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && !m_GhostTimer.Active ? true : m_Refreshed;
            
            // Tick down the shadow mode.
            bool finished = m_GhostTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.MidAction:
                        OnEndGhostMode(character);
                        break;
                    default:
                        break;
                }

            }

            switch (m_ActionPhase) {
                case ActionPhase.MidAction:
                    WhileInGhostMode(character, dt);
                    break;
                default:
                    break;
            }
            
        }

        private void OnStartGhostMode(CharacterController character) {
            character.Default.Enable(character, false);

            // Start the shadow mode timer.
            m_GhostTimer.Start(m_GhostModeDuration);
            m_ActionPhase = ActionPhase.MidAction;

            m_GhostHand.transform.position = character.transform.position + 1.2f * (Vector3)character.Input.Direction.Normal;
            m_GhostHand.gameObject.SetActive(true);
            
            character.Body.Stop();
            m_GhostHand.ReleaseAll();
            // character.Animator.Push(m_ShadowModeAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
        
        }

        private void OnEndGhostMode(CharacterController character) {
            character.Default.Enable(character, true);

            // Stop the shadow timer.
            m_GhostTimer.Stop();
            m_ActionPhase = ActionPhase.None;

            character.Body.Stop();
            character.Body.ReleaseXY();

            m_GhostHand.gameObject.SetActive(false);
            m_GhostHand.Freeze();
            // character.Animator.Remove(m_ShadowModeAnimation);

        }

        private void WhileInGhostMode(CharacterController character, float dt) {
            Vector2 direction = character.Input.Direction.Normal;
            m_GhostHand.velocity += m_GhostAcceleration * direction * dt;

            if (m_GhostHand.velocity.magnitude > m_GhostSpeed) {
                m_GhostHand.velocity = m_GhostSpeed * m_GhostHand.velocity.normalized;
            }

            if (direction == Vector2.zero) {
                m_GhostHand.velocity *= 0.925f;
                if (m_GhostHand.velocity.magnitude <= Game.Physics.Collisions.CollisionPrecision) {
                    m_GhostHand.velocity = Vector2.zero;
                }
            }

            float angle = Vector2.SignedAngle(Vector2.right, m_GhostHand.velocity);
            float flip = 0f;
            // if (angle <= 0f) {
            //     flip = 1f;
            // }
            m_GhostHand.transform.eulerAngles = 180f * Vector3.up * flip + Vector3.forward * angle;

        }

    }
}