/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    // TODO: Add the Jelly.
    // TODO: Add the Springs.
    public class BouncyPlatform : PlatformObject {

        #region Enumerations.

        public enum BounceState {
            None,
            Tensing,
            Releasing,
        }

        #endregion

        #region Variables

        /* --- Constants --- */

        // The default bounce speed.
        private const float BOUNCE_SPEED = 26f;

        // The jump speed for missed bounces.
        private const float MISSED_BOUNCE_SPEED = 18f;

        /* --- Members --- */

        // Whether this is bouncing or releasing.
        [SerializeField] 
        private BounceState m_BounceState = BounceState.None;
        
        // The speed with which this moves.
        [SerializeField] 
        private float m_SinkSpeed = 2f;
        
        // The speed with which this moves.
        [SerializeField] 
        private float m_RiseSpeed = 6f;
        
        // The max tension before releasing.
        [SerializeField] 
        private float m_MaxTension = 0.7f;
        private Vector3 MaxTensionPosition => m_Origin + Vector3.down * m_MaxTension;

        // The sound that plays when this bounces.
        [SerializeField] 
        private AudioClip m_BounceSound = null;

        #endregion

        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
            base.Update();
            
            // What to do for each state.
            switch (m_BounceState) {
                case BounceState.None:
                    if (m_Pressed) { OnStartTensing(); }
                    break;
                default:
                    break;
            }

        }

        void FixedUpdate() {

            // What to do for each state.
            switch (m_BounceState) {
                case BounceState.Tensing:
                    WhileTensing(Time.fixedDeltaTime);
                    break;
                case BounceState.Releasing:
                    WhileReleasing(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        private void OnStartTensing() {
            m_BounceState = BounceState.Tensing;
        }

        private void WhileTensing(float dt) {
            transform.Move(MaxTensionPosition, m_SinkSpeed, dt, m_CollisionContainer);
            
            float distance = (transform.position - MaxTensionPosition).magnitude;
            
            // float distance = (m_Origin - MaxTensionPosition).magnitude / m_SinkSpeed;
            if (distance < (m_Origin - MaxTensionPosition).magnitude / 2f) {
                PreemptiveClamp();
            }

            if (distance < Game.Physics.Collisions.CollisionPrecision) {
                CheckPreemptiveBounce();
                Game.Audio.Sounds.PlaySound(m_BounceSound, 0.2f);
                m_BounceState = BounceState.Releasing;
            }
        }

        private void WhileReleasing(float dt) {
            transform.Move(m_Origin, m_RiseSpeed, dt, m_CollisionContainer);

            // Bounce a character that did not pre-emptively bounce if it
            // PRESSES jump while the platform is releasing.
            CheckBounce();

            float distance = (transform.position - m_Origin).magnitude;
            if (distance < Game.Physics.Collisions.CollisionPrecision) {
                m_BounceState = BounceState.None;
                MissedBounce();
            }

        }

        // Clamps the characters jump after the platform has gone down a certain distance.
        private void PreemptiveClamp() {
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    character.Default.ClampJump(true);
                    if (character.Input.Action0.Released) {
                        character.Default.OnExternalJump(character, character.Default.JumpSpeed);
                        character.Default.ClampJump(false);
                    }
                }
            }
        }

        // If the character has pressed the jump key/ is holding the jump key while the
        // bouncy platform has preemptively clamped the character.
        private void CheckPreemptiveBounce() {
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    if (character.Input.Action0.Held) {
                        print("pre-emptive bounce");
                        character.Default.OnExternalJump(character, BOUNCE_SPEED);
                        character.Default.ClampJump(false);
                    }
                    
                }
            }
        }

        // Bounce a character that did not pre-emptively bounce if it
        // PRESSES jump while the platform is releasing.
        private void CheckBounce() {
            print("checking bounce");
            print(m_CollisionContainer.Count);
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    if (character.Input.Action0.Held) {
                        print("releasing bounce");
                        character.Default.OnExternalJump(character, BOUNCE_SPEED);
                        character.Default.ClampJump(false);
                    }
                }
            }
        }

        // For characters that missed a bounce.
        private void MissedBounce() {
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    print("missed bounce");
                    character.Default.OnExternalJump(character, MISSED_BOUNCE_SPEED);
                    character.Default.ClampJump(false);
                }
            }
            m_CollisionContainer = new List<Transform>();
        }

    }
}
