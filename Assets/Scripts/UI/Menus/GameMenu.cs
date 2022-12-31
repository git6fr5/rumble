/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.UI;

/* --- Definitions --- */
using LevelSettings = Platformer.Levels.LevelSettings;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class GameMenu : Menu {

        #region Fields.

        // The list of ldtk levels.
        [SerializeField] 
        private LDtkComponentProject[] m_LDtkData;

        //
        [HideInInspector]
        private Dictionary<Tab, LDtkComponentProject> m_TabLDtkDictionary = new Dictionary<Tab, LDtkComponentProject>();

        // The base tab in this menu screen.
        [SerializeField]
        private Tab m_BaseTab;

        // The level menu that this feeds data into.
        [SerializeField]
        private LevelMenu m_LevelMenu;

        #endregion

        #region Methods.

        // Runs once when instantiated.
        // I don't like using this.
        private void Awake() {
            CreateLevelTabs();
            m_BaseTab.transform.SetParent(null);
            Destroy(m_BaseTab.gameObject);
        }

        // Sets the selected tab
        public override void SetSelectedTab(Tab tab) {
            base.SetSelectedTab(tab);
            LevelSettings.CurrentLevelData = m_TabLDtkDictionary[tab];
            m_LevelMenu.SetSelectedLevel();
        }

        // Creates the tabs for the different runs.
        private void CreateLevelTabs() {
            m_TabLDtkDictionary = new Dictionary<Tab, LDtkComponentProject>();
            for (int i = 0; i < m_LDtkData.Length; i++) {
                // Parse the data.
                string levelName = LevelSettings.GetLevelName(m_LDtkData[i]);
                int maxStars = LevelSettings.GetPointsAvailable(m_LDtkData[i]);
                // Read the data into the structures.
                Tab tab = CreateLevelTab(i, levelName, maxStars);
                m_TabLDtkDictionary.Add(tab, m_LDtkData[i]);
            }
            
        }

        // Creates a single tab at the given index.
        private Tab CreateLevelTab(int i, string levelName, int maxStars) {
            Tab tab = Instantiate(m_BaseTab.gameObject, transform).GetComponent<Tab>();

            RectTransform rt = m_BaseTab.GetComponent<RectTransform>();
            RectTransform newRt = tab.GetComponent<RectTransform>();
            newRt.anchoredPosition = rt.anchoredPosition + Vector2.down * i * rt.sizeDelta.y;
            tab.gameObject.SetActive(true);

            tab.SetText(levelName);
            // tab.SetStars(maxStars);

            return tab;
        }

        #endregion

    }

}