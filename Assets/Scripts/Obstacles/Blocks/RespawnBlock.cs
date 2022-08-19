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

        [SerializeField] private Sprite m_ActiveSprite;
        [SerializeField] private Sprite m_InactiveSprite;

        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();

        [SerializeField] private AudioClip m_ActivateSound;

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
            // Timer.CountdownTicks(ref m_ActivationTicks, true, ActivationBuffer, 0f);
            // m_ActivationEffect.Play();

            if (Game.MainPlayer.Respawn != this) {
                SoundManager.PlaySound(m_ActivateSound, 0.15f);
            } 
            character.SetResetPoint(this);

        }

        private void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);

            if (Game.MainPlayer.Respawn == this) {
                m_SpriteRenderer.sprite = m_ActiveSprite;
            }
            else {
                m_SpriteRenderer.sprite = m_InactiveSprite;
            }
            // Timer.CountdownTicks(ref m_ActivationTicks, false, ActivationBuffer, Time.fixedDeltaTime);

        }
        
    }
}