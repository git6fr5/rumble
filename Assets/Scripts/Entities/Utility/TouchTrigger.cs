// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// using UnityExtensions;
// // Platformer.
// using Platformer.Entities;

// /* --- Definitions --- */
// using Game = Platformer.Management.GameManager;
// using CharacterController = Platformer.Character.CharacterController;

// namespace Platformer.Entities.Utility {

//     ///<summary>
//     /// 
//     ///<summary>
//     [RequireComponent(typeof(Entity))]
//     public class TouchTrigger : MonoBehaviour {

//         #region Variables.

//         /* --- Constants --- */

//         // The amount of time before the orb naturally respawns.
//         private const float RESET_DELAY = 1.5f;

//         // The amount of time before it starts blinking.
//         private const float RESET_BLINK_DELAY = 0.7f;
        
//         // The amount of times the orb blinks before it reappears.
//         protected const float RESET_BLINK_COUNT = 3;

//         [SerializeField]
//         protected OrbAnimator m_Animator;

//         [SerializeField]
//         private VisualEffect m_BurstParticle;

//         [SerializeField]
//         private VisualEffect m_EmissionParticle;

//         /* --- Parameters --- */

//         // Used to cache the origin of what this orb is centered around.
//         [SerializeField, ReadOnly] 
//         protected Vector3 m_Origin;

//         // The sound that plays when this orb is collected.
//         [SerializeField] 
//         private AudioClip m_TouchSound;

//         // The sound that plays when this orb is reset.
//         [SerializeField] 
//         private AudioClip m_RefreshSound;

//         // The sound that plays when this orb blinks.
//         [SerializeField]
//         protected AudioClip m_BlinkSound;

//         // Gets the blink time for this object.
//         private float BlinkTime => GetBlinkTime();
//         // Gets the time in between blinking and resetting fully.
//         private float PreBlinkTime => GetPreBlinkTime(); 
//         // Gets the time in between blinking and resetting fully.
//         private float PostBlinkTime => GetPostBlinkTime(); 

//         #endregion

//         // Runs once before the first frame.
//         void Awake() {
//             // Cache the origin
//             m_Origin = transform.position;

//             // Collision settings.
//             m_Entity.SetAsTrigger(true);
//             gameObject.layer = Game.Physics.CollisionLayers.OrbLayer;

//             // Rendering settings.
//             if (m_Animator != null) {
//                 m_Animator.Initialize(this);
//             }
//             // Temporarily
//             else {
//                 m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.OrbLayer;
//                 m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.OrbOrder;
//             }

//             m_EmissionParticle.Play();

//         }

//         // Runs everytime something enters this trigger area.
//         void OnTriggerEnter2D(Collider2D collider) {
//             CharacterController character = collider.GetComponent<CharacterController>();
//             if (character != null) {
//                 OnTouch(character);
//             }
//         }

//         protected virtual void OnTouch(CharacterController character) {
//             m_BurstParticle.Play();
//             Game.Physics.Time.RunHitStop(8);
//             Game.Audio.Sounds.PlaySound(m_TouchSound, 0.05f);
//             m_EmissionParticle.Stop();
//         }

//         // A coroutine to eventually reset this orb object.
//         protected IEnumerator IEReset() {
//             // Wait a little until this orb starts blinking back into existence.
//             yield return new WaitForSeconds(PreBlinkTime);
//             // Break if the orb has been prematurely reset.
//             if (m_Hitbox.enabled) {
//                 m_SpriteRenderer.enabled = true;
//                 yield break;
//             }

//             // Blink the orb a couple of times.
//             for (int i = 0; i < 2 * RESET_BLINK_COUNT; i++) {
//                 m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
//                 Game.Audio.Sounds.PlaySound(m_BlinkSound, 0.05f);

//                 if (m_Hitbox.enabled) {
//                     m_SpriteRenderer.enabled = true;
//                     yield break;
//                 }

//                 yield return new WaitForSeconds(BlinkTime);
//             }

//             // Wait one more blink just because it feels more correct.
//             yield return new WaitForSeconds(PostBlinkTime);
//             Reset();

//         }

//         // Resets the object to its default state.
//         public virtual void Reset() {
//             // Give the player feedback that this object has been reset.
//             Game.Audio.Sounds.PlaySound(m_RefreshSound, 0.15f);
//             m_EmissionParticle.Play();

//             // Reset the hitbox and rendering components of this object.
//             m_Hitbox.enabled = true;
//             m_SpriteRenderer.enabled = true;
//         }

//         // Reset all the objects of the given type, that are currently active in the scene.
//         public static void ResetAll() {
//             OrbObject[] orbs = (OrbObject[])GameObject.FindObjectsOfType(typeof(OrbObject));
//             for (int i = 0; i < orbs.Length; i++) {
//                 orbs[i].Reset();
//             }
//         }
        
//         // Returns the time that this object takes per blink.
//         protected virtual float GetBlinkTime() { return (RESET_DELAY - RESET_BLINK_DELAY) / (float)(2 * RESET_BLINK_COUNT); }

//         // Returns the time in between blinking and resetting fully.
//         protected virtual float GetPostBlinkTime() { return (RESET_DELAY - RESET_BLINK_DELAY) / (float)RESET_BLINK_COUNT; }

//         // Returns the time before the object starts blinking back.
//         protected virtual float GetPreBlinkTime() { return RESET_BLINK_DELAY; }

//     }

// }