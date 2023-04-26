/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.SceneManagement;
// Platformer.
using Platformer.UI;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class RespawnButton : Button {

        // Runs whenever this button is pressed.
        protected override void OnPress() {
            Game.Instance.Play();
            Game.MainPlayer.Reset();
        }

    }

}