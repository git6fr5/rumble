/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer {

    ///<summary>
    /// Defines the general physics settings for the game.
    ///<summary>
    public static class PhysicsSettings {

        // Movement.
        private static float m_MovementPrecision = 1f/32f;
        public static float MovementPrecision => m_MovementPrecision;

        // Collision.
        private static float m_CollisionPrecision = 1f/48f;
        public static float CollisionPrecision => m_CollisionPrecision;

        // Gravity
        private static float m_GravityScale = 1f;
        public static float GravityScale => m_GravityScale;

    }
}