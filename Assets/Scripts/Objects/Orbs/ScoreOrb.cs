// TODO: Clean

/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Orbs;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(CircleCollider2D))]
    public class ScoreOrb : OrbObject {

        #region Variables.

        // Whether this transform is following something.
        [SerializeField] 
        private Transform m_Follow = null;
        public Transform Following => m_Follow;

        // The effect that when this orb is collected.
        [SerializeField] 
        private VisualEffect m_CollectEffect;
        
        // The sound that plays when this orb is collected.
        [SerializeField] 
        private AudioClip m_CollectSound;

        #endregion

        #region Methods.

        // Runs once every fixed interval.
        protected override void FixedUpdate() {
            if (m_Follow == null) {
                base.FixedUpdate();
            }
            else {
                float mag = (m_Follow.position - transform.position).magnitude;
                transform.Move(m_Follow.position, mag * 5f, Time.fixedDeltaTime);
            }
        }

        // Sets this star to follow a controller.
        protected override void OnTouch(CharacterController controller) {
            // Follows the transform.
            m_Follow = controller.transform;
            m_Hitbox.enabled = false;

            // The feedback on touching a star.
            base.OnTouch(controller);
        }

        // Collects all stars that are currently following the given transform.
        public static void CollectAllFollowing(Transform transform) {
            ScoreOrb[] scoreOrbs = (ScoreOrb[])GameObject.FindObjectsOfType(typeof(ScoreOrb));
            for (int i = 0; i < scoreOrbs.Length; i++) {
                if (scoreOrbs[i].Following == transform) {
                    scoreOrbs[i].Collect();
                }
            }
        }

        // Collects this orb.
        public void Collect() {
            // The feedback on collecting a star.
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sounds.PlaySound(m_CollectSound, 0.05f);
            Game.Visuals.Particles.PlayEffect(m_CollectEffect);

            // Destroy the star once its been collected.
            // Game.Level.AddPoint(this);
            Destroy(gameObject);
        }

        public override void Reset() {
            m_Follow = null;
            transform.position = m_Origin;
            base.Reset();
        }
        
        #endregion
        
    }
}