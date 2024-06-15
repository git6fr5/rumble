/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Tests {

    using Character;
    using Physics;

    ///<summary>
    /// The default ability for controlling the character.
    ///<summary>
    [CreateAssetMenu(fileName="Flying", menuName="Actions/Flying")]
    public class FlyingAction : DefaultAction {

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            m_MoveEnabled = true;
        }

        public void EnableMovement(CharacterController character, bool enable) {
            if (!enable) {
                character.Body.velocity = Vector2.zero;
                m_MoveEnabled = false;
            }
            else {
                m_MoveEnabled = enable;
            }
        }

        // Runs once every frame to check the inputs for this ability.
        public override void InputUpdate(CharacterController character) {
            if (!m_ActionEnabled) { return; }
            // This action currently takes no inputs.            
        }

        // Runs once every fixed interval.
        public override void PhysicsUpdate(CharacterController character, float dt) {
            if (!m_ActionEnabled) { return; }

            // Tick the m_CoyoteTimer timer.
            if (m_MoveEnabled) { 
                WhileFlying(character, dt); 
            }
            
        }

        // Process the physics of this action.
        protected void WhileFlying(CharacterController character, float dt) {
            Vector2 targetVelocity = character.Input.Direction.Normal * m_Speed;
            Vector2 deltaVelocity = (targetVelocity - character.Body.velocity).normalized * dt * m_Acceleration;
            
            Vector2 velocity = character.Body.velocity + deltaVelocity;
            if (deltaVelocity.sqrMagnitude > (targetVelocity - character.Body.velocity).sqrMagnitude) {
                velocity = targetVelocity;
            }

            // Set the velocity.
            character.Body.SetVelocity(velocity);
            
        }

    }

}
