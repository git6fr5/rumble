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
    public class AmbienceController : MonoBehaviour {

        #region Fields.

        /* --- Members --- */

        // The music for this run.
        [SerializeField]
        private AudioClip m_Ambience;

        // The audio source that plays the music.
        [HideInInspector]
        private AudioSource m_Source;

        // Environemnt sounds.
        // [SerializeField] 
        // private AudioClip m_GroundImpactSound;
        // public AudioClip GroundImpactSound => m_GroundImpactSound;

        // [SerializeField] 
        // private AudioClip m_GroundStepSoundA;
        // public AudioClip GroundStepSoundA => m_GroundStepSoundA;
        
        // [SerializeField] 
        // private AudioClip m_GroundStepSoundB;
        // public AudioClip GroundStepSoundB => m_GroundStepSoundB;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        private void Start() {
            m_Source = GenerateSource();
            PlayAmbience();
        }

        // Generates the audio source to play the music from.
        private AudioSource GenerateSources(int count) {
            AudioSource audioSource = new GameObject("Ambience AudioSource", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.transform.SetParent(Game.Audio.transform);
            audioSource.transform.localPosition = Vector3.zero;
            return audioSource;
        }

        // Plays the music.
        public void PlayAmbience() {
            m_Source.clip = m_Ambience;
            m_Source.volume = AudioSettings.AmbienceVolume * volume;
            m_Source.loop = true;
            m_Source.pitch = 1f;
            m_Source.Play();
        }

        #endregion

    }
}