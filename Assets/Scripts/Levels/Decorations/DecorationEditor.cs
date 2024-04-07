// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using Platformer.Levels;

namespace Platformer.LevelEditing {

    [ExecuteInEditMode]
    public class DecorationEditor : MonoBehaviour {

        // The singleton.
        [SerializeField]
        private static DecorationEditor Instance = null;

        // The level manager.
        [SerializeField]
        private LevelManager m_LevelManager;

        [SerializeField]
        private List<DecorationSection> m_DecorationSection = new List<DecorationSection>();

        [Header("Controls")]
        public bool m_Reorganize = false;

        [SerializeField]
        private DecorationLayer m_CurrentLayer = null;
        public static DecorationLayer CurrentLayer => Instance != null ? Instance.m_CurrentLayer : null;

        void Update() {
            if (!Application.isPlaying) {
                if (Instance == null) {
                    Instance = this;
                }
            }

            if (m_Reorganize && !Application.isPlaying) {

                m_DecorationSection = new List<DecorationSection>();
                if (m_LevelManager != null) {
                    // Reorganize();
                }

                m_Reorganize = false;

            }
        }

        private void Reorganize() {

            Transform temp = new GameObject("Temp").transform;

            // foreach (DecorationSection section in m_DecorationSection) {
            //     foreach (DecorationLayer layer in section.Layers) {
            //         foreach (SpriteRenderer spriteRenderer in layer.SpriteRenderers) {
            //             spriteRenderer.transform.SetParent(temp);
            //         }
            //     }
            // }
            

            for (int i = 0; i < m_LevelManager.Sections.Count; i++) {
                DecorationSection section = DecorationSection.New(m_LevelManager.Sections[i].gameObject.name + " Decorations");
                m_DecorationSection.Add(section);
                section.transform.SetParent(transform);
            }

        }

    }

}
