/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Utilities;
using Platformer.Character;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    /// 
    ///<summary>
    public class ShadowBlock : BlockController {

        protected override bool CheckActivationCondition() {
            return Game.MainPlayer.Shadow.Enabled;
        }

        protected override void OnActivation() {
            m_Hitbox.enabled = true;
        }

        protected override void OnDeactivation() {
            m_Hitbox.enabled = false;
        }

        // Resets the block.
        public override void Reset() {
            base.Reset();
        }

    }

}