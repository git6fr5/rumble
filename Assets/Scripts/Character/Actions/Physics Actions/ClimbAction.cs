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
    ///
    ///<summary>
    [System.Serializable]
    public class ClimbAction : PhysicsAction {

        [SerializeField] private float m_Speed;
        [SerializeField] private float m_Acceleration;

        // Process the physics of this action.
        public override void Process(Rigidbody2D body, InputSystem input, CharacterController state, float dt) {
            if (state.Disabled) { return; }
            if (!state.Sticky.Climbing && !state.Sticky.WallJumping) { return; }

            float x = 0f;
            if (state.Sticky.WallJumping) {
                x = body.velocity.x;
            }

            // Cache the target and current velocities.
            float targetSpeed = input.Direction.Climb * m_Speed;
            float currentSpeed = body.velocity.y;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                body.velocity = new Vector2(x, targetSpeed);
            }
            else {
                body.velocity = new Vector2(x, currentSpeed + deltaSpeed);
            }
            
        }
        

    }
}