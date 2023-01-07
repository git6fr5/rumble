/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Spikes {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Collider2D))]
    public class SpikeObject : MonoBehaviour {

        #region Variables

        /* --- Constants --- */
        
        // The amount of time before the orb naturally respawns.
        private const float RESET_DELAY = 1.5f;

        // The amount of time before it starts blinking.
        private const float RESET_BLINK_DELAY = 0.7f;

        // The amount of times the orb blinks before it reappears.
        private const float RESET_BLINK_COUNT = 3;

        // The opacity of the orb when it first blinks back into screen.
        private const float RESET_BASE_OPACITY = 0.2f;

        // The opacity increase of the orb per blink.
        private const float RESET_OPACITY_PER_BLINK = 0.075f;

        /* --- Components --- */
        
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        
        protected Collider2D m_Hitbox => GetComponent<Collider2D>();

        /* --- Member --- */

        // The euler angles rotation of this spike.
        public Vector3 Rotation => new Vector3(0f, 0f, m_Rotation);
        
        // The z value rotation of the object.
        [SerializeField] 
        protected float m_Rotation = 0f;
        
        // The base position of the object.
        [SerializeField, ReadOnly] 
        protected Vector3 m_Origin = new Vector3(0f, 0f, 0f);

        // The effect that plays when this spike shatters.
        [SerializeField] 
        private Sprite m_ShatterParticle;
        
        // The effect that plays when the spike shatters.
        [SerializeField] 
        private AudioClip m_ShatterSound;
        
        // The effect that plays when this spike shatters.
        [SerializeField] 
        private Sprite m_RefreshParticle;
        
        // The sound that plays when this orb is reset.
        [SerializeField] 
        private AudioClip m_RefreshSound;

        // The sound that plays when this orb blinks.
        [SerializeField]
        private AudioClip m_BlinkSound;
        
        #endregion

        #region Methods.

        // Runs once before the first frame.
        private void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
            Reset();
        }

        // Initalizes from the LDtk files.
        public virtual void Init(int offset, float rotation, Vector3[] path) {
            m_Rotation = rotation;
            transform.eulerAngles = Rotation;
        }

        // Runs whenever another collider enters into the trigger area.
        protected virtual void OnTriggerEnter2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                character.Reset();
                Shatter();
            }
        }

        protected virtual void Shatter() {
            Game.Visuals.Effects.PlayImpactEffect(m_ShatterParticle, 8, 0.6f, transform, Vector3.zero);
            Game.Audio.Sounds.PlaySound(m_ShatterSound, 0.15f);
            m_Hitbox.enabled = false;
            m_SpriteRenderer.enabled = false;
            StartCoroutine(IEReset());
        }

        // Reset after a delay.
        protected IEnumerator IEReset() {
            yield return new WaitForSeconds(RESET_BLINK_DELAY);

            // Set up the colors.
            Color cacheColor = m_SpriteRenderer.color;
            Color tempColor = m_SpriteRenderer.color;
            tempColor.a = RESET_BASE_OPACITY;
            
            // Blink the orb a couple of times.
            m_SpriteRenderer.color = tempColor; 
            for (int i = 0; i < 2 * RESET_BLINK_COUNT; i++) {
                tempColor.a += RESET_OPACITY_PER_BLINK;
                m_SpriteRenderer.color = tempColor; 
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                Game.Audio.Sounds.PlaySound(m_BlinkSound, 0.05f);
                yield return new WaitForSeconds((RESET_DELAY - RESET_BLINK_DELAY) / (float)(2 * RESET_BLINK_COUNT));
            }
            
            // Reset the orbs color.
            m_SpriteRenderer.color = cacheColor;
            m_SpriteRenderer.enabled = true;

            // Wait one more blink just because it feels more correct.
            yield return new WaitForSeconds((RESET_DELAY - RESET_BLINK_DELAY) / (float)RESET_BLINK_COUNT);

            // Make the orb collectible agains.
            Reset();

            yield return null;
        
        }

        
        public virtual void Reset() {
            Game.Visuals.Effects.PlayImpactEffect(m_RefreshParticle, 8, 0.6f, transform, Vector3.zero);
            Game.Audio.Sounds.PlaySound(m_RefreshSound, 0.15f);
            m_Hitbox.enabled = true;
            m_SpriteRenderer.enabled = true;
        }

        #endregion

    }

}
