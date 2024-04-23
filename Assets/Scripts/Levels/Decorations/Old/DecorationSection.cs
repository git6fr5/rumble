// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Levels {

    public class DecorationSection : MonoBehaviour {

        [SerializeField]
        private LevelSection m_LevelSection;
        public LevelSection levelSection => m_LevelSection;

        [SerializeField]
        private DecorationLayer[] m_Layers;
        public DecorationLayer[] layers => m_Layers;

        public static DecorationSection New(string name) {
            return new GameObject(name, typeof(DecorationSection)).GetComponent<DecorationSection>();
        }

        public void SetSection(LevelSection levelSection) {
            m_LevelSection = levelSection;
        }

        public void GetLayers() {
            m_Layers = transform.GetComponentsInChildren<DecorationLayer>();
        }

    }

}
