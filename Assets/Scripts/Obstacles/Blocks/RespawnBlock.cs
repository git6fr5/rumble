/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class RespawnBlock : MonoBehaviour {

        [SerializeField] private float m_Period;
        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField] private Vector2 m_Ellipse;

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        public Vector3 Origin => m_Origin;


        // [SerializeField] private VisualEffect m_ActivationEffect;
        // [SerializeField, ReadOnly] private float m_ActivationTicks = 0f;
        // private static float ActivationBuffer = 0.5f;

        void Start() {
            m_Origin = transform.position;
        }

        protected void OnTriggerEnter2D(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            if (character != null) {
                Activate(character);
            }
        }

        protected void Activate(CharacterState character) {
            character.SetResetPoint(this);
            // Timer.CountdownTicks(ref m_ActivationTicks, true, ActivationBuffer, 0f);
            // m_ActivationEffect.Play();
        }

        private void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);

            // Timer.CountdownTicks(ref m_ActivationTicks, false, ActivationBuffer, Time.fixedDeltaTime);

        }
        
    }
}