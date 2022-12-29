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
    public class SoundController : MonoBehaviour {

        #region Fields.

        /* --- Constants --- */

        // The default volume of a sound effect if the incoming
        // sound effect's volume is not specified.
        public const float DEFAULT_VOLUME = 1f;

        // The threshold interval under which a sound is considered to have
        // just been played.
        public const float JUST_PLAYED_INTERVAL = 0.05f;

        // The number of audio sources to be generated initially.
        public const int INITAL_SOURCE_COUNT = 15;

        // The spread in pitch when playing a sound effect.
        public const float PITCH_SPREAD = 0.025f;

        /* --- Members --- */

        // The audio sources that play the sounds.
        [HideInInspector]
        private List<AudioSource> m_Sources = new List<AudioSource>();
        
        #endregion

        #region Methods

        // Runs once before the first frame.
        private void Start() {
            m_Sources = new List<AudioSource>();
            GenerateSources(INITAL_SOURCE_COUNT);
        }

        // Generates sources to play the sound effects through.
        private void GenerateSources(int count) {
            for (int i = 0; i < count; i++) {
                AudioSource audioSource = new GameObject("Sound AudioSource " + m_Sources.Count.ToString(), typeof(AudioSource)).GetComponent<AudioSource>();
                audioSource.transform.SetParent(Game.Audio.transform);
                audioSource.transform.localPosition = Vector3.zero;
                m_Sources.Add(audioSource);
            }
        }

        // Plays the given sound.
        public void PlaySound(AudioClip audioClip, float volume = DEFAULT_VOLUME) {
            if (audioClip == null || m_Sources == null) { return; }
            
            List<AudioSource> playing = GetPlaying(audioClip, JUST_PLAYED_INTERVAL);
            volume *= GetVolumeReduction(playing.Count) * AudioSettings.SoundVolume;
            
            AudioSource audioSource = GetFreeAudioSource();
            audioSource.clip = audioClip;
            audioSource.volume = Mathf.Sqrt(volume);
            audioSource.pitch = Random.Range(1f - PITCH_SPREAD, 1f + PITCH_SPREAD);
            audioSource.Play();
            
        }

        // Stops all sources playing the given sound.
        public void StopSound(AudioClip audioClip) {
            List<AudioSource> playing = GetPlaying(audioClip);
            for (int i = 0; i < playing.Count; i++) {
                playing[i].Stop();
            }
        }

        // Gets all the sources playing a given sound.
        // That are still under the given interval.
        public List<AudioSource> GetPlaying(AudioClip audioClip, float interval = -1f) {
            if (interval == -1f) { interval = Mathf.Infinity; }
            return m_Sources.FindAll(s => s.clip == audioClip && s.isPlaying && s.time < interval);
        }

        // Gets the volume reduction based on the number of sounds
        // of the same type already being played.
        public float GetVolumeReduction(int count) {
            if (count < 1) { return 1f; }
            return 1f / (1f + count);
        }

        // Gets a free audio source, generates a new one if necessary.
        public AudioSource GetFreeAudioSource() {
            for (int i = 0; i < m_Sources.Count; i++) {
                if (!m_Sources[i].isPlaying) {
                    return m_Sources[i];
                }
            }
            GenerateSources(1);
            return m_Sources[m_Sources.Count - 1];
        }

        #endregion

    }

}