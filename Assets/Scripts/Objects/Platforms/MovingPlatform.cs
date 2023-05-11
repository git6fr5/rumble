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
using IPathable = Platformer.Levels.Entities.IPathable;
using PathingData = Platformer.Levels.Entities.PathingData;
using Leg = Platformer.Objects.Decorations.DecorationController;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    public class MovingPlatform : PlatformObject, IPathable {

        #region Variables.

        // The path that this platform follows.
        protected Vector3[] m_Path = null;
        
        [SerializeField]
        private PlatformLegs m_Legs = null;

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

        #endregion

        public override void SetLength(int length) {
            base.SetLength(length);
            m_Legs.CreateLegs(length, HitboxLength);
        }

        public void SetPath(PathingData pathingData) {
            // Convert the start and end nodes into world positions.
            m_Path = new Vector3[2];

            m_Path[0] = m_Origin;
            if (pathingData.Direction.x != 0f) {
                m_Path[1] = m_Origin + (pathingData.Distance - m_Length) * (Vector3)pathingData.Direction;
            }
            else {
                m_Path[1] = m_Origin + pathingData.Distance * (Vector3)pathingData.Direction;
            }
            m_PauseTimer.Start(m_PauseDuration);

        }
        
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
            float distance = ((Vector2)m_Path[m_PathIndex] - (Vector2)transform.position).magnitude;
            if (distance == 0f && m_PauseTimer.Value == m_PauseDuration) {
                Game.Audio.Sounds.PlaySound(m_StopMovingSound);
                m_Legs.SetLegAnimation(m_Legs.IdleAnimation, m_Legs.IdleAnimation);   
            }

            bool finished = m_PauseTimer.TickDownIf(dt, distance == 0f);
            bool neverStarted = distance == 0f && m_PauseDuration == 0f;
            if (finished || neverStarted) {
                Game.Audio.Sounds.PlaySound(m_StartMovingSound);

                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
                m_PauseTimer.Start(m_PauseDuration);
                
                float direction = ((Vector2)m_Path[m_PathIndex] - (Vector2)transform.position).x;
                m_Legs.SetAnimationDirection(m_Legs.FrontLegAnim, direction, 1f);
                m_Legs.SetAnimationDirection(m_Legs.BackLegAnim, direction, -1f);
                m_Legs.SetLegAnimation(m_Legs.FrontLegAnim, m_Legs.BackLegAnim);
            }

        }
        
    }

    

}
