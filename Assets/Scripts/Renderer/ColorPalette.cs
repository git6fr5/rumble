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

        public void Set(Material material, int first = 1) {

            string tag = first == 1 ? "A" : "B";
            for (int i = 0; i < 6; i++) {
                material.SetColor("_Color" + tag + i.ToString(), m_Color[i]);
            }

        }

    }

}