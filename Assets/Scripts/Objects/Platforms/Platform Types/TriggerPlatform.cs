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
    // TODO: Add legs, and eyes.
    public class TriggerPlatform : PlatformObject {

        #region Enumerations.

        public enum TriggerState {
            None,
            Triggered,
            Releasing,
        }

        #endregion

        #region Variables.

        // The path that this platform follows.
        protected Vector3[] m_Path = null;

        // The current position in the path that the path is following.
        [SerializeField, ReadOnly] 
        protected int m_PathIndex;

        // Whether this platform is sinking or rising.
        [SerializeField]
        private TriggerState m_TriggerState = TriggerState.None;

        // The delay between being pressed and sinking.
        [SerializeField] 
        private float m_TriggerDelay = 0.06f;
        
        // Tracks the delay.
        [SerializeField, ReadOnly] 
        private Timer m_TriggerDelayTimer = new Timer(0f, 0f);

        // The speed at which this platform sinks at.
        [SerializeField] 
        private float m_TriggerMoveSpeed = 8f;
        
        // The speed with which the platform rises at.
        [SerializeField] 
        private float m_ReleaseMoveSpeed = 6f;

        // The sound that plays when this start sinks at.
        [SerializeField] 
        private AudioClip m_OnTriggeredSound = null;

        // The sound that plays when this starts rising.
        [SerializeField] 
        private AudioClip m_OnReleasedSound = null;

        // The sound that plays when this reaches the bottom.
        [SerializeField] 
        private AudioClip m_OnReachedEndSound = null;

        // The sound that plays when this reaches the top.
        [SerializeField] 
        private AudioClip m_OnReachedStartSound = null;
        
        #endregion

        // Runs once every frame.
        protected override void Update() {
            base.Update();
            
            // What to do for each state.
            switch (m_TriggerState) {
                case TriggerState.None:
                    if (m_Pressed) { OnTriggered(m_TriggerDelay); }
                    break;
                case TriggerState.Triggered:
                    if (!m_Pressed) { OnReleased(0f); }
                    break;
                case TriggerState.Releasing:
                    if (m_Pressed) { OnTriggered(0f); }
                    break;
                default:
                    break;
            }

        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_TriggerDelayTimer.TickDown(Time.fixedDeltaTime);
            
            if (finished) {

                // What to do for each state.
                switch (m_TriggerState) {
                    case TriggerState.Triggered:
                        Game.Audio.Sounds.PlaySound(m_OnTriggeredSound, 0.15f);
                        break;
                    case TriggerState.Releasing:
                        Game.Audio.Sounds.PlaySound(m_OnReleasedSound, 0.15f);
                        break;
                    default:
                        break;
                }

            }

            // What to do for each state.
            switch (m_TriggerState) {
                case TriggerState.Triggered:
                    WhileTriggered(Time.fixedDeltaTime);
                    break;
                case TriggerState.Releasing:
                    WhileReleasing(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        private void OnTriggered(float delay) {
            m_TriggerDelayTimer.Start(delay);
            m_TriggerState = TriggerState.Triggered;

            if (delay == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnTriggeredSound, 0.15f);
            }
            
        }

        private void OnReleased(float delay) {
            m_TriggerDelayTimer.Start(delay);
            m_TriggerState = TriggerState.Releasing;

            if (delay == 0f) {
                // TODO: Incorporate this into the timer.
                Game.Audio.Sounds.PlaySound(m_OnReleasedSound, 0.15f);
            }

        }


        private void WhileTriggered(float dt) {
            if (m_TriggerDelayTimer.Active) { return; }

            transform.Move(m_Path[1], m_TriggerMoveSpeed, Time.fixedDeltaTime, m_CollisionContainer);
            float distance = (transform.position - m_Path[1]).magnitude;
            if (distance == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnReachedEndSound, 0.15f);
            }

        }

        private void WhileReleasing(float dt) {
            if (m_TriggerDelayTimer.Active) { return; }

            transform.Move(m_Path[0], m_ReleaseMoveSpeed, Time.fixedDeltaTime, m_CollisionContainer);

            float distance = (transform.position - m_Path[0]).magnitude;
            if (distance == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnReachedStartSound, 0.15f);
                m_TriggerState = TriggerState.None;
            }

        }

    }

}
