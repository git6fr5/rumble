/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

/* --- Definitions --- */
using LevelSettings = Platformer.Levels.LevelSettings;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class LevelMenu : MonoBehaviour {

        // The title text of this level.
        [SerializeField]
        private Text m_TitleText = null;

        // The title text of this level.
        [SerializeField]
        private Text m_LevelStars = null;

        // The title text of this level.
        [SerializeField]
        private Text m_LevelTime = null;


        #region Methods.

        public void SetSelectedLevel() {
            m_TitleText.text = LevelSettings.LevelName.ToUpper();
            m_LevelStars.text = "STARS: " + LevelSettings.CompletedPoints.ToString() + "/" + LevelSettings.MaxPoints.ToString();
            m_LevelTime.text = "BEST TIME: " + "00:00:00";
        }


        #endregion

    }

}