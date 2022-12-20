/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilities;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class RespawnBlock : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount above the respawn block that this respawns people at.
        public const float RESPAWN_HEIGHT = 1.5f;

        // The amount of time before something is respawned.
        public const float RESPAWN_DELAY = 0.12f;

        /* --- Members --- */

        // The position at which things are respawned for this block.
        public Vector3 RespawnPosiiton => transform.position + Vector3.up * RESPAWN_HEIGHT;

        #endregion

        public bool Active => Game.MainPlayer.Respawn == this;

        [SerializeField] private float m_Period;
        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField] private Vector2 m_Ellipse;

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        public Vector3 Origin => m_Origin;

        [SerializeField] private Sprite m_ActiveSprite;
        [SerializeField] private Sprite m_InactiveSprite;

        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();

        [SerializeField] private AudioClip m_ActivateSound;

        [SerializeField] private float m_FirstActivationTime = Mathf.Infinity;
        [SerializeField] private float m_TotalActivatedTime = 0f;
        public float FirstActivationTime => m_FirstActivationTime;
        public float TotalTimeActive => m_TotalActivatedTime;

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
            m_FirstActivationTime = Game.Ticks < m_FirstActivationTime ? Game.Ticks : m_FirstActivationTime;

            if (!Active) { SoundManager.PlaySound(m_ActivateSound, 0.15f); } 
            character.SetResetBlock(this);
        }

        private void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);

            if (Active) {
                m_TotalActivatedTime += Time.fixedDeltaTime;
                m_SpriteRenderer.sprite = m_ActiveSprite;
            }
            else {
                m_SpriteRenderer.sprite = m_InactiveSprite;
            }
            // Timer.CountdownTicks(ref m_ActivationTicks, false, ActivationBuffer, Time.fixedDeltaTime);

        }
        
    }
}