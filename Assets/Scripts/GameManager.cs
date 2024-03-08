// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class GameManager : Gobblefish.GameManager {

        // This singleton.
        public static GameManager PLATFORMER_INSTANCE;

        [SerializeField]
        private Platformer.Character.CharacterController m_Player;
        public static Platformer.Character.CharacterController MainPlayer => PLATFORMER_INSTANCE.m_Player;

        // Exposes functionality for the levels in the game.
        [SerializeField] 
        private Platformer.Levels.LevelManager m_LevelManager;
        public static Platformer.Levels.LevelManager Level => PLATFORMER_INSTANCE.m_LevelManager;

        // Exposes functionality for the physics in the game.
        [SerializeField] 
        private Platformer.Physics.PhysicsManager m_PhysicsManager;
        public static Platformer.Physics.PhysicsManager Physics => PLATFORMER_INSTANCE.m_PhysicsManager;

        [SerializeField]
        private bool m_Playing = true;
        public static bool Playing => PLATFORMER_INSTANCE.m_Playing;

        protected override void Awake() {
            PLATFORMER_INSTANCE = this;
            base.Awake();
        }

        // Pause the game.
        public void Pause() {
            // m_LevelManager.OnSaveAndQuit();
            PLATFORMER_INSTANCE.m_PhysicsManager.Time.Pause();
        }

    }
    
}

