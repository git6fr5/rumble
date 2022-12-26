/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Spikes;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public class MovingSpikeball : MonoBehaviour {

        #region Variables.

        // The pause timer.
        [HideInInspector]
        protected Timer m_PauseTimer = new Timer(0f, 0f);

        // The amount of time the platform pauses
        [SerializeField]
        protected float m_PauseDuration = 0f;

        // The speed with which the platform moves at.
        [SerializeField] 
        protected float m_Speed = 4.5f;

        // The sound that plays when the platform starts moving.
        [SerializeField]
        private AudioClip m_StartMovingSound = null;

        // The sound that plays when the platform stops moving.
        [SerializeField]
        private AudioClip m_StopMovingSound = null;

        #endregion

        #region Methods.

        // Initalizes from the LDtk files.
        public override void Init(int length, Vector3[] path) {
            base.Init(length, path);
            m_PauseTimer.Start(m_PauseDuration);
        }
        
        // Runs once every frame.
        // Having to do this is a bit weird.
        void Update() {
            m_Origin = transform.position;
            base.Update();
        }

        // Runs once every fixed interval.
        private void FixedUpdate() {
            transform.Move(m_Path[m_PathIndex], m_Speed, Time.fixedDeltaTime, m_CollisionContainer);
            SetTarget(Time.fixedDeltaTime);
        }

        // Sets the target for this platform.
        private void SetTarget(float dt) {
            float distance = (m_Path[m_PathIndex] - transform.position).magnitude;
            if (distance == 0f && m_PauseTimer.Value == m_PauseDuration) {
                Game.Audio.Sounds.PlaySound(m_StopMovingSound);
            }

            bool finished = m_PauseTimer.TickDownIf(dt, distance == 0f);
            bool neverStarted = distance == 0f && m_PauseDuration == 0f;
            if (finished || neverStarted) {
                Game.Audio.Sounds.PlaySound(m_StartMovingSound);

                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
                m_PauseTimer.Start(m_PauseDuration);
            }
        }

        #endregion        

    }

}