/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Visuals {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class ColorPalette {

        // The colors in this palette.
        [SerializeField] 
        private Color[] m_Color;

        // Blends the material between an A and a B color palette.
        public void SetBlend(Material material, string tag = "A") {
            for (int i = 0; i < 6; i++) {
                material.SetColor("_Color" + tag + i.ToString(), m_Color[i]);
            }
        }

        // Sets the materials color.
        public void SetSimple(Material material) {
            for (int i = 0; i < 6; i++) {
                material.SetColor("_Color" + i.ToString(), m_Color[i]);
            }
        }

    }

}