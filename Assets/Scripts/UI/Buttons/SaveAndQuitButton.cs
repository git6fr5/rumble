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
using SaveSystem = Platformer.Management.SaveSystem;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class SaveAndQuitButton : Button {

        // The game scene name.
        private const string MENU_SCENE_NAME = "Menu";

        // Runs whenever this button is pressed.
        protected override void OnPress() {
            Game.Level.OnSaveAndQuit();
            SaveSystem.SaveLevelSettings();
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }

    }

}