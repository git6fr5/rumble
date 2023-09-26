/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class Hazard : MonoBehaviour {

        #region Methods.

        // Collects this orb.
        public void Kill() {
            Debug.Log("Player killed");
            CharacterController character = Game.MainPlayer;
            character.Reset();
        }
        
        #endregion
        
    }
}