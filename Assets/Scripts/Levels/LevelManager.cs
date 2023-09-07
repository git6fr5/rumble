/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.Levels;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using SaveSystem = Platformer.Management.SaveSystem;

namespace Platformer.Management {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class LevelManager: MonoBehaviour {
        
        /* --- Variables --- */
        #region Variables

        [HideInInspector]
        private int m_Deaths = 0;
        public int Deaths => m_Deaths;

        [HideInInspector]
        private int m_Points = 0;
        public int Points => m_Points;
        
        #endregion

        // Initializes the world.
        public void OnGameLoad() {
            
        }

        // Loads the map layouts for all the given levels.
        public void Preload() {

        }

        public void MoveToLoadPoint(string roomName, Transform playerTransform) {
            
        }

        // Resets the current room.
        public void Reset() {
            Platformer.Objects.Blocks.BlockObject.ResetAll();
            Platformer.Objects.Orbs.OrbObject.ResetAll();
        }

        // Loads the entities for 
        public void Load(Room room) {
            
        }

        public void Unload(Room room) {
            
        }

        /* --- end room manager --- */

        public void AddDeath() {
            m_Deaths += 1;
        }

        public void AddPoint() {
            m_Points += 1;
        }

        public void OnSaveAndQuit() {
        }

        public void OnComplete() {

        }

    }

}
    