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

        // Collision Layers
        [SerializeField] 
        private LayerMask m_Ground;
        public LayerMask Ground => m_Ground;

        [SerializeField] 
        private LayerMask m_Water;
        public LayerMask Water => m_Water;

        [SerializeField] 
        private LayerMask m_Platform;
        public LayerMask Platform => m_Platform;

        [SerializeField] 
        private LayerMask m_Characters;
        public LayerMask Characters => m_Characters;

        [SerializeField] 
        private LayerMask m_Orb;
        public LayerMask Orb => m_Orb;

        [SerializeField]
        private LayerMask m_Opaque;
        public LayerMask Opaque => m_Opaque;

        #endregion

        #region Layer Names

        public string DANGEROUS = "Dangerous";
        
        public string STATIC = "Ignore Color Swap";

        public string PSEUDOGROUND = "Pseudoground";

        public string CHARACTER = "Character";

        // ---

        public string PROJECTILE_COLLISION_LAYER => DANGEROUS;
        
        public string SPIKE_COLLISION_LAYER => DANGEROUS;
        
        public int ORB_COLLISION_LAYER => LayerMask.NameToLayer(STATIC);
        
        public string PLATFORM_COLLISION_LAYER => PSEUDOGROUND;
        
        public int BLOCK_COLLISION_LAYER => LayerMask.NameToLayer(PSEUDOGROUND);
        
        public string CHARACTER_COLLISION_LAYER => CHARACTER;

        #endregion

    }

}