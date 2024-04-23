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
            DecorationLayer[] _layers = transform.GetComponentsInChildren<DecorationLayer>();
            for (int i = 0; i < _layers.Length; i++) {
                m_Layers.Add(_layers[i]);
            }
        }

        public void ParentToSection(Transform transform) {
            
            SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                DecorationLayer l = m_Layers.Find(_l => _l.sortingLayer == spriteRenderer.sortingLayer);
                if (l != null) {
                    transform.SetParent(l);
                }
            }

        }

    }

}
