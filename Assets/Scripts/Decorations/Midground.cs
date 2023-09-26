/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Decorations;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Decorations {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class Midground : MonoBehaviour {

        [SerializeField]
        private Color m_LayerColor;

        [SerializeField]
        private string m_LayerName;

        public List<Decoration> m_Decorations = new List<Decoration>();  

        public bool updating = false;

        void Update() {

            if (!Application.isPlaying && updating) {
                print("updating");
            
                m_Decorations = new List<Decoration>();
                Decoration decor = null;
                foreach (Transform child in transform) {
                    decor = child.GetComponent<Decoration>();
                    if (decor != null) {
                        m_Decorations.Add(decor);
                    }
                }

                for (int i = 0; i < m_Decorations.Count; i++) {
                    for (int j = 0; j < m_Decorations[i].Renderers.Count; j++) {
                        m_Decorations[i].Renderers[j].sortingLayerName = m_LayerName;                        
                    }
                }

            }
        }

    }

}