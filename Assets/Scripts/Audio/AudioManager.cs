/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Audio;

namespace Platformer.Management {

    ///<summary>
    /// Ties the audio functionality to the rest of the game.
    ///<summary>
    public class AudioManager : MonoBehaviour {

        #region Fields.

        // The music being played in the game.
        [SerializeField]
        private MusicController m_MusicController;
        public MusicController Music => m_MusicController;
        
        // The ambience being played in the game.
        [SerializeField]
        private AmbienceController m_AmbienceController;
        public AmbienceController Ambience => m_AmbienceController;

        // The sounds being played in the game.
        [SerializeField]
        private SoundController m_SoundController;
        public SoundController Sounds => m_SoundController;

        #endregion

        #region Methods.

        public void OnGameLoad() {

        }

        #endregion

    }

}

