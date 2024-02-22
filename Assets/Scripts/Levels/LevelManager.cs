/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using LDtkUnity;

/* --- Definitions --- */
using Game = Platformer.GameManager;
// using SaveSystem = Platformer.Management.SaveSystem;

namespace Platformer.Levels {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    [ExecuteInEditMode]
    public class LevelManager: MonoBehaviour {

        // Handles all the tilemap functionality.
        [SerializeField] 
        private TilemapController m_TilemapController;
        public TilemapController Maps => m_TilemapController;

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LDtkEntityManager m_LDtkEntityManager;

        // The JSON data corresponding to the given ldtk data.
        [SerializeField]
        private LDtkLayers m_LDtkLayers = new LDtkLayers();
        public LDtkLayers LDtkLayers => m_LDtkLayers;

        // A reference to all the created levels.
        [SerializeField] 
        public List<LevelSection> m_Sections = new List<LevelSection>();

        // The given LDtk file.
        [SerializeField] 
        private LDtkComponentProject m_LDtkData;

        // The JSON data corresponding to the given ldtk data.
        [HideInInspector]
        private LdtkJson m_JSON;

        [Header("Controls")]
        public bool m_Reload;

        void OnEnable() {
            m_Reload = false;
            if (!Application.isPlaying) {
                OnReload();
            }
        }

        void Update() {
            if (!Application.isPlaying) {

                if (m_Reload) {
                    OnReload();
                    m_Reload = false;
                }

            }
        }

        public void OnReload() {
            m_JSON = m_LDtkData.FromJson();
            CollectLevelSections(ref m_Sections, m_JSON, transform);
            m_TilemapController.RefreshLevel(m_Sections, m_LDtkLayers.Ground);    

            for (int i = 0; i < m_Sections.Count; i++) {
                m_Sections[i].DestroyEntities();
                m_Sections[i].GenerateEntities(m_LDtkEntityManager, m_LDtkLayers);
            }
        }

        // Collects all the levels from the LDtk file.
        private static List<LevelSection> CollectLevelSections(ref List<LevelSection> sections, LdtkJson json, Transform transform) {
            foreach (LevelSection section in sections) {
                if (section != null && section.gameObject != null) { DestroyImmediate(section.gameObject); }
            }

            sections = new List<LevelSection>();
            for (int i = 0; i < json.Levels.Length; i++) {
                LevelSection section = new GameObject(json.Levels[i].Identifier, typeof(LevelSection)).GetComponent<LevelSection>();
                section.transform.SetParent(transform);
                section.Preload(i, json);
                sections.Add(section);
            }
            return sections;
        }

        // Loads the entities for
        public void Load(LevelSection section) {
            // for (int i = 0; i < section.Pieces.Length; i++) {
            //     section.Pieces[i].SetActive(true);
            // }
        }

        public void Unload(LevelSection section) {
            // for (int i = 0; i < section.Pieces.Length; i++) {
            //     section.Pieces[i].SetActive(false);
            // }
            // Platformer.Objects.Spitters.Projectile.DeleteAll(); // Should go somewhere saying custom.
        }

    }

}
