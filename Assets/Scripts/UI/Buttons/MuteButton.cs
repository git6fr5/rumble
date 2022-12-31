/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// Platformer.
using Platformer.UI;

/* --- Definitions --- */
using AudioSettings = Platformer.Audio.AudioSettings;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Image))]
    public class MuteButton : Button {

        #region Variables.

        /* --- Component --- */
        
        // The image attached to this component.
        private Image m_Image => GetComponent<Image>();

        // The audio menu somewhere up this transform.
        private AudioMenu m_AudioMenu = null;

        /* --- Members --- */

        // The type of volume this might mute.
        [SerializeField] 
        private AudioSettings.VolumeType m_VolumeType = AudioSettings.VolumeType.Master;  

        #endregion

        #region Methods.

        // Runs once when instantiated.
        void Awake() {
            FindAudioMenu(transform.parent);
            RenderButton();
        }

        // Finds the audio menu somewhere up this transform.
        private void FindAudioMenu(Transform transform) {
            m_AudioMenu = transform.parent.GetComponent<AudioMenu>();
            if (m_AudioMenu == null && transform.parent != null) {
                FindAudioMenu(transform.parent);
            }
        }

        // Runs whenever this button is pressed.
        protected override void OnPress() {
            
            switch (m_VolumeType) {
                case AudioSettings.VolumeType.Master:
                    AudioSettings.MuteMaster = !AudioSettings.MuteMaster;
                    break;
                case AudioSettings.VolumeType.Music:
                    AudioSettings.MuteMusic = !AudioSettings.MuteMusic;
                    break;
                case AudioSettings.VolumeType.Ambience:
                    AudioSettings.MuteAmbience = !AudioSettings.MuteAmbience;
                    break;
                case AudioSettings.VolumeType.Sound:
                    AudioSettings.MuteSound = !AudioSettings.MuteSound;
                    break;
                default:
                    break;
            }

            RenderButton();
            m_AudioMenu.OnVolumeChanged(m_VolumeType);
        }

        void RenderButton() {

            bool muted = false;
            switch (m_VolumeType) {
                case AudioSettings.VolumeType.Master:
                    muted = AudioSettings.MuteMaster;
                    break;
                case AudioSettings.VolumeType.Music:
                    muted = AudioSettings.MuteMusic;
                    break;
                case AudioSettings.VolumeType.Ambience:
                    muted = AudioSettings.MuteAmbience;
                    break;
                case AudioSettings.VolumeType.Sound:
                    muted = AudioSettings.MuteSound;
                    break;
                default:
                    break;
            }

            m_Image.color = muted ? Color.white / 2f : Color.white;

        }

        #endregion
    }

}