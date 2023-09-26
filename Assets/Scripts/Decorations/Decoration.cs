/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Decorations {

    public class Decoration : MonoBehaviour {

        [SerializeField]
        private List<Renderer> m_Renderers = new List<Renderer>();
        public List<Renderer> Renderers => m_Renderers;

    }

}