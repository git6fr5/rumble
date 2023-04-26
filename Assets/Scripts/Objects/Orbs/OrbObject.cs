/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Objects;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    /// 
    ///<summary>
    public class OrbObject : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before the orb naturally respawns.
        private const float RESET_DELAY = 1.5f;
        // The amount of time before it starts blinking.
        private const float RESET_BLINK_DELAY = 0.7f;
        // The amount of times the orb blinks before it reappears.
        protected const float RESET_BLINK_COUNT = 3;
        // The opacity of the orb when it first blinks back into screen.
        protected const float RESET_BASE_OPACITY = 0.2f;
        // The opacity increase of the orb per blink.
        protected const float RESET_OPACITY_PER_BLINK = 0.075f;

        /* --- Components --- */

        // The sprite renderer displaying this gameObject.
        [SerializeField]
        protected SpriteRenderer m_SpriteRenderer;

        // The collider attached to this gameObject
        [SerializeField]
        protected CircleCollider2D m_Hitbox;
        public CircleCollider2D Hitbox => m_Hitbox;

        /* --- Parameters --- */

        // Used to cache the origin of what this orb is centered around.
        [SerializeField, ReadOnly] 
        protected Vector3 m_Origin;

        // The effect that plays when this orb is collected
        [SerializeField] 
        private VisualEffect m_TouchEffect;
        // The sound that plays when this orb is collected.
        [SerializeField] 
        private AudioClip m_TouchSound;
        // The effect that when this orb is reset.
        [SerializeField] 
        private VisualEffect m_RefreshEffect;
        // The sound that plays when this orb is reset.
        [SerializeField] 
        private AudioClip m_RefreshSound;
        // The sound that plays when this orb blinks.
        [SerializeField]
        protected AudioClip m_BlinkSound;

        // Gets the blink time for this object.
        private float BlinkTime => GetBlinkTime();
        // Gets the time in between blinking and resetting fully.
        private float PreBlinkTime => GetPreBlinkTime(); 
        // Gets the time in between blinking and resetting fully.
        private float PostBlinkTime => GetPostBlinkTime(); 

        #endregion

        // Runs once before the first frame.
        void Start() {
            // Set the hitbox to a trigger.
            m_Hitbox.isTrigger = true;
            // Set the origin of this object.
            m_Origin = transform.position;
            // Set the collision layer of an orb object. 
            gameObject.layer = Game.Physics.CollisionLayers.ORB_COLLISION_LAYER;
            // Set the rendering layer and order of an orb object.
            m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.ORB_RENDERING_LAYER;
            m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.ORB_RENDERING_ORDER;
            // Reset the orb to its default settings.
            Reset();
        }

        // Runs everytime something enters this trigger area.
        void OnTriggerEnter2D(Collider2D collider) {
            // Checks if a character has touched this trigger area and then runs the appropriate functionality.
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                OnTouch(character);
            }
        }

        protected virtual void OnTouch(CharacterController character) {
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sounds.PlaySound(m_TouchSound, 0.05f);
            Game.Visuals.Effects.PlayEffect(m_TouchEffect);
            // Game.Visuals.Effects.PlayEffect(character.ImpactEffect);
        }

        // A coroutine to eventually reset this orb object.
        protected IEnumerator IEReset() {
            // Wait a little until this orb starts blinking back into existence.
            yield return new WaitForSeconds(PreBlinkTime);
            // Break if the orb has been prematurely reset.
            if (m_Hitbox.enabled && m_SpriteRenderer.enabled) {
                yield break;
            }
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
                yield return new WaitForSeconds(BlinkTime);
            }
            // Reset the orbs color.
            m_SpriteRenderer.color = cacheColor;
            m_SpriteRenderer.enabled = true;
            // Wait one more blink just because it feels more correct.
            yield return new WaitForSeconds(PostBlinkTime);
            // Make the orb collectible agains.
            Reset();

        }

        // Resets the object to its default state.
        public virtual void Reset() {
            // Give the player feedback that this object has been reset.
            Game.Visuals.Effects.PlayEffect(m_RefreshEffect);
            Game.Audio.Sounds.PlaySound(m_RefreshSound, 0.15f);
            // Reset the hitbox and rendering components of this object.
            m_Hitbox.enabled = true;
            m_SpriteRenderer.enabled = true;

        }

        // Reset all the objects of the given type, that are currently active in the scene.
        public static void ResetAll() {
            OrbObject[] orbs = (OrbObject[])GameObject.FindObjectsOfType(typeof(OrbObject));
            for (int i = 0; i < orbs.Length; i++) {
                orbs[i].Reset();
            }
        }
        
        // Returns the time that this object takes per blink.
        protected virtual float GetBlinkTime() { return (RESET_DELAY - RESET_BLINK_DELAY) / (float)(2 * RESET_BLINK_COUNT); }

        // Returns the time in between blinking and resetting fully.
        protected virtual float GetPostBlinkTime() { return (RESET_DELAY - RESET_BLINK_DELAY) / (float)RESET_BLINK_COUNT; }

        // Returns the time before the object starts blinking back.
        protected virtual float GetPreBlinkTime() { return RESET_BLINK_DELAY; }

    }

}