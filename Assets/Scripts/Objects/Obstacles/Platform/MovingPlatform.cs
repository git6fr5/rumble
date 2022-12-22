/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class MovingPlatform : Platform {

        // The pause timer.
        [HideInInspector]
        protected Timer m_PauseTimer = new Timer(0f, 0f);

        // The amount of time the platform pauses
        [SerializeField]
        protected float m_PauseDuration = 0.2f;

        // The speed with which the platform moves at.
        [SerializeField] 
        protected float m_Speed = 3f;
        
        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
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
            bool finished = m_PauseTimer.TickDownIf(distance == 0f, dt);
            if (finished) {
                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
                m_PauseTimer.Start(m_PauseDuration);
            }
        }
        
    }

}