/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using LDtkUnity;

namespace Platformer.Levels {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class LevelManager : Gobblefish.Manager<LevelManager, LevelSettings> {

        [SerializeField]
        private LevelDecorationCollection m_LevelDecorations;

        // A reference to all the created levels.
        [SerializeField] 
        public List<LevelSection> m_Sections = new List<LevelSection>();
        public List<LevelSection> Sections => m_Sections;

        // The current section.
        private LevelSection m_CurrentSection = null;
        public LevelSection CurrentSection => m_CurrentSection;

        public bool dont = false;

        protected override void Awake() {
            m_Settings = new LevelSettings();
            base.Awake();

            // Decoration decor = m_LevelDecorations.GetNew("Demo");
            

        }

        public void SetSections(List<LevelSection> sections) {
            for (int i = 0; i < m_Sections.Count; i++) {
                if (m_Sections[i] != null && m_Sections[i].gameObject != null) {
                    DestroyImmediate(m_Sections[i].gameObject);
                }
            }
            m_Sections = sections;
        }

        public static void AddDeath() {
            Settings.deaths += 1;
        }

        public static void AddPoint() {
            Settings.points += 1;
        }

        public static void SetCurrentSection(LevelSection levelSection) {
            Instance.m_CurrentSection = levelSection;
        }

    }

}
