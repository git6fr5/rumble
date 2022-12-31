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

namespace Platformer.Objects.Blocks {

    ///<summary>
    /// 
    ///<summary>
    public class ShadowBlock : BlockObject {

        protected override bool CheckActivationCondition() {
            return Game.MainPlayer.Shadow.Enabled;
        }

        // The functionality for when a block is touched.
        protected override void OnTouched(CharacterController character, bool touched) {
            if (m_Active) {
                character.Shadow.TryStartShadowTravel(character, this);
            }
        }

        protected override void OnActivation() {
            base.OnActivation();
            m_Hitbox.isTrigger = true;
        }

        protected override void OnDeactivation() {
            base.OnDeactivation();
            m_Hitbox.isTrigger = false;
        }

        // Resets the block.
        public override void Reset() {
            m_Hitbox.isTrigger = false;
            base.Reset();
        }

    }

}