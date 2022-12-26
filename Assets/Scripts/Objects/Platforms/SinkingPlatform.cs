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
    public class SinkingPlatform : PlatformObject {

        #region Enumerations.

        public enum SinkState {
            None,
            Sinking,
            Rising,
        }

        #endregion

        #region Variables.

        // Whether this platform is sinking or rising.
        [SerializeField]
        private SinkState m_SinkState = SinkState.None;

        // The delay between being pressed and sinking.
        [SerializeField] 
        private float m_SinkDelay = 0.12f;
        
        // Tracks the delay.
        [SerializeField, ReadOnly] 
        private Timer m_SinkDelayTimer = new Timer(0f, 0f);

        // The speed at which this platform sinks at.
        [SerializeField] 
        private float m_SinkSpeed = 4.5f;
        
        // The speed with which the platform rises at.
        [SerializeField] 
        private float m_RiseSpeed = 2f;

        // The sound that plays when this start sinks at.
        [SerializeField] 
        private AudioClip m_OnStartSinkingSound = null;

        // The sound that plays when this starts rising.
        [SerializeField] 
        private AudioClip m_OnStartRisingSound = null;

        // The sound that plays when this reaches the bottom.
        [SerializeField] 
        private AudioClip m_OnReachedBottomSound = null;

        // The sound that plays when this reaches the top.
        [SerializeField] 
        private AudioClip m_OnReachedTopSound = null;
        
        #endregion

        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
            base.Update();
            
            // What to do for each state.
            switch (m_SinkState) {
                case SinkState.None:
                    if (m_PressedDown) { OnStartSinking(m_SinkDelay); }
                    break;
                case SinkState.Sinking:
                    if (!m_PressedDown) { OnStartRising(0f); }
                    break;
                case SinkState.Rising:
                    if (m_PressedDown) { OnStartSinking(0f); }
                    break;
                default:
                    break;
            }

        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_SinkDelayTimer.TickDown(Time.fixedDeltaTime);
            
            if (finished) {

                // What to do for each state.
                switch (m_SinkState) {
                    case SinkState.Sinking:
                        Game.Audio.Sounds.PlaySound(m_OnStartSinkingSound, 0.15f);
                        break;
                    case SinkState.Rising:
                        Game.Audio.Sounds.PlaySound(m_OnStartRisingSound, 0.15f);
                        break;
                    default:
                        break;
                }

            }

            // What to do for each state.
            switch (m_SinkState) {
                case SinkState.Sinking:
                    WhileSinking(Time.fixedDeltaTime);
                    break;
                case SinkState.Rising:
                    WhileRising(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        private void OnStartSinking(float delay) {
            m_SinkDelayTimer.Start(delay);
            m_SinkState = SinkState.Sinking;

            if (delay == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnStartSinkingSound, 0.15f);
            }
            
        }

        private void OnStartRising(float delay) {
            m_SinkDelayTimer.Start(delay);
            m_SinkState = SinkState.Rising;

            if (delay == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnStartRisingSound, 0.15f);
            }

        }


        private void WhileSinking(float dt) {
            if (m_SinkDelayTimer.Active) { return; }

            transform.Move(m_Path[1], m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);
            float distance = (transform.position - m_Path[1]).magnitude;
            if (distance == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnReachedBottomSound, 0.15f);
            }

        }

        private void WhileRising(float dt) {
            if (m_SinkDelayTimer.Active) { return; }

            transform.Move(m_Path[0], m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);

            float distance = (transform.position - m_Path[0]).magnitude;
            if (distance == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnReachedBottomSound, 0.15f);
                m_SinkState = SinkState.None;
            }

        }

    }

}
