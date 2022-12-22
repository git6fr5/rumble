/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.Levels.LDtk;

namespace Platformer.Levels.LDtk {

    /// <summary>
    /// Stores a reference to the ldtk layers for easy reference.
    /// <summary>
    public class LDtkLayers {

        // Layer Names
        [SerializeField]
        private string m_Control = "Controls";
        public string Control => m_Control;

        [SerializeField]
        private string m_Entity = "Entities";
        public string Entity => m_Entity;

        [SerializeField]
        private string m_Water = "Water";
        public string Water => m_Water;

        [SerializeField]
        private string m_Ground = "Ground";
        public string Ground => m_Ground;
        
    }

}
    