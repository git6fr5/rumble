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
    ///
    ///<summary>
    [System.Serializable]
    public class ClimbAction : PhysicsAction {

        [SerializeField] private float m_Speed;
        [SerializeField] private float m_Acceleration;

        // Process the physics of this action.
        public override void Process(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (state.Disabled) { return; }
            if (!state.Sticky.Climbing && !state.Sticky.WallJumping) { return; }

            float x = 0f;
            if (state.Sticky.WallJumping) {
                x = body.velocity.x; // input.Direction.Facing * Mathf.Abs(body.velocity.x);
                // input.Direction.ForceFacing(Mathf.Sign(x));
                Game.ParticleGrid.Spin((Vector3)body.position, 1e4f, 2f, -1f);
            }
            else {
                Game.ParticleGrid.Implode(state.Body.position, 2e4f, 4f, 0.7f);
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