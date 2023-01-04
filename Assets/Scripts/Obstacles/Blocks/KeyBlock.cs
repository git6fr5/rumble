/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Blocks;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;
using KeyOrb = Platformer.Objects.Orbs.KeyOrb;

namespace Platformer.Objects.Blocks {

    ///<summary>
    /// 
    ///<summary>
    public class KeyBlock : BlockObject {

        [SerializeField]
        private bool m_Unlocked = false;

        protected override bool CheckActivationCondition() {
            return m_Unlocked;
        }

        // The functionality for when a block is touched.
        protected override void OnTouched(CharacterController character, bool touched) {
            base.OnTouched(character, touched);
            if (m_Unlocked) { return; }
            if (touched) {
                bool used = KeyOrb.UseFirstFollowing(character.transform);
                if (used) {
                    m_Unlocked = true;
                }
            }
        }

        // Runs once when something enters this area.
        protected virtual void OnCollisionEnter2D(Collision2D collision) {
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character != null) {
                m_Touched = true;
                OnTouched(character, true);
            }
        }

        // Runs once when something leaves this area.
        protected virtual void OnCollisionExit2D(Collision2D collision) {
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character != null) {
                m_Touched = false;
                OnTouched(character, false);
            }
        }

        protected override void OnActivation() {
            base.OnActivation();
            m_Hitbox.enabled = false;
        }

        protected override void OnDeactivation() {
            base.OnDeactivation();
            m_Hitbox.enabled = true;
        }

        // Resets the block.
        public override void Reset() {
            m_Hitbox.enabled = true;
            base.Reset();
        }

    }

}