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
        private LayerMask Ground;
        public LayerMask Ground => m_Ground;

        [SerializeField] 
        private LayerMask m_Character;
        public LayerMask Character => m_Character;

        [SerializeField]
        private LayerMask m_Solid;
        public LayerMask Solid => m_Solid;

        #endregion

        #region Layer Names

        // Anything that needs to be ground goes on ground

        public static int DecorLayer => LayerMask.NameToLayer("Default");
        
        public static int OrbLayer => LayerMask.NameToLayer("Default");

        public static int SpikeLayer => LayerMask.NameToLayer("Default");
        
        public static int ProjectileLayer => LayerMask.NameToLayer("Default");

        public static int PlatformLayer => LayerMask.NameToLayer("Ground");
        
        public static int BlockLayer => LayerMask.NameToLayer("Ground");

        #endregion

    }

}