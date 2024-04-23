// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using Platformer.Levels;

namespace Platformer.Levels {

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
        public bool m_UpdateSections = false;
        public bool m_Reposition = false;

        [SerializeField]
        private DecorationLayer m_CurrentLayer = null;
        public static DecorationLayer CurrentLayer => Instance != null ? Instance.m_CurrentLayer : null;

        void Update() {
            if (!Application.isPlaying) {
                if (Instance == null) {
                    Instance = this;
                }
            }

            if (m_Reposition && !Application.isPlaying) {
                Reposition();
                m_Reposition = false;
            }

            if (m_Reorganize && !Application.isPlaying) {
                Reorganize();
                m_Reorganize = false;
            }

            // if (m_Reorganize && !Application.isPlaying) {
            //     m_Reorganize = false;
            // }

            if (m_UpdateSections) {
                UpdateDecorationSections();
            }
            
        }

        private void Reposition() {
            if (m_LevelManager == null) { return; }

            foreach (DecorationSection section in m_DecorationSection) {
                
                if (section.levelSection != null) {
                    section.transform.position = section.levelSection.GetComponent<BoxCollider2D>().offset;
                }

            } 

        }

        private void Reorganize() {

            List<Transform> unorganizedTransforms = new List<Transform>();

            foreach (DecorationSection section in m_DecorationSection) {
                foreach (DecorationLayer layer in section.layers) {
                    foreach (Transform child in layer.transform) {

                        if (section.levelSection != null) {

                            if (!section.levelSection.camHandles.Box.bounds.Contains(child.position)) {
                                unorganizedTransforms.Add(child);
                            }

                        }

                    }
                }
            }

            foreach (Transform child in unorganizedTransforms) {
                
                foreach (DecorationSection section in m_DecorationSection) {
                    if (section.levelSection != null) {
                        if (section.levelSection.camHandles.Box.bounds.Contains(child.position)) {
                            section.ParentToSection(child);
                            break;
                        }
                    }
                }

            }

        }

        private void UpdateDecorationSections() {
            foreach (DecorationSection section in m_DecorationSection) {
                if (section.levelSection == null) {
                    section.SetSection(FindLevelSection(section.gameObject.name));
                    section.GetLayers();
                }
            }
        }

        private LevelSection FindLevelSection(string name) {
            string sectionName = name.Split(" ")[0];
            print(sectionName);

            foreach (LevelSection section in m_LevelManager.Sections) {
                if (section.gameObject.name == sectionName) {
                    print("found section");
                    return section;
                }
            }

            return null;

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
