/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Orbs;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(CircleCollider2D))]
    public class SwitchOrb : OrbObject {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before the orb naturally respawns.
        [SerializeField]
        private float m_ResetDelay = 6f;

        [SerializeField, ReadOnly]
        private bool m_Collected = false;
        public bool Collected => m_Collected;

        // The amount of time before it starts blinking.
        private float ResetBlinkDelay => m_ResetDelay - 1.6f;

        private int ResetBlinkCount = 6;


        #endregion

        #region Methods.

        // Collects this orb.
        protected override void OnTouch(CharacterController character) {
            base.OnTouch(character);
            m_Collected = true;

            // Disable the orb for a bit.
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
            StartCoroutine(IESwitchReset());

        }

        // Reset after a delay.
        protected IEnumerator IESwitchReset() {
            yield return new WaitForSeconds(ResetBlinkDelay);

            if (m_Hitbox.enabled && m_SpriteRenderer.enabled) {
                yield return null;
            }

            // Set up the colors.
            Color cacheColor = m_SpriteRenderer.color;
            Color tempColor = m_SpriteRenderer.color;
            tempColor.a = RESET_BASE_OPACITY;
            
            // Blink the orb a couple of times.
            m_SpriteRenderer.color = tempColor; 
            for (int i = 0; i < 2 * ResetBlinkCount; i++) {
                tempColor.a += RESET_OPACITY_PER_BLINK;
                m_SpriteRenderer.color = tempColor; 
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                Game.Audio.Sounds.PlaySound(m_BlinkSound, 0.05f);
                yield return new WaitForSeconds((m_ResetDelay - ResetBlinkDelay) / (float)(2 * ResetBlinkCount));
            }
            
            // Reset the orbs color.
            m_SpriteRenderer.color = cacheColor;
            m_SpriteRenderer.enabled = true;

            // Wait one more blink just because it feels more correct.
            yield return new WaitForSeconds((m_ResetDelay - ResetBlinkDelay) / (float)ResetBlinkCount);

            // Make the orb collectible agains.
            Reset();

            yield return null;
        
        }
        
        // Resets the orb.
        public override void Reset() {
            m_Collected = false;
            base.Reset();
        }

        #endregion
        
    }
}