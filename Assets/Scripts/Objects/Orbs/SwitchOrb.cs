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
    public class SwitchOrb : OrbObject {

        #region Variables.

        /* --- Members --- */

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
            StartCoroutine(IEReset());

        }

        // Returns the time that this object takes per blink.
        protected override float GetBlinkTime() {
            return (m_ResetDelay - ResetBlinkDelay) / (float)(2 * ResetBlinkCount);
        }

        // Returns the time in between blinking and resetting fully.
        protected override float GetPostBlinkTime() {
            return (m_ResetDelay - ResetBlinkDelay) / (float)ResetBlinkCount;
        }

        // Returns the time before the object starts blinking back.
        protected override float GetPreBlinkTime() {
            return ResetBlinkDelay;
        }
        
        // Resets the orb.
        public override void Reset() {
            m_Collected = false;
            base.Reset();
        }

        #endregion
        
    }
}