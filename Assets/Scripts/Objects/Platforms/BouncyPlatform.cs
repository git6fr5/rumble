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
    public class BouncyPlatform : PlatformObject {

        #region Enumerations.

        public enum BounceState {
            None,
            Tensing,
            Releasing,
        }

        #endregion

        // Whether this is bouncing or releasing.
        [SerializeField] 
        private BounceState m_BounceState = BounceState.None;
        
        // The amount of bounce on releasing.
        [SerializeField] 
        private float m_JumpSpeed = 10;
        
        // The speed with which this moves.
        [SerializeField] 
        private float m_MoveSpeed = 7.5f;
        
        // The max tension before releasing.
        [SerializeField] 
        private float m_MaxTension = 0.7f;

        // The sound that plays when this bounces.
        [SerializeField] 
        private AudioClip m_BounceSound = null;

        // The max tension for the bounce platform.
        Vector3 MaxTensionPosition => m_Origin + Vector3.down * m_MaxTension;

        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
            base.Update();
            
            // What to do for each state.
            switch (m_BounceState) {
                case BounceState.None:
                    if (m_PressedDown) { OnStartTensing(); }
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
            transform.Move(MaxTensionPosition, m_MoveSpeed, dt, m_CollisionContainer);
            
            float distance = (transform.position - MaxTensionPosition).magnitude;
            if (distance == 0f) {
                OnBounce();
            }
        }

        private void WhileReleasing(float dt) {
            transform.Move(m_Origin, m_MoveSpeed, dt, m_CollisionContainer);

            float distance = (transform.position - m_Origin).magnitude;
            if (distance == 0f) {
                m_BounceState = BounceState.None;
            }

        }

        private void OnBounce() {
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    character.Body.velocity += new Vector2(0f, m_JumpSpeed);
                }

            }

            Game.Audio.Sounds.PlaySound(m_BounceSound, 0.2f);
            m_BounceState = BounceState.Releasing;
        }

    }
}
