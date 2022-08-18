// TODO: Clean

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Rendering;

namespace Platformer.Rendering {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class ColorPalette {

        [SerializeField] private Color[] m_Color;

        public void SetBlend(Material material, string tag = "A") {
            for (int i = 0; i < 6; i++) {
                material.SetColor("_Color" + tag + i.ToString(), m_Color[i]);
            }
        }

        public void SetSimple(Material material) {
            for (int i = 0; i < 6; i++) {
                material.SetColor("_Color" + i.ToString(), m_Color[i]);
            }
        }

    }

}