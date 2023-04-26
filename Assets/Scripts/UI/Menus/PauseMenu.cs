/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

/* --- Definitions --- */
using LevelSettings = Platformer.Levels.LevelSettings;
using Game = Platformer.Management.GameManager;


namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class PauseMenu : MonoBehaviour {

        // The title text of this level.
        [SerializeField]
        private Text m_TitleText = null;

        // The title text of this level.
        [SerializeField]
        private Text m_CurrentLevelStars = null;

        // The title text of this level.
        [SerializeField]
        private Text m_CurrentLevelTime = null;
        
        // The title text of this level.
        [SerializeField]
        private Text m_LevelStars = null;

        // The title text of this level.
        [SerializeField]
        private Text m_LevelTime = null;


        #region Methods.

        void OnEnable() {
            SetSelectedLevel();
            // Invoke("SetSelectedLevel", 0.1f);
            // Invoke("SetSelectedLevel", 0.2f);
            // Invoke("SetSelectedLevel", 0.4f);
        }

        public void SetSelectedLevel() {
            m_TitleText.text = LevelSettings.LevelName.ToUpper();

            m_CurrentLevelStars.text = "CURRENT STARS: " + GetCurrentPointsText() + "/" + LevelSettings.MaxPoints.ToString();
            m_CurrentLevelTime.text = "CURRENT TIME: " + GetCurrentTimeText();
            
            m_LevelStars.text = "BEST STARS: " + GetPointsText() + "/" + LevelSettings.MaxPoints.ToString();
            m_LevelTime.text = "BEST TIME: " + GetTimeText();
        }

        public string GetCurrentPointsText() {
            if (Game.Level.Points < 0) {
                return "0";
            }
            return Game.Level.Points.ToString();
        }

        public string GetCurrentTimeText() {
            string fullTime = Game.Physics.Time.Ticks.ToString();
            string splitTime = fullTime.Split('.')[0];
            return splitTime + " seconds";

        }
        
        public string GetPointsText() {
            if (LevelSettings.CompletedPoints < 0) {
                return "0";
            }
            return LevelSettings.CompletedPoints.ToString();
        }

        public string GetTimeText() {
            if (LevelSettings.CompletedTime < 0f) {
                return "Incomplete.";
            }

            string fullTime = LevelSettings.CompletedTime.ToString();
            string splitTime = fullTime.Split('.')[0];
            return splitTime + " seconds";

        }

        #endregion

    }

}