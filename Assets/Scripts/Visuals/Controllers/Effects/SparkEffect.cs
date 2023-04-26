/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

namespace Platformer.Visuals.Effects {

    ///<summary>
    ///
    ///<summary>
    public class SparkEffect : Effect {

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
        private const float OPACITY_FACTOR = 5f;

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
        protected float m_Speed = 0.75f;   

        // Basically, if this is off, the timer keeps restarting so it it all coagulates at the center.
        [SerializeField] 
        protected bool m_Coagulate = false;   

        // The position where this impact occurs.
        [HideInInspector] 
        private Vector3 m_Origin = Vector3.zero;

        [SerializeField]
        private Timer m_Timer = new Timer(0f, 1f);

        [SerializeField, ReadOnly]
        private Dictionary<SpriteRenderer, Vector2> m_Directions = new Dictionary<SpriteRenderer, Vector2>();


        #endregion

        // Plays the bubble effect for the given duration.
        public void Play(Sprite sprite, int count, float speed, float rate, bool coagulate) {
            m_Count = count;
            m_Speed = speed;
            m_Sprite = sprite != null ? sprite : m_Sprite;
            m_Coagulate = coagulate;
            m_Timer = new Timer(0f, 1f / rate);
            Play();
        }

        // Plays the impact effect.
        public override void Play() {
            m_Origin = transform.position;
            for (int i = 0; i < m_Count; i++) {
                m_Particles.Spawn("Impact", transform, m_Sprite, m_Radius, MIN_SORTING_ORDER, MAX_SORTING_ORDER, 0.5f, 2f);
            }
            for (int i = 0; i < m_Particles.Count; i++) {
                m_Directions.Add(m_Particles[i], Random.insideUnitCircle.normalized);
            }
            m_Timer.Start();
            base.Play();
        }

        // Runs once every fixed interval.
        private void FixedUpdate() {
            m_Particles.RemoveAll(particles => particles == null);
            m_Particles.FadeOut(FADE_SPEED, FADE_THRESHOLD, Time.fixedDeltaTime);
            m_Particles.MoveIndividually(m_Directions, m_Speed, Time.fixedDeltaTime);
            m_Particles.Rotate(ROTATION_SPEED, Time.fixedDeltaTime);

            bool finished = m_Timer.TickDown(Time.fixedDeltaTime);
            if (finished) {
                Dictionary<SpriteRenderer, Vector2> dict = new Dictionary<SpriteRenderer, Vector2>();
                foreach (KeyValuePair<SpriteRenderer, Vector2> kv in m_Directions) {
                    if (kv.Key != null) {
                        dict.Add(kv.Key, Random.insideUnitCircle.normalized);
                    }

                }
                if (!m_Coagulate) {
                    m_Timer.Start();
                }
                m_Directions = dict;
            }
        }

    }

}