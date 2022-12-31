/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

/* --- Definitions --- */
using AudioSettings = Platformer.Audio.AudioSettings;
using AudioManager = Platformer.Management.AudioManager;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class AudioMenu : MonoBehaviour {

        #region Variables.

        // The audio manager in this scene.
        [SerializeField]
        public AudioManager m_Audio;

        // The master audio slider.
        [SerializeField]
        public Slider m_MasterVolumeSlider = null;

        // The master audio slider.
        [SerializeField]
        public Slider m_MusicVolumeSlider = null;

        // The master audio slider.
        [SerializeField]
        public Slider m_AmbienceVolumeSlider = null;

        // The master audio slider.
        [SerializeField]
        public Slider m_SoundsVolumeSlider = null;

        #endregion

        #region Methods.

        private void Start() {
            GetValuesFromSettings();
            Invoke("SetEventListeners", 0);
        }

        // Reads the save data to get the initial values.
        public void GetValuesFromSettings() {
            m_MasterVolumeSlider.value = AudioSettings.MasterVolumeValue;
            m_MusicVolumeSlider.value = AudioSettings.MusicVolumeValue;
            m_AmbienceVolumeSlider.value = AudioSettings.AmbienceVolumeValue;
            m_SoundsVolumeSlider.value = AudioSettings.SoundVolumeValue;
        }

        public void SetEventListeners() {
            m_MasterVolumeSlider.onValueChanged.AddListener( delegate { OnVolumeChanged(AudioSettings.VolumeType.Master); } );
            m_MusicVolumeSlider.onValueChanged.AddListener( delegate { OnVolumeChanged(AudioSettings.VolumeType.Music); } );
            m_AmbienceVolumeSlider.onValueChanged.AddListener( delegate { OnVolumeChanged(AudioSettings.VolumeType.Ambience); } );
            m_SoundsVolumeSlider.onValueChanged.AddListener( delegate { OnVolumeChanged(AudioSettings.VolumeType.Sound); } );
        }

        // Invoked when the value of the slider changes.
        public void OnVolumeChanged(AudioSettings.VolumeType volumeType) {

            switch (volumeType) {
                case AudioSettings.VolumeType.Master:
                    AudioSettings.MasterVolume = m_MasterVolumeSlider.value;
                    break;
                case AudioSettings.VolumeType.Music:
                    AudioSettings.MusicVolume = m_MusicVolumeSlider.value;
                    break;
                case AudioSettings.VolumeType.Ambience:
                    AudioSettings.AmbienceVolume = m_AmbienceVolumeSlider.value;
                    break;
                case AudioSettings.VolumeType.Sound:
                    AudioSettings.SoundVolume = m_SoundsVolumeSlider.value;
                    break;
                default:
                    break;
            }

            m_Audio.ResetVolumes();
        }

        #endregion

    }

}