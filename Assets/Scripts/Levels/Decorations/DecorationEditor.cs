// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Levels {

    [ExecuteInEditMode]
    public class DecorationEditor : MonoBehaviour {

        // The editor.
        // [SerializeField]
        // private DecorationLayer m_CurrentLayer = null;
        // public static DecorationLayer CurrentLayer => INSTANCE != null ? INSTANCE.m_CurrentLayer : null;

        // The level manager.
        [SerializeField]
        private LevelManager m_LevelManager;

        [SerializeField]
        private List<DecorationSection> m_DecorationSection = new List<DecorationSection>();

        [Header("Controls")]
        public bool m_Reorganize = false;

        void Update() {
            if (m_Reorganize && !Application.isPlaying) {

                m_DecorationSection = new List<DecorationSection>();
                if (m_LevelManager != null) {
                    Reorganize();
                }

                m_Reorganize = false;

            }
        }

        private void Reorganize() {

            // Transform temp = new GameObject("Temp").transform;

            // foreach (DecorationSection section in m_DecorationSection) {
            //     foreach (DecorationLayer layer in section.Layers) {
            //         foreach (SpriteRenderer spriteRenderer in layer.SpriteRenderers) {
            //             spriteRenderer.transform.SetParent(temp);
            //         }
            //     }
            // }
            

            // for (int i = 0; i < m_LevelManager.Sections.Count; i++) {
            //     DecorationSection section = DecorationSection.New(m_LevelManager.Sections[i].gameObject.name + " Decorations");
            //     m_DecorationSection.Add(section);
            // }

        }

    }

}
