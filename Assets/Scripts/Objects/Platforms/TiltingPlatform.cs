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
    public class TiltingPlatform : PlatformObject {

        #region Enumerations.

        public enum TiltState {
            None,
            Tilting,
            Untilting
        }

        #endregion

        public TiltState m_TiltState = TiltState.None;

        // The delay between being pressed and sinking.
        [SerializeField] 
        private float m_TiltDelay = 0.04f;
        
        // Tracks the delay.
        [SerializeField, ReadOnly] 
        private Timer m_TiltDelayTimer = new Timer(0f, 0f);

        // The sound that plays when this start sinks at.
        [SerializeField] 
        private AudioClip m_OnStartTiltingSound = null;

        // The sound that plays when this starts rising.
        [SerializeField] 
        private AudioClip m_OnStartUntiltingSound = null;

        // The sound that plays when this start sinks at.
        [SerializeField] 
        private AudioClip m_OnFullTiltSound = null;

        // The sound that plays when this starts rising.
        [SerializeField] 
        private AudioClip m_OnStableSound = null;

        // The speed with which the platform rises at.
        [SerializeField] 
        private float m_BasePressure = 40f;

        // The speed with which the platform rises at.
        [SerializeField] 
        private float m_MaxAngle = 90f;

        // The speed with which the platform rises at.
        [SerializeField] 
        private float m_TiltPressure = 90f;

        // The speed with which the platform rises at.
        [SerializeField] 
        private float m_TiltDirection = 1f;

        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
            base.Update();
            
            // What to do for each state.
            switch (m_TiltState) {
                case TiltState.None:
                    if (m_Pressed) { OnStartTilting(m_TiltDelay); }
                    break;
                case TiltState.Tilting:
                    if (!m_Pressed) { OnStartUntilting(0f); }
                    break;
                case TiltState.Untilting:
                    if (m_Pressed) { OnStartTilting(0f); }
                    break;
                default:
                    break;
            }

        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_TiltDelayTimer.TickDown(Time.fixedDeltaTime);
            
            // What to do for each state.
            switch (m_TiltState) {
                case TiltState.Tilting:
                    WhileTilting(Time.fixedDeltaTime);
                    break;
                case TiltState.Untilting:
                    WhileUntilting(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        private void OnStartTilting(float delay) {
            m_TiltDelayTimer.Start(delay);
            m_TiltState = TiltState.Tilting;

            if (delay == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnStartTiltingSound, 0.15f);
            }
            
        }

        private void OnStartUntilting(float delay) {
            m_TiltDelayTimer.Start(delay);
            m_TiltState = TiltState.Untilting;

            if (delay == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnStartUntiltingSound, 0.15f);
            }

        }

        private void WhileTilting(float dt) {
            if (m_TiltDelayTimer.Active) { return; }

            int count = 0;
            float pressure = 0f;
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                
                float x = transform.position.x - m_CollisionContainer[i].transform.position.x;
                if (x < 0f) {
                    count -= 1;
                }
                else {
                    count += 1;
                }
                pressure += x;

            }

            if (count == 0) {
                return;
            }
            
            m_TiltDirection = Mathf.Sign(count);
            m_TiltPressure = pressure * m_BasePressure;

            transform.RotateTowards(m_TiltDirection * m_MaxAngle, m_TiltPressure, dt);
            if (Mathf.Abs(transform.eulerAngles.z) == m_MaxAngle) {
                Game.Audio.Sounds.PlaySound(m_OnFullTiltSound, 0.15f);
            }

        }

        private void WhileUntilting(float dt) {
            if (m_TiltDelayTimer.Active) { return; }

            transform.RotateTowards(0f, m_TiltPressure, dt);
            if (Mathf.Abs(transform.eulerAngles.z) == 0f) {
                Game.Audio.Sounds.PlaySound(m_OnStableSound, 0.15f);
                m_TiltState = TiltState.None;
            }

        }


    }
}
