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
    [System.Serializable]
    public class LDtkLayers {

        // Layer Names
        [SerializeField]
        private string m_Control = "CONTROLS";
        public string Control => m_Control;

        [SerializeField]
        private string m_Entity = "ENTITIES";
        public string Entity => m_Entity;

        [SerializeField]
        private string[] m_Decorations;
        public string[] Decorations => m_Decorations;

        private string m_Ground0 = "TILES_0";
        public string Ground0 => m_Ground0;
        private string m_Ground1 = "TILES_1";
        public string Ground1 => m_Ground1;
        private string m_Ground2 = "TILES_2";
        public string Ground2 => m_Ground2;
        
    }

}
    