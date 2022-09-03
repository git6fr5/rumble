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

using Platformer.Decor;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [DefaultExecutionOrder(1000)]
    public class Orb : MonoBehaviour {

        private static float ResetDelay = 3.5f;

        public enum Type {
            None, DashOrb, HopOrb, GhostOrb, ShadowOrb, StickyOrb
        }

        [SerializeField] protected ColorPalette m_Palette;
        public ColorPalette Palette => m_Palette;

        [SerializeField] protected Type m_Type;

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

        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
            m_Palette.SetSimple(m_SpriteRenderer.material);
        }

        float ticks = 0f;
        float duration = 0.5f;

        void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
            
            ticks -= Time.fixedDeltaTime;
            if (ticks < 0f) {
                // Game.ParticleGrid.Impulses(transform.position, 1e4f, 5f, 0.5f, 3);
                ticks = duration;
                // dustA.Activate();
            }
        }

        void OnTriggerEnter2D(Collider2D collider) {
            CharacterState state = collider.GetComponent<CharacterState>();
            if (state != null && state.IsPlayer) {
                Collect(state);
            }
        }

        void Collect(CharacterState state) {
            Game.MainPlayer.ExplodeDust.Activate();
            
            Game.HitStop(8);
            state.OverrideFall(false);
            state.OverrideMovement(false);
            state.DisableAllAbilityActions();

            switch (m_Type) {
                case Type.DashOrb:
                    state.Dash.Enable(state, true);
                    break;
                case Type.HopOrb:
                    state.Hop.Enable(state, true);
                    break;
                case Type.GhostOrb:
                    state.Ghost.Enable(state, true);
                    break;
                case Type.ShadowOrb:
                    state.Shadow.Enable(state, true);
                    break;
                case Type.StickyOrb:
                    state.Sticky.Enable(state, true);
                    break;
                default:
                    break;
            }

            if (m_CollectEffect != null) {
                m_CollectEffect.Play();
            }
            SoundManager.PlaySound(m_CollectSound, 0.05f);

            Screen.Recolor(m_Palette);
            
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
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
        }
        
    }
}