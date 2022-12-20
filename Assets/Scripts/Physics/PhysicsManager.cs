/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Management {

    ///<summary>
    /// Ties the physics functionality to the rest of the game.
    ///<summary>
    public class PhysicsManager : MonoBehaviour {

        #region Fields.

        // Controls the flow of time in the game.
        [SerializeField]
        private TimeController m_TimeController;
        public TimeController Time => m_TimeController;

        // Controls the flow of time in the game.
        // [SerializeField]
        // private CollisionController m_CollisionController;
        // public CollisionController Collisions => m_CollisionController;

        // Collision.
        [SerializeField]
        private CollisionLayers m_CollisionLayers = new CollisionLayers();
        public CollisionLayers CollisionLayers => m_CollisionLayers;

        #endregion

    }

}