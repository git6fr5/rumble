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

        protected override void OnActivation() {
            base.OnActivation();
            m_Hitbox.enabled = true;
        }

        protected override void OnDeactivation() {
            base.OnDeactivation();
            m_Hitbox.enabled = false;
        }

        // Resets the block.
        public override void Reset() {
            base.Reset();
        }

    }

}