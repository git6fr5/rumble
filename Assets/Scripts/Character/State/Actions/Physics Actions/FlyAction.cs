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
    /// Figures out how the character should move while flying.
    ///<summary>
    [System.Serializable]
    public class FlyAction : PhysicsAction {

        [SerializeField] private float m_Speed;
        [SerializeField] private float m_Acceleration;

        [SerializeField] private AudioClip m_FlySound;

        // Process the physics of this action.
        public override void Process(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (state.Disabled) { return; }
            if (!state.MovementOverride || !state.Ghost.Enabled) { return; }

            Vector2 direction = input.Direction.Fly;
            body.velocity += m_Acceleration * direction.normalized * dt;

            if (body.velocity.magnitude > m_Speed) {
                body.velocity = m_Speed * body.velocity.normalized;
            }

            if (direction == Vector2.zero) {
                body.velocity *= 0.925f;
                if (body.velocity.magnitude <= Game.Physics.MovementPrecision) {
                    body.velocity = Vector2.zero;
                }
            }

            SoundManager.PlaySound(m_FlySound, 0.05f);
            
        }

    }

}