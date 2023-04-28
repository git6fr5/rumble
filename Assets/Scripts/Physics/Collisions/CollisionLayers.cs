/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// Stores a set of collision layers for easy reference.
    ///<summary>
    [System.Serializable]
    public class CollisionLayers {

        #region Layer Masks

        [SerializeField] 
        private LayerMask m_Default;
        public LayerMask Default => m_Default;

        // Collision Layers
        [SerializeField] 
        private LayerMask m_Ground;
        public LayerMask Ground => m_Ground;

        [SerializeField] 
        private LayerMask m_Character;
        public LayerMask Characters => m_Character;

        [SerializeField]
        private LayerMask m_Solid; // For characters + ground
        public LayerMask Solid => m_Solid;

        [SerializeField]
        private LayerMask m_IgnoreCharacter; // For anything that can touch the ground but not the character.
        public LayerMask IgnoreCharacter => m_IgnoreCharacter;

        #endregion

        #region Layer Names

        // Anything that needs to be ground goes on ground

        public int DecorLayer => LayerMask.NameToLayer("Default");
        
        public int OrbLayer => LayerMask.NameToLayer("Default");

        public int SpikeLayer => LayerMask.NameToLayer("Default");
        
        public int ProjectileLayer => LayerMask.NameToLayer("Default");

        public int PlatformLayer => LayerMask.NameToLayer("Ground");
        
        public int BlockLayer => LayerMask.NameToLayer("Ground");

        #endregion

    }

}