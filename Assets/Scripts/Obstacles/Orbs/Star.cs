// TODO: Clean

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Utilites;
using Platformer.Obstacles;
using Platformer.Character;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Star : MonoBehaviour {

        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        [SerializeField] private float m_Period = 3f;
        [SerializeField] private Vector2 m_Ellipse = new Vector2(0f, 2f/16f);

        [SerializeField] private VisualEffect m_CollectEffect;
        [SerializeField] private AudioClip m_CollectSound;

        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
        }

        void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            CharacterState state = collider.GetComponent<CharacterState>();
            if (state != null && state.IsPlayer) {
                Collect(state);
            }
        }

        void Collect(CharacterState state) {
            // Game.HitStop(8);
            
            if (m_CollectEffect != null) {
                m_CollectEffect.Play();
            }
            SoundManager.PlaySound(m_CollectSound, 0.15f);

            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;

        }
        
        
    }
}