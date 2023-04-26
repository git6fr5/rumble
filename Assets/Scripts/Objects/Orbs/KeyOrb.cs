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
    public class KeyOrb : OrbObject {

        #region Variables.

        /* --- Constants --- */

        // A factor to multiply the distance by to get the speed. 
        private const float SPEED_FACTOR = 5f;

        /* --- Members --- */

        // Whether this transform is following something.
        [SerializeField, ReadOnly] 
        private Transform m_Follow = null;
        public Transform Following => m_Follow;

        // Tracks whether this has been collected or not.
        [SerializeField, ReadOnly]
        private bool m_Used = false;
        // The effect that when this orb is collected.
        [SerializeField] 
        private VisualEffect m_UseEffect;
        // The sound that plays when this orb is collected.
        [SerializeField] 
        private AudioClip m_UseSound;

        #endregion

        #region Methods.

        // Runs once every fixed interval.
        void FixedUpdate() {
            // Only override the movement of the orb if it is following something.
            if (m_Follow == null) {
                // base.FixedUpdate();
            }
            else {
                // Get the direction from the object being followed.
                Vector3 direction = -(m_Follow.position - transform.position).normalized;
                // Get the position that we should go to, based on the object being followed 
                // This is not simply the followed object's position, but slightly lagging behind. 
                Vector3 followPosition = m_Follow.position + direction;
                // Get the appropiate speed based on how far the followed object is 
                float speed = (followPosition - transform.position).magnitude * SPEED_FACTOR;
                transform.Move(followPosition, speed, Time.fixedDeltaTime);
            }
        }

        // Sets this star to follow a controller.
        protected override void OnTouch(CharacterController character) {
            // Follows the transform.
            m_Hitbox.enabled = false;

            // Index.
            KeyOrb[] keyOrbs = (KeyOrb[])GameObject.FindObjectsOfType(typeof(KeyOrb));
            KeyOrb keyOrb = FindKeyOrbFollowing(keyOrbs, character.transform);
            if (keyOrb == null) {
                m_Follow = character.transform;
            }
            else {
                KeyOrb cachedKeyOrb = keyOrb;
                while (keyOrb != null) {
                    cachedKeyOrb = keyOrb;
                    keyOrb = FindKeyOrbFollowing(keyOrbs, keyOrb.transform);
                }
                m_Follow = cachedKeyOrb.transform;
            }

            // The feedback on touching a star.
            base.OnTouch(character);
        }

        public static KeyOrb FindKeyOrbFollowing(KeyOrb[] keyOrbs, Transform transform) {
            for (int i = 0; i < keyOrbs.Length; i++) {
                if (keyOrbs[i].Following == transform) { 
                    return keyOrbs[i];
                }
            }
            return null;
        }

        // Collects all stars that are currently following the given transform.
        public static bool UseFirstFollowing(Transform transform) {
            KeyOrb[] keyOrbs = (KeyOrb[])GameObject.FindObjectsOfType(typeof(KeyOrb));
            
            KeyOrb cachedKeyOrb = null;
            for (int i = 0; i < keyOrbs.Length; i++) {
                if (keyOrbs[i].Following == transform) {
                    cachedKeyOrb = keyOrbs[i];
                    break;
                }
            }

            if (cachedKeyOrb == null) {
                return false;
            }

            KeyOrb nextOrb = FindKeyOrbFollowing(keyOrbs, cachedKeyOrb.transform);
            if (nextOrb != null) {
                nextOrb.SetFollowing(transform);
            }

            cachedKeyOrb.Use();
            return true;
        }

        public void SetFollowing(Transform transform) {
            m_Follow = transform;
        }

        // Collects this orb.
        public void Use() {
            // The feedback on collecting a star.
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sounds.PlaySound(m_UseSound, 0.05f);
            Game.Visuals.Effects.PlayEffect(m_UseEffect);

            // Destroy the star once its been collected.
            m_Hitbox.enabled = false;
            m_SpriteRenderer.enabled = false;
            m_Follow = null;
            m_Used = true;
        }

        public override void Reset() {
            if (!m_Used) {
                m_Follow = null;
                transform.position = m_Origin;
                base.Reset();
            }
        }
        
        #endregion
        
    }
}