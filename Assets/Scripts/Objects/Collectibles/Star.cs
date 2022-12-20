// TODO: Clean

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Utilities;
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

        private static float ResetDelay = 1.5f;

        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        [SerializeField] private float m_Period = 3f;
        [SerializeField] private Vector2 m_Ellipse = new Vector2(0f, 2f/16f);

        [SerializeField] private VisualEffect m_CollectEffect;
        [SerializeField] private AudioClip m_CollectSound;
        [SerializeField] private VisualEffect m_RefreshEffect;
        [SerializeField] private AudioClip m_RefreshSound;

        [SerializeField] private Transform m_Follow = null;
        public Transform Following => m_Follow;

        public static void CollectAllFollowing(Transform transform) {
            Star[] stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
            for (int i = 0; i < stars.Length; i++) {
                if (stars[i].Following == transform) {
                    stars[i].Collect();
                }
            }
        }

        public static void ResetAll() {
            Star[] stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
            for (int i = 0; i < stars.Length; i++) {
                stars[i].Reset();
            }
        }

        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
        }

        void FixedUpdate() {
            if (m_Follow == null) {
                Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
                Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
            }
            else {
                float mag = (m_Follow.position - transform.position).magnitude;
                Obstacle.Move(transform, m_Follow.position, mag * 5f, Time.fixedDeltaTime);
            }
        }

        void OnTriggerEnter2D(Collider2D collider) {
            CharacterState state = collider.GetComponent<CharacterState>();
            if (state != null && state.IsPlayer) {
                m_Follow = state.transform;
            }
        }

        public void Collect() {
            // Game.HitStop(8);
            
            if (m_CollectEffect != null) {
                m_CollectEffect.Play();
            }
            SoundManager.PlaySound(m_CollectSound, 0.15f);

            Game.Score.AddStar(this);
            Destroy(gameObject);

        }

        public void Reset() {
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
            m_Follow = null;
            StartCoroutine(IEReset());
        }

        // Reset after a delay.
        IEnumerator IEReset() {
            float ratio = 7f / 16f;
            yield return new WaitForSeconds(ratio * ResetDelay);
            int count = 6;
            Color temp = m_SpriteRenderer.color;
            Color _temp = temp;
            _temp.a = 0.2f;
            m_SpriteRenderer.color = _temp; 
            for (int i = 0; i < count; i++) {
                _temp.a += 0.075f;
                m_SpriteRenderer.color = _temp; 
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                yield return new WaitForSeconds(ratio * ResetDelay / (float)count);

            }
            m_SpriteRenderer.color = temp;
            m_SpriteRenderer.enabled = true;

            if (m_RefreshEffect != null) {
                m_RefreshEffect.Play();
            }
            SoundManager.PlaySound(m_RefreshSound, 0.15f);

            yield return new WaitForSeconds(ResetDelay * (1f - 2f * ratio));
            Regrow();
            yield return null;
        }
        
        protected void Regrow() {
            m_Hitbox.enabled = true;
            m_SpriteRenderer.enabled = true;
            m_SpriteRenderer.color = Color.white; 
        }
        
        
    }
}