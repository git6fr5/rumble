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

        //
        [SerializeField]
        private AudioClip m_MovingRightAnimation = null;

        //
        [SerializeField]
        private AudioClip m_MovingLeftAnimation = null;

        // Th
        [SerializeField]
        public TransformAnimation m_IdleAnimation;

        // The sound that plays when the platform stops moving.
        [SerializeField]
        private AudioClip m_StopMovingSound = null;

        #endregion

        public const float LEG_SPACING = 1f;
        public Leg leg;
        public List<Leg> legs = new List<Leg>();

        public TransformAnimation m_MovingLegAnimation;
        public TransformAnimation m_IdleAnimation;

        public override void SetLength(int length) {
            base.SetLength(length);

            if (length <= 2) {
                leg.transform.localPosition += (length - 1f) * new Vector3(0.5f, 0.2f, 0f);
                leg.gameObject.SetActive(true);
                legs.Add(leg);
                return;
            }

            float d = spacing;
            float a = AdjustedLength - 0.5f - spacing;
            legs = new List<Decorations.DecorationController>();
            while (d < a) {
                
                GameObject newLeg = Instantiate(leg.gameObject);
                newLeg.transform.SetParent(transform);
                newLeg.transform.localPosition = Vector3.right * d + Vector3.up * leg.transform.localPosition.y;
                newLeg.SetActive(true);
                d += spacing;

                legs.Add(newLeg.GetComponent<Decorations.DecorationController>());

            }

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

                StopLegAnimation();
            }

            bool finished = m_PauseTimer.TickDownIf(dt, distance == 0f);
            bool neverStarted = distance == 0f && m_PauseDuration == 0f;
            if (finished || neverStarted) {
                Game.Audio.Sounds.PlaySound(m_StartMovingSound);

                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
                m_PauseTimer.Start(m_PauseDuration);

                SetLegAnimation();   
            }

        }

        public void StopLegAnimation() {
            foreach (Decorations.DecorationController leg in legs) {
                leg.enabled = false;
                for (int i = 0; i < leg.AnimatedPieces.Length; i++) {
                    leg.AnimatedPieces[i].transform.Reset();
                }   
            } 
        }

        public void SetLegAnimation() {
            Vector3 direction = m_Path[m_PathIndex] - transform.position;
            float rotationScale = direction.x > 0 ? 1f : -1f;
            float scaleScale = -1f * rotationScale;
            float posScale = rotationScale;

            for (int n = 0; n < legs.Count; n++) {

                Decorations.DecorationController leg = legs[n];

                leg.enabled = true;
                for (int i = 0; i < leg.AnimatedPieces.Length; i++) {
                    
                    leg.AnimatedPieces[i].Animation.RotationScale = Mathf.Abs(leg.AnimatedPieces[i].Animation.RotationScale) * rotationScale;
                    leg.AnimatedPieces[i].Animation.ScaleScale = Mathf.Abs(leg.AnimatedPieces[i].Animation.ScaleScale) * scaleScale;
                    
                    float _posScale = i % 2 == 0 ? posScale : -posScale; 
                    leg.AnimatedPieces[i].Animation.PositionScale = Mathf.Abs(leg.AnimatedPieces[i].Animation.PositionScale) * _posScale;
                    
                    float maxVal = leg.AnimatedPieces[i].Animation.AnimationTimer.MaxValue;
                    
                    float incMax = 0.5f * maxVal; // 2f / 3f;
                    float baseVal = i % 2 == 0 ? maxVal / 2f : 0f;
                    float incVal = ((float)n / legs.Count) * incMax;
                    if (direction.x > 0f) {

                        baseVal = maxVal / 2f - baseVal;
                        incVal = incMax - incVal;
                    }
                    
                    leg.AnimatedPieces[i].Animation.AnimationTimer.Set(baseVal + incVal);
                }   
            }
        }
        
    }

    

}
