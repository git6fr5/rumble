/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using PathingNode = Platformer.Entities.Utility.PathingNode;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Pathing : MonoBehaviour {

        private Entity m_Entity;

        // The path that this platform follows.
        [SerializeField]
        private PathingNode[] m_Nodes;
        
        // The current position in the path that the path is following.
        [SerializeField, ReadOnly] 
        protected int m_PathIndex;

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

        // Used to cache references.
        void Awake() {
            m_Entity = GetComponent<Entity>();
        }

        void Start() {
            for (int i = 0; i < m_Nodes.Length; i++) {
                m_Nodes[i].transform.parent = null;
            }
        }

        // Runs once every fixed interval.
        private void FixedUpdate() {
            transform.Move(m_Nodes[m_PathIndex].Position, m_Speed, Time.fixedDeltaTime, m_Entity.CollisionContainer);
            SetTarget(Time.fixedDeltaTime);
        }

        // Sets the target for this platform.
        private void SetTarget(float dt) {
            // Take a step.
            float distance = ((Vector2)m_Nodes[m_PathIndex].Position - (Vector2)transform.position).magnitude;
            if (distance == 0f && m_PauseTimer.Value == m_PauseDuration) {
                Game.Audio.Sounds.PlaySound(m_StopMovingSound);
            }

            // At an end point.
            bool finished = m_PauseTimer.TickDownIf(dt, distance == 0f);
            bool neverStarted = distance == 0f && m_PauseTimer.MaxValue == 0f;
            if (finished || neverStarted) {
                Game.Audio.Sounds.PlaySound(m_StartMovingSound);
                m_PathIndex = (m_PathIndex + 1) % m_Nodes.Length;
                m_PauseTimer.Start(m_PauseDuration);
            }

        }

        public bool debugPath = false;
        void OnDrawGizmos() {
            if (!debugPath) { return; }
            for (int i = 0; i < m_Nodes.Length; i++) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(m_Nodes[i].transform.position, 0.2f);
                if (i > 0) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(m_Nodes[i-1].transform.position, m_Nodes[i].transform.position);
                }
            }
            Gizmos.DrawLine(m_Nodes[m_Nodes.Length - 1].transform.position, m_Nodes[0].transform.position);

        }

    }

}
