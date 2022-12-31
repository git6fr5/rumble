/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using SaveSystem = Platformer.Management.SaveSystem;

namespace Platformer.Audio {

    ///<summary>
    /// The settings for using the audio in the game.
    ///<summary>
    public static class AudioSettings {

        #region Enumerations.

        public enum VolumeType {
            Master, Music, Ambience, Sound
        }

        #endregion

        [System.Serializable]
        public class AudioSettingsData {

            public float masterVolume;
            public bool masterMuted;
            public float musicVolume;
            public bool musicMuted;
            public float ambienceVolume;
            public bool ambienceMuted;
            public float soundVolume;
            public bool soundMuted;

            public AudioSettingsData() {
                masterVolume = AudioSettings.MasterVolumeValue;
                masterMuted = AudioSettings.MuteMaster;
                musicVolume = AudioSettings.MusicVolumeValue;
                musicMuted = AudioSettings.MuteMusic;
                ambienceVolume = AudioSettings.AmbienceVolumeValue;
                ambienceMuted = AudioSettings.MuteAmbience;
                soundVolume = AudioSettings.SoundVolumeValue;
                soundMuted = AudioSettings.MuteSound;
            }

            public void Read() {
                AudioSettings.MasterVolume = masterVolume;
                AudioSettings.MuteMaster = musicMuted;
                AudioSettings.MusicVolume = musicVolume;
                AudioSettings.MuteMusic = musicMuted;
                AudioSettings.AmbienceVolume = ambienceVolume;
                AudioSettings.MuteAmbience = ambienceMuted;
                AudioSettings.SoundVolume = soundVolume;
                AudioSettings.MuteSound = soundMuted;
            }
            
        }

        // The master volume for all audio sources.
        private static float m_MasterVolume = 0.5f;
        public static float MasterVolumeValue => m_MasterVolume;

        // Whether the game is on mute.
        private static bool m_MuteMaster = false;
        public static bool MuteMaster {
            get { return m_MuteMaster; }
            set { 
                m_MuteMaster = value; 
                SaveSystem.SaveAudioSettings();
            }
        }

        // The actual master volume of the game.
        public static float MasterVolume {
            get { return m_MuteMaster ? 0f : m_MasterVolume; }
            set { 
                m_MasterVolume = value;
                SaveSystem.SaveAudioSettings();
            }
        }

        // The volume of the music.
        private static float m_MusicVolume = 0.5f;
        public static float MusicVolumeValue => m_MusicVolume;

        // Whether the music is muted.
        private static bool m_MuteMusic = false;
        public static bool MuteMusic {
            get { return m_MuteMusic; }
            set { 
                m_MuteMusic = value; 
                SaveSystem.SaveAudioSettings();
            }
        }

        // The actual volume to play the music at.
        public static float MusicVolume {
            get { return m_MuteMusic ? 0f : MasterVolume * m_MusicVolume; }
            set { 
                m_MusicVolume = value; 
                SaveSystem.SaveAudioSettings();
            }
        }

        // The volume of the ambience.
        private static float m_AmbienceVolume = 0.5f;
        public static float AmbienceVolumeValue => m_AmbienceVolume;

        // Whether the ambience is on mute.
        private static bool m_MuteAmbience = false;
        public static bool MuteAmbience {
            get { return m_MuteAmbience; }
            set { 
                m_MuteAmbience = value; 
                SaveSystem.SaveAudioSettings();
            }
        }

        // The actual volume to play the music at.
        public static float AmbienceVolume {
            get { return m_MuteAmbience ? 0f : MasterVolume * m_AmbienceVolume; }
            set { 
                m_AmbienceVolume = value; 
                SaveSystem.SaveAudioSettings();
            }
        }

        // The volume of the sfx in the game.
        private static float m_SoundVolume = 1f;
        public static float SoundVolumeValue => m_SoundVolume;

        // Whether the sound is on mute.
        private static bool m_MuteSound = false;
        public static bool MuteSound {
            get { return m_MuteSound; }
            set { 
                m_MuteSound = value; 
                SaveSystem.SaveAudioSettings();
            }
        }

        // The actual volume to play the music at.
        public static float SoundVolume {
            get { return m_MuteSound ? 0f : MasterVolume * m_SoundVolume; }
            set { 
                m_SoundVolume = value; 
                SaveSystem.SaveAudioSettings();
            }
        } 

    }

}