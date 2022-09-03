// TODO: Clean

/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Platformer.Decor;
using Platformer.Utilites;
using Platformer.Obstacles;
using Platformer.Character;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

    ///<summary>
    /// An orb that changes the player's active abilities.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [DefaultExecutionOrder(1000)]
    public class Orb : MonoBehaviour {

        #region Enumerations

        public enum Type {
            None, 
            DashOrb, 
            HopOrb, 
            GhostOrb, 
            ShadowOrb, 
            StickyOrb
        }

        #endregion

        #region Variables

        // Constants.
        private const float ResetDelay = 3.5f;

        // Components.
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();
        
        // The type of this orb.
        [SerializeField] protected Type m_Type;
        [SerializeField] protected ColorPalette m_Palette;
        public ColorPalette Palette => m_Palette;

        // Movement.
        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        [SerializeField] private float m_Period = 3f;
        [SerializeField] private Vector2 m_Ellipse = new Vector2(0f, 2f/16f);

        // Sounds.
        [SerializeField] private AudioClip m_CollectSound;
        [SerializeField] private AudioClip m_RefreshSound;

        #endregion

        // Runs once before the first frame.
        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
            m_Palette.SetSimple(m_SpriteRenderer.material);
        }

        // Runes once every fixed interval.
        void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
        }

        // Procession collisions with this orb.
        void OnTriggerEnter2D(Collider2D collider) {
            CharacterState state = collider.GetComponent<CharacterState>();
            if (state != null && state.IsPlayer) {
                Collect(state);
            }
        }

        // Collects this orb when the character interacts with it.
        void Collect(CharacterState state) {
            
            // Visual and sonic feedback.
            Game.RampStop(16);
            Game.MainPlayer.ExplodeDust.Activate();
            SoundManager.PlaySound(m_CollectSound, 0.05f);
            Screen.Recolor(m_Palette);

            // Reset the players abilities.
            state.DisableAllAbilityActions();

            // Enable the appropriate ability.
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

            // Disable this orb.
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
            StartCoroutine(IEReset());

        }

        // Reset after a delay.
        IEnumerator IEReset() {

            // Cache these settings.
            float ratio = 7f / 16f;
            int count = 6;
            Color tmp0 = m_SpriteRenderer.color;
            Color tmp1 = tmp0;

            // Wait a little before before beginning the flash effect.
            yield return new WaitForSeconds(ratio * ResetDelay);

            // Start the flashing.
            tmp1.a = 0.2f;
            m_SpriteRenderer.color = tmp1; 
            for (int i = 0; i < count; i++) {
                tmp1.a += 0.075f;
                m_SpriteRenderer.color = tmp1; 
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                yield return new WaitForSeconds(ratio * ResetDelay / (float)count);

            }

            // End the flashing.
            m_SpriteRenderer.color = tmp0;
            m_SpriteRenderer.enabled = true;
            SoundManager.PlaySound(m_RefreshSound, 0.15f);

            // Wait a little before re-enabling the hitbox.
            yield return new WaitForSeconds(ResetDelay * (1f - 2f * ratio));
            
            // Reenable the hitbox.
            m_Hitbox.enabled = true;
            yield return null;
            
        }
        
    }

}