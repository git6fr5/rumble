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
        private string m_Control = "CONTROLS";
        public string Control => m_Control;

        [SerializeField]
        private string m_Entity = "ENTITIES";
        public string Entity => m_Entity;

        [SerializeField]
        private string m_Decorations = "DECOR";
        public string Decorations => m_Decorations;

        [SerializeField]
        private string m_Ground = "GROUND";
        public string Ground => m_Ground;
        
    }

}
    