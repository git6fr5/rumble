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

namespace Platformer.Objects.Spikes {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class FallingSpike : SpikeObject {

        #region Enumerations.

        public enum FallState {
            Looking, 
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
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();

        /* --- Members --- */

        // The current fall state of this falling spike.
        [SerializeField]
        private FallState m_FallState = FallState.Looking;

        // The duration this crumbles for before falling.
        [SerializeField] 
        private float m_CrumbleDuration;
        
        // Tracks how long this is crumbling for
        [SerializeField] 
        private Timer m_CrumbleTimer = new Timer(0f, 0f);

        // The strength with which this shakes while crumbling
        [SerializeField] 
        private float m_ShakeStrength;
        private float Strength => m_ShakeStrength * m_CrumbleTimer.InverseRatio;

        // public Sparkle m_Sparkle;
        
        #endregion
        
        // Runs once every frame.
        private void Update() {

            // What to do for each state.
            switch (m_FallState) {
                case FallState.Looking:
                    WhileLooking();
                    break;
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

        // Runs when another collider enters the trigger area.
        protected override void OnTriggerEnter2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                character.Reset();
                Shatter();
            }
            else if (m_FallState == FallState.Falling) {
                bool hitGround = collider.gameObject.layer == LayerMask.NameToLayer("Ground");
                bool hitPlatform = collider.gameObject.layer == LayerMask.NameToLayer("Platform");
                if (hitGround || hitPlatform) {
                    Shatter();
                }
            }
        }

        private void OnFall() {
            // m_TrailSparkle.enabled = true;
            transform.position = m_Origin + Game.Physics.Collisions.CollisionPrecision * Vector3.down;
            m_Body.ReleaseXY();
            m_Body.SetWeight(WEIGHT);
            m_FallState = FallState.Falling;
        }

        protected override void Shatter() {
            base.Shatter();
            transform.position = m_Origin;
            m_Body.Stop();
            m_Body.Freeze();
            m_FallState = FallState.Reforming;
        }

        private void WhileLooking() {
            CharacterController character = Game.Physics.Collisions.LineOfSight<CharacterController>(transform.position, Vector3.down, Game.Physics.CollisionLayers.Opaque);
            if (character != null) {
                m_FallState = FallState.Crumbling;
                m_CrumbleTimer.Start(m_CrumbleDuration);
            }
        }

        public override void Reset() {
            base.Reset();
            m_Body.Stop();
            m_Body.Freeze();
            m_FallState = FallState.Looking;
        }

    }

}
