/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Audio;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Audio {

    ///<summary>
    ///
    ///<summary>
    public class MusicController : MonoBehaviour {

        #region Fields.

        /* --- Members --- */

        // The music for this run.
        [SerializeField]
        private AudioClip m_Music;

        // The audio source that plays the music.
        [HideInInspector]
        private AudioSource m_Source;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        private void Start() {
            m_Source = GenerateSource();
            PlayMusic();
        }

        // Generates the audio source to play the music from.
        private AudioSource GenerateSource() {
            AudioSource audioSource = new GameObject("Music AudioSource", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.transform.SetParent(Game.Audio.transform);
            audioSource.transform.localPosition = Vector3.zero;
            return audioSource;
        }

        // Plays the music.
        public void PlayMusic() {
            m_Source.clip = m_Music;
            m_Source.volume = AudioSettings.MusicVolume;
            m_Source.loop = true;
            m_Source.pitch = 1f;
            m_Source.Play();
        }

        #endregion

    }
}