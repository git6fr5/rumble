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
using IRotatable = Platformer.Levels.Entities.IRotatable;

namespace Platformer.Objects.Spikes {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Collider2D))]
    public class SpikeObject : MonoBehaviour, IRotatable {

        #region Variables

        /* --- Constants --- */
        
        // The amount of time before the orb naturally respawns.
        private const float RESET_DELAY = 1.5f;

        // The amount of time before it starts blinking.
        private const float RESET_BLINK_DELAY = 0.7f;

        // The amount of times the orb blinks before it reappears.
        private const float RESET_BLINK_COUNT = 3;

        /* --- Components --- */

        //        
        protected Collider2D m_Hitbox = null;

        //
        protected SpriteRenderer m_SpriteRenderer = null;

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

        // Gets the blink time for this object.
        private float BlinkTime => GetBlinkTime();
        // Gets the time in between blinking and resetting fully.
        private float PreBlinkTime => GetPreBlinkTime(); 
        // Gets the time in between blinking and resetting fully.
        private float PostBlinkTime => GetPostBlinkTime(); 
        
        #endregion

        #region Methods.

        // Initialize the spike.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin.
            transform.position = worldPosition;
            m_Origin = worldPosition;

            // Collision settings.
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_Hitbox.isTrigger = true;
            gameObject.layer = Game.Physics.CollisionLayers.SpikeLayer;

            // Rendering settings.
            m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.SpikeLayer;
            m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.SpikeOrder;

        }

        // Initalizes from the LDtk files.
        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
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

        // A coroutine to eventually reset this orb object.
        protected IEnumerator IEReset() {
            // Wait a little until this orb starts blinking back into existence.
            yield return new WaitForSeconds(PreBlinkTime);
            // Break if the orb has been prematurely reset.
            if (m_Hitbox.enabled) {
                m_SpriteRenderer.enabled = true;
                yield break;
            }

            // Blink the orb a couple of times.
            for (int i = 0; i < 2 * RESET_BLINK_COUNT; i++) {
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                Game.Audio.Sounds.PlaySound(m_BlinkSound, 0.05f);

                if (m_Hitbox.enabled) {
                    m_SpriteRenderer.enabled = true;
                    yield break;
                }

                yield return new WaitForSeconds(BlinkTime);
            }

            // Wait one more blink just because it feels more correct.
            yield return new WaitForSeconds(PostBlinkTime);
            Reset();

        }

        public virtual void Reset() {
            Game.Visuals.Effects.PlayImpactEffect(m_RefreshParticle, 8, 0.6f, transform, Vector3.zero);
            Game.Audio.Sounds.PlaySound(m_RefreshSound, 0.15f);
            m_Hitbox.enabled = true;
            m_SpriteRenderer.enabled = true;
        }

                // Returns the time that this object takes per blink.
        protected virtual float GetBlinkTime() { return (RESET_DELAY - RESET_BLINK_DELAY) / (float)(2 * RESET_BLINK_COUNT); }

        // Returns the time in between blinking and resetting fully.
        protected virtual float GetPostBlinkTime() { return (RESET_DELAY - RESET_BLINK_DELAY) / (float)RESET_BLINK_COUNT; }

        // Returns the time before the object starts blinking back.
        protected virtual float GetPreBlinkTime() { return RESET_BLINK_DELAY; }

        #endregion

    }

}
