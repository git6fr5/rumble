/* --- Libraries --- */
// System.
using System.IO;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

/* --- Defintions --- */
using AudioSettingsData = Platformer.Audio.AudioSettings.AudioSettingsData;
// using LevelSettingsData = Platformer.Levels.LevelSettings.LevelSettingsData;

//
namespace Platformer.Management {
    
    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class SaveSystem : MonoBehaviour {
        
        public const string AUDIO_SETTINGS_PATH = "/audio_settings.json";

        public const string LEVEL_FILE_PATH = "/levels/";

        // Runs once before instantiation.
        void Awake() {
            ReadAudioSettings();
        }

        #region Audio Settings

        public static void SaveAudioSettings() {
            string audioSettingsJson = JsonUtility.ToJson(new AudioSettingsData());
            System.IO.File.WriteAllText(Application.persistentDataPath + AUDIO_SETTINGS_PATH, audioSettingsJson);
        }

        public static void ReadAudioSettings() {
            string audioSettingsJson = System.IO.File.ReadAllLines(Application.persistentDataPath + AUDIO_SETTINGS_PATH)[0];
            AudioSettingsData audioSettingsData = JsonUtility.FromJson<AudioSettingsData>(audioSettingsJson);
            audioSettingsData.Read();
        }

        #endregion

        #region Level Settings

        // public static void CheckForLevelSettings() {
        //     if (!Directory.Exists(Application.persistentDataPath + LEVEL_FILE_PATH)) {
        //         Directory.CreateDirectory(Application.persistentDataPath + LEVEL_FILE_PATH);
        //     }
        //     LevelSettingsData levelSettingsData = new LevelSettingsData();
        //     string levelPath = LEVEL_FILE_PATH + levelSettingsData.fileName + ".json";
        //     if (!File.Exists(Application.persistentDataPath + levelPath)) {
        //         CreateNewLevelSettingsFile();    
        //     }
        //     ReadLevelSettings(levelSettingsData.fileName);
        // }

        // public static void ReadLevelSettings(string fileName) {
        //     string levelPath = LEVEL_FILE_PATH + fileName + ".json";
        //     string levelSettingsJson = System.IO.File.ReadAllLines(Application.persistentDataPath + levelPath)[0];
        //     LevelSettingsData levelSettingsData = JsonUtility.FromJson<LevelSettingsData>(levelSettingsJson);
        //     levelSettingsData.Read();
        // }

        // public static void SaveLevelSettings() {
        //     Debug.Log("Saving");
        //     LevelSettingsData levelSettingsData = new LevelSettingsData();
        //     string levelSettingsJson = JsonUtility.ToJson(levelSettingsData);
        //     string levelPath = LEVEL_FILE_PATH + levelSettingsData.fileName + ".json";
        //     System.IO.File.WriteAllText(Application.persistentDataPath + levelPath, levelSettingsJson);
        // }

        // public static void CreateNewLevelSettingsFile() {
        //     Debug.Log("New File");
        //     LevelSettingsData levelSettingsData = LevelSettingsData.NewFile();
        //     string levelSettingsJson = JsonUtility.ToJson(levelSettingsData);
        //     string levelPath = LEVEL_FILE_PATH + levelSettingsData.fileName + ".json";
        //     System.IO.File.WriteAllText(Application.persistentDataPath + levelPath, levelSettingsJson);
        // }

        #endregion

    }

}
