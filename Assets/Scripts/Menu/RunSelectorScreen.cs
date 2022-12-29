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
using Platformer.Levels;
using Platformer.Levels.LDtk;
using Platformer.Levels.Tilemaps;
using Platformer.Levels.Entities;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class RunSelectorScreen : MonoBehaviour {

        [System.Serializable]
        public class RunData {

            public Tab Tab;
            public string LevelName;
            public int MaxStars;
            public int StarsCollected;

            public RunData(Tab tab, string levelName, int maxStars) {
                Tab = tab;
                LevelName = levelName;
                MaxStars = maxStars;
                StarsCollected = 0;
            }

        }

        #region Fields.

        [SerializeField] 
        private LDtkComponentProject[] m_LDtkData;

        [SerializeField]
        private LDtkLayers m_LDtkLayers = new LDtkLayers();

        [SerializeField]
        private List<RunData> m_RunData = new List<RunData>();

        [SerializeField]
        private Tab m_BaseTab;

        [SerializeField]
        private Menu m_Menu;

        // The text box where the title screen goes.
        [SerializeField]
        private Text m_TitleText = null;

        #endregion

        #region Methods.

        private void Start() {
            
            for (int i = 0; i < m_LDtkData.Length; i++) {

                Tab tab = CreateTab(i);
                string levelName = GetLevelName(i);
                int maxStars = GetStars(i);

                // tab.SetStars(maxStars);
                tab.SetText(levelName);
                
                RunData runData = new RunData(tab, levelName, maxStars);
                m_RunData.Add(runData);

            }

            m_BaseTab.transform.SetParent(null);
            Destroy(m_BaseTab.gameObject);

        }

        void Update() {
            
            for (int i = 0; i < m_RunData.Count; i++) {
                if (m_RunData[i].Tab == m_Menu.SelectedTab) {
                    SetTitle(m_RunData[i].LevelName);
                    // SetSymbol(m_RunData[i].StarsCollected, m_RunData[i].MaxStars);
                }
            }

        }

        private Tab CreateTab(int i) {
            Tab tab = Instantiate(m_BaseTab.gameObject, m_Menu.transform).GetComponent<Tab>();

            RectTransform rt = m_BaseTab.GetComponent<RectTransform>();
            RectTransform newRt = tab.GetComponent<RectTransform>();
            newRt.anchoredPosition = rt.anchoredPosition + Vector2.down * i * rt.sizeDelta.y;
            tab.gameObject.SetActive(true);

            return tab;
        }

        private string GetLevelName(int i) {
            LdtkJson json = m_LDtkData[i].FromJson();
            string levelName = json.LevelNamePattern;
            levelName = levelName.Replace('_', ' ');
            levelName = levelName.Split('%')[0];
            return levelName;
        }

        private int GetStars(int i) {
            LdtkJson json = m_LDtkData[i].FromJson();
            int total = 0;

            for (int j = 0; j < json.Levels.Length; j++) {
                List<LDtkTileData> entityData = LDtkReader.GetLayerData(json.Levels[j], m_LDtkLayers.Entity);
                total += entityData.FindAll(entity => entity.vectorID == LDtkTileData.ScoreOrbID).Count;
            }
            
            return total;
        }

        public void SetTitle(string text) {
            m_TitleText.text = text.ToUpper();
        }

        #endregion

    }

}