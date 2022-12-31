/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.SceneManagement;
// Platformer.
using Platformer.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class StartLevelButton : Button {

        // The game scene name.
        private const string GAME_SCENE_NAME = "Game";

        // Runs whenever this button is pressed.
        protected override void OnPress() {
            SceneManager.LoadScene(GAME_SCENE_NAME);
        }

    }

}