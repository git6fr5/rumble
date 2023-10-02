/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public class Firefly : MonoBehaviour {

        [SerializeField]
        private Vector2 m_Origin;

        [SerializeField]
        private float m_ThinkDuration = 0.5f;
        private float m_Ticks;

        [SerializeField]
        private Vector2 m_Direction;

        [SerializeField]
        private Vector2 m_Target;

        [SerializeField]
        private float m_Radius = 5f;

        [SerializeField]
        private float m_Speed = 1f;

        void Start() {
            m_Origin = transform.position;
            m_Target = m_Origin + Random.insideUnitCircle.normalized * m_Radius;
            m_Direction = (m_Target - (Vector2)transform.position).normalized;
            m_Ticks = m_ThinkDuration;
        }

        void FixedUpdate() {
            float dt = Time.fixedDeltaTime;
            transform.position += (Vector3)m_Direction * dt * m_Speed;

            m_Ticks -= dt;
            m_Direction += (m_Target - (Vector2)transform.position).normalized * dt;

            if (m_Ticks <= 0f) {
                m_Direction = m_Direction.normalized;
                m_Target = m_Origin + Random.insideUnitCircle.normalized * m_Radius;
                m_Ticks = m_ThinkDuration;
            }

        }

        void OnDrawGizmos() {
            if (Application.isPlaying) { return; }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_Radius); 

        }

    }

}