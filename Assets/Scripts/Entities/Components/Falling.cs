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
using SpriteAnimator = Platformer.Visuals.Animation.SpriteAnimator;
using SpriteAnimation = Platformer.Visuals.Animation.SpriteAnimation;
using TrailAnimator = Platformer.Visuals.Animation.TrailAnimator;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Falling : MonoBehaviour {

        #region Enumerations.

        public enum FallState {
            Stable, 
            Crumbling, 
            Falling, 
            Reforming
        }

        #endregion

        #region Variables.

        /* --- Constants --- */

        // The weight with which gravity pulls this.
        private static float WEIGHT = 1f;

        /* --- Components --- */

        // The body attached to this gameObject 
        private Rigidbody2D m_Body;

        /* --- Members --- */

        // The current fall state of this falling spike.
        [SerializeField]
        private FallState m_FallState = FallState.Stable;

        // The duration this crumbles for before falling.
        [SerializeField] 
        private float m_CrumbleDuration = 0.5f;
        
        // Tracks how long this is crumbling for
        [SerializeField] 
        private Timer m_CrumbleTimer = new Timer(0f, 0f);

        // The strength with which this shakes while crumbling
        [SerializeField] 
        private float m_ShakeStrength = 0.12f;
        private float Strength => m_ShakeStrength * m_CrumbleTimer.InverseRatio;

        [SerializeField]
        private SpriteAnimator m_SpriteAnimator;

        [SerializeField]
        private SpriteAnimation m_LookingAnimation;

        [SerializeField]
        private SpriteAnimation m_FallingAnimation;

        [SerializeField]
        private Vector3 m_Origin;

        // [SerializeField]
        // private TrailAnimator[] m_Trails;
        // public GameObject m_TrailObject;       

        // public Sparkle m_Sparkle;
        
        #endregion

        // Initialize the spike.
        void Start() {
            m_Origin = transform.position;
            m_Body = GetComponent<Rigidbody2D>();
            m_Body.Stop();
            m_Body.Freeze();
            m_FallState = FallState.Stable;
        }

        // Runs once every frame.
        void Update() {

            // What to do for each state.
            switch (m_FallState) {
                case FallState.Crumbling:
                    transform.Shake(m_Origin, Strength);
                    break;
                default:
                    break;
            }
            
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_CrumbleTimer.TickDown(Time.fixedDeltaTime);

            // When we're finished crumbling.
            if (finished) {

                switch (m_FallState) {
                    case FallState.Crumbling:
                        OnFall();
                        break;
                    default:
                        break;
                }

            }

        }

        public void StartFall(CharacterController character) {
            if (character != null && m_FallState == FallState.Stable) {
                m_FallState = FallState.Crumbling;
                m_CrumbleTimer.Start(m_CrumbleDuration);
                m_SpriteAnimator.SetAnimation(m_FallingAnimation);
                m_SpriteAnimator.SetFrameRate(12);
            }
        }

        private void OnFall() {
            // m_TrailSparkle.enabled = true;
            transform.position = m_Origin + Game.Physics.Collisions.CollisionPrecision * Vector3.down;
            m_Body.ReleaseXY();
            m_Body.SetWeight(WEIGHT);
            m_FallState = FallState.Falling;

            // for (int i = 0; i < m_Trails.Length; i++) {
            //     m_Trails[i].enabled = true;
            // }
            // m_TrailObject.SetActive(true);

            m_SpriteAnimator.SetFrameRate(16);
            m_SpriteAnimator.Play();
        }

        public void Reset() {
            // m_Entity.Reset();
            
            m_Body.Stop();
            m_Body.Freeze();
            m_FallState = FallState.Stable;

            m_SpriteAnimator.SetAnimation(m_LookingAnimation);
            m_SpriteAnimator.SetFrameRate(4);
            // m_Animator.Stop();
        }

    }

}
