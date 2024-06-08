/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Gobblefish;
using Platformer.Physics;

/* --- Definitions --- */
using PathingNode = Platformer.Entities.Utility.PathingNode;
using Reset = Platformer.Entities.Utility.Reset;
using IReset = Platformer.Entities.Utility.IReset;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Pathing : MonoBehaviour, IReset{

        public enum PathingState {
            Moving,
            Waiting,
        }

        public PathingState m_PathState;

        public Entity m_Entity;

        // The path that this platform follows.
        [SerializeField]
        private PathingNode[] m_Nodes;
        public PathingNode[] nodes => m_Nodes;

        [SerializeField]
        private bool triggerNodeEvents = false;
        
        // The current position in the path that the path is following.
        [SerializeField, ReadOnly] 
        protected int m_PathIndex;

        // The pause timer.
        [SerializeField]
        protected Timer m_PathTimer = new Timer(0f, 0f);

        // The amount of time the platform pauses
        // [SerializeField]
        // protected float PauseDuration = 0.5f;
        protected float PauseDuration => 1f / m_Speed;

        // The speed with which the platform moves at.
        [SerializeField] 
        protected float m_Speed = 3f;

        // The speed with which the platform moves at.
        [SerializeField] 
        protected Vector3 m_Direction = new Vector3(0f, 0f, 0f);

        // Incase this has an elongatable length.
        public Elongatable m_Elongatable;

        private Reset reset;
        private MovementAnimator movementAnimator;
        public Vector3 currentTargetPos => m_Nodes[m_PathIndex].Position;

        [HideInInspector] public int speedLevel;

        void Start() {

            m_PathState = PathingState.Moving; 
            Wait();

            foreach (Transform child in transform) {
                if (child.GetComponent<Power>() != null) {
                    m_Speed /= 2f;
                }
                if (child.GetComponent<MovementAnimator>() != null) {
                    movementAnimator = child.GetComponent<MovementAnimator>();
                    // movementAnimator.RecalculateDistance(m_Nodes[0], m_Nodes[1]);
                }
            }

            for (int i = 0; i < m_Nodes.Length; i++) {
                m_Nodes[i].transform.parent = transform.parent;
            }

            Entity entity = GetComponentInChildren<Entity>();
            if (entity != null) {
                m_Elongatable = entity.GetComponent<Elongatable>();
            }

        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_PathTimer.TickDown(Time.fixedDeltaTime);

            // Whenever the path timer hits 0.
            if (finished) {

                switch (m_PathState) {
                    case PathingState.Waiting:
                        Next();
                        break;
                    case PathingState.Moving:
                        Wait();
                        break;
                    default:
                        break;
                }

            }

            // What to do for each state.
            switch (m_PathState) {
                case PathingState.Moving:
                    Move(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        // Runs once every fixed interval.
        private void Move(float dt) {
            float ds = m_Speed * dt;
            if (movementAnimator != null) {
                float _dt = dt / m_PathTimer.MaxValue;
                ds = movementAnimator.GetStepDistance(m_PathTimer.InverseRatio, _dt);
            }
            transform.position += ds * m_Direction; // , 1f, m_Entity?.CollisionContainer);
        }

        // Sets the target for this platform.
        private void Next() {
            int prevIndex = m_PathIndex;
            m_PathIndex = (m_PathIndex + 1) % m_Nodes.Length;
            if (movementAnimator != null) {
                movementAnimator.RecalculateDistance(m_Nodes[prevIndex].Position, m_Nodes[m_PathIndex].Position);
            }
            Vector3 displacement = (m_Nodes[prevIndex].Position-m_Nodes[m_PathIndex].Position);
            m_PathTimer.Start(displacement.magnitude/m_Speed);
            m_Direction = -displacement.normalized;
            m_PathState = PathingState.Moving;
        }

        private void Wait() {
            transform.position = m_Nodes[m_PathIndex].Position;
            if (triggerNodeEvents) {
                m_Nodes[m_PathIndex].OnReached.Invoke(m_PathIndex);
            }
            m_PathTimer.Start(PauseDuration);
            m_PathState = PathingState.Waiting;
        }

        public bool debugPath = true;
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

        public void SetEntity(Entity entity) {
            m_Entity = entity;
        }

        public void SetPath(List<PathingNode> path, int speedLevel, float speed = 3f) {
            m_Nodes = path.ToArray();
            m_Speed = speed + (2f / 3f * speed * speedLevel);
            // speedLevel = path.speedLevel;
        }

        public void OnStartResetting() {
            print("is this being called");
            // m_PathIndex = 0;
            // transform.position = currentTargetPos;
        }

        public void OnFinishResetting() {
            // m_PathState = FallState.Stable;
            m_PathIndex = 0;
            transform.localPosition = currentTargetPos;
            // m_PauseTimer = new Timer(0f, 0f);
        }

    }

}
