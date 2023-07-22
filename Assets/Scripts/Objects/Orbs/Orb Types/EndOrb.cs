/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
using UnityEngine.SceneManagement;
// Platformer.
using Platformer.Objects.Orbs;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using SaveSystem = Platformer.Management.SaveSystem;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    /// An orb that when touched ends the level.
    ///<summary>
    public class EndOrb : OrbObject {

        // The game scene name.
        private const string MENU_SCENE_NAME = "Menu";

        #region Methods.

        // The functionality for when a block is touched.
        protected override void OnTouch(CharacterController character) {
            base.OnTouch(character);
            EndLevel();
        }

        public void EndLevel() {
            Game.Level.OnComplete();
            SaveSystem.SaveLevelSettings();
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }

        #endregion

    }

}