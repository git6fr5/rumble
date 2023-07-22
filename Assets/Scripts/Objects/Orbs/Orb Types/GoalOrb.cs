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
    public class GoalOrb : OrbObject {

        protected override void OnTouch(CharacterController character) {
            base.OnTouch(character);
            ScoreOrb.CollectAllFollowing(character.transform);
        }

    }
}