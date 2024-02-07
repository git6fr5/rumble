/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

namespace Platformer {

    ///<summary>
    /// Ties the physics functionality to the rest of the game.
    ///<summary>
    public class PhysicsManager : MonoBehaviour {

        #region Fields.

        // Controls the flow of time in the game.
        [SerializeField]
        private TimeController m_TimeController;
        public TimeController Time => m_TimeController;

        // Exposes simple collision functionality.
        [SerializeField]
        private CollisionCheck m_CollisionCheck = new CollisionCheck();
        public CollisionCheck Collisions => m_CollisionCheck;

        // Collision.
        [SerializeField]
        private CollisionLayers m_CollisionLayers = new CollisionLayers();
        public CollisionLayers CollisionLayers => m_CollisionLayers;

        #endregion

        #region Methods.

        public void OnGameLoad() {

        }

        #endregion

    }

}