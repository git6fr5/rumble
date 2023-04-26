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
    public class ScoreOrb : OrbObject {

        #region Variables.

        // Whether this transform is following something.
        [SerializeField, ReadOnly] 
        private Transform m_Follow = null;
        public Transform Following => m_Follow;

        // Tracks whether this has been collected or not.
        [SerializeField, ReadOnly]
        private bool m_Collected = false;

        // The effect that when this orb is collected.
        [SerializeField] 
        private VisualEffect m_CollectEffect;
        
        // The sound that plays when this orb is collected.
        [SerializeField] 
        private AudioClip m_UseSound;

        #endregion

        #region Methods.

        // Runs once every fixed interval.
        void FixedUpdate() {
            if (m_Follow != null) {
                Vector3 direction = -(m_Follow.position - transform.position).normalized;
                Vector3 followPosition = m_Follow.position + direction;
                float mag = (followPosition - transform.position).magnitude;
                transform.Move(followPosition, mag * 5f, Time.fixedDeltaTime);
            }
        }

        // Sets this star to follow a controller.
        protected override void OnTouch(CharacterController character) {
            // Follows the transform.
            m_Hitbox.enabled = false;

            // Index.
            ScoreOrb[] scoreOrbs = (ScoreOrb[])GameObject.FindObjectsOfType(typeof(ScoreOrb));
            ScoreOrb scoreOrb = FindScoreOrbFollowing(scoreOrbs, character.transform);
            if (scoreOrb == null) {
                m_Follow = character.transform;
            }
            else {
                ScoreOrb cachedScoreOrb = scoreOrb;
                while (scoreOrb != null) {
                    cachedScoreOrb = scoreOrb;
                    scoreOrb = FindScoreOrbFollowing(scoreOrbs, scoreOrb.transform);
                }
                m_Follow = cachedScoreOrb.transform;
            }

            // The feedback on touching a star.
            base.OnTouch(character);
        }

        public static ScoreOrb FindScoreOrbFollowing(ScoreOrb[] scoreOrbs, Transform transform) {
            for (int i = 0; i < scoreOrbs.Length; i++) {
                if (scoreOrbs[i].Following == transform) { 
                    return scoreOrbs[i];
                }
            }
            return null;
        }

        // Collects all stars that are currently following the given transform.
        public static void CollectAllFollowing(Transform transform) {
            ScoreOrb[] scoreOrbs = (ScoreOrb[])GameObject.FindObjectsOfType(typeof(ScoreOrb));
            for (int i = 0; i < scoreOrbs.Length; i++) {
                if (scoreOrbs[i].Following == transform) {

                    List<ScoreOrb> collection = new List<ScoreOrb>();
                    collection.Add(scoreOrbs[i]);
                    
                    ScoreOrb nextOrb = FindScoreOrbFollowing(scoreOrbs, scoreOrbs[i].transform);
                    while (nextOrb != null) {
                        collection.Add(nextOrb);
                        nextOrb = FindScoreOrbFollowing(scoreOrbs, nextOrb.transform);
                    }
                    
                    float time = 0.2f;
                    for (int j = collection.Count-1; j >= 0; j--) {
                        collection[j].DelayedCollect(time);
                        time += 0.2f;
                    }

                }
            }
        }

        public void DelayedCollect(float delay) {
            if (!m_Collected) {
                Invoke("Collect", delay);
            }
            m_Collected = true;
        }

        // Collects this orb.
        public void Collect() {
            // The feedback on collecting a star.
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sounds.PlaySound(m_UseSound, 0.05f);
            Game.Visuals.Effects.PlayEffect(m_CollectEffect);

            // Destroy the star once its been collected.
            Game.Level.AddPoint(this);
            m_Hitbox.enabled = false;
            m_SpriteRenderer.enabled = false;
            m_Follow = null;
            m_Collected = true;
        }

        public override void Reset() {
            if (!m_Collected) {
                m_Follow = null;
                transform.position = m_Origin;
                base.Reset();
            }
        }
        
        #endregion
        
    }
}