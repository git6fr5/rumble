/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

namespace Platformer.Visuals.Effects {

    ///<summary>
    /// Creates a dust impact particle.
    ///<summary>
    public class ImpactEffect : Effect {

        #region Variables.
        
        /* --- Constants --- */

        // The min sorting order that is renderered at.
        private const int MIN_SORTING_ORDER = -100;

        // The max sorting order that is renderered at.
        private const int MAX_SORTING_ORDER = 100;

        // The max sorting order that is renderered at.
        private const float FADE_THRESHOLD = 0.1f;

        // The speed with which this fades.
        private const float FADE_SPEED = 0.15f;   

        // The rotation speed of the particles in this impact.
        private const float ROTATION_SPEED = 5f; 

        // The radius within which particles are spawned in.
        private const float m_Radius = 0.25f;

        /* --- Members --- */

        // The sprite that is used for the particle.
        [SerializeField] 
        private Sprite m_Sprite = null;

        // The list of particles in this impact.
        [SerializeField, ReadOnly] 
        protected List<SpriteRenderer> m_Particles = new List<SpriteRenderer>();

        // The amount of particles that this spawns on impact. 
        [SerializeField] 
        protected int m_Count = 8;   

        // The speed with which the particles in this impact move.
        [SerializeField] 
        protected float m_Speed = 0.15f;   

        // The position where this impact occurs.
        [HideInInspector] 
        private Vector3 m_ImpactOrigin = Vector3.zero;

        #endregion

        #region Methods.

        // Plays the circle effect for the given duration.
        public void Play(Sprite sprite, int count, float speed) {
            m_Count = count;
            m_Speed = speed;
            m_Sprite = sprite;
            Play();
        }

        // Plays the impact effect.
        public override void Play() {
            m_ImpactOrigin = transform.position;
            for (int i = 0; i < m_Count; i++) {
                m_Particles.Spawn("Impact", transform, m_Sprite, m_Radius, MIN_SORTING_ORDER, MAX_SORTING_ORDER, 0.5f, 2f);
            }
            base.Play();
        }

        // Runs once every fixed interval.
        private void FixedUpdate() {
            m_Particles.RemoveAll(particles => particles == null);
            m_Particles.FadeOut(FADE_SPEED, FADE_THRESHOLD, Time.fixedDeltaTime);
            m_Particles.Rotate(ROTATION_SPEED, Time.fixedDeltaTime);
            m_Particles.RadialMove(m_ImpactOrigin, m_Speed, Time.fixedDeltaTime);
            if (m_Particles.Count == 0) {
                Stop();
            }
        }

        // Destroys the impact effect.
        private void OnDestroy() {
            m_Particles.Reset();
        }

        #endregion

    }

}
