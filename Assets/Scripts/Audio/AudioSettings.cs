/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Audio {

    ///<summary>
    /// The settings for using the audio in the game.
    ///<summary>
    public static class AudioSettings {

        // The master volume for all audio sources.
        private static float m_MasterVolume = 0.5f;

        // Whether the game is on mute.
        private static bool m_MuteMaster = false;

        // The actual master volume of the game.
        public static float MasterVolume => m_MuteMaster ? 0f : m_MasterVolume;

        // The volume of the music.
        private static float m_MusicVolume = 0.5f;

        // Whether the music is muted.
        private static bool m_MuteMusic = false;

        // The actual volume to play the music at.
        public static float MusicVolume => m_MuteMusic ? 0f : MasterVolume * m_MusicVolume;

        // The volume of the ambience.
        private static float m_AmbienceVolume = 0.5f;

        // Whether the ambience is on mute.
        private static bool m_MuteAmbience = false;

        // The actual volume to play the music at.
        public static float AmbienceVolume => m_MuteAmbience ? 0f : MasterVolume * m_AmbienceVolume;

        // The volume of the sfx in the game.
        private static float m_SoundVolume = 1f;

        // Whether the sound is on mute.
        private static bool m_MuteSound = false;

        // The actual volume to play the music at.
        public static float SoundVolume => m_MuteSound ? 0f : MasterVolume * m_SoundVolume;

    }

}