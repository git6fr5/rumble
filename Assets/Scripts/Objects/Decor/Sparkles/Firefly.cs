/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Decor;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class Firefly : Sparkle {
        
        [SerializeField] 
        protected float m_Speed = 0.75f; 

        [SerializeField] 
        private Vector2 m_Direction; 
        
        [SerializeField] 
        private float m_ThinkDuration; 

        [SerializeField] 
        private float m_DirectionTicks = 1f;
        
        [SerializeField] 
        private Vector2 m_Target;

        [SerializeField] 
        private Vector2 m_Origin;

        [SerializeField] 
        private Sparkle m_SecondarySparkle;

        [SerializeField] 
        private bool m_OverrideAndPlay = false;

        void Awake() {
            m_Origin = transform.position;
        }
        
        protected override bool IsActive() {
            bool enabled = (Game.Instance != null && Game.MainPlayer.Dash.Enabled) || m_OverrideAndPlay;
            if (!enabled) {
                m_SecondarySparkle.Reset();
            }
            m_SecondarySparkle.enabled = enabled;
            return enabled;
        }

        // protected override void AdjustPosition(float deltaTime) {
        //     m_Sparkles.RemoveAll(thing => thing == null);
        //     Vector3 deltaPosition = Vector3.up * m_Speed * deltaTime;
        //     for (int i = 0; i < m_Sparkles.Count; i++) {
        //         m_Sparkles[i].transform.position += deltaPosition;
        //     }
        // }

        protected override void ExtraStuff(float dt) {
            transform.position += (Vector3)m_Direction * dt * m_Speed;

            bool changeDir = Timer.TickDown(ref m_DirectionTicks, dt);
            m_Direction += (m_Target - (Vector2)transform.position).normalized * dt;

            if (changeDir) {
                m_Direction = m_Direction.normalized;
                m_Target = m_Origin + Random.insideUnitCircle.normalized * 5f;
                m_DirectionTicks = m_ThinkDuration;
            }

        }

    }

}