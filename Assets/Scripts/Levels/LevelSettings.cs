/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

/* -- Definitions --- */
using LDtkTileData = Platformer.Levels.LDtk.LDtkTileData;
using LDtkReader = Platformer.Levels.LDtk.LDtkReader;
using SaveSystem = Platformer.Management.SaveSystem;

namespace Platformer.Levels {

    ///<summary>
    /// The settings for using the visuals in the game.
    ///<summary>
    public static class LevelSettings {

        #region Data.

        // Saveable???
        [System.Serializable]
        public class LevelSettingsData {
            
            public string levelName;
            public string fileName;
            public int maxPoints;
            
            public int currentPoints;
            public int currentDeaths;
            public float currentTime;
            
            public int completePoints;
            public int completeDeaths;
            public float completeTime;

            public LevelSettingsData() {
                levelName = LevelSettings.LevelName;
                fileName = LevelSettings.FileName;
                maxPoints = LevelSettings.MaxPoints;
                
                currentPoints = LevelSettings.CurrentPoints;
                currentDeaths = LevelSettings.CurrentDeaths;
                currentTime = LevelSettings.CurrentTime;

                completePoints = LevelSettings.CompletedPoints;
                completeDeaths = LevelSettings.CompletedDeaths;
                completeTime = LevelSettings.CompletedTime;
            }

            public static LevelSettingsData NewFile() {
                LevelSettingsData levelSettingsData = new LevelSettingsData();

                levelSettingsData.currentPoints = 0;
                levelSettingsData.currentDeaths = 0;
                levelSettingsData.currentTime = 0f;

                levelSettingsData.completePoints = -1;
                levelSettingsData.completeDeaths = -1;
                levelSettingsData.completeTime = -1f;

                return levelSettingsData;
            }

            public void Read() {
                Debug.Log(LevelSettings.LevelName);

                LevelSettings.CurrentPoints = this.currentPoints;
                LevelSettings.CurrentDeaths = this.currentDeaths;
                LevelSettings.CurrentTime = this.currentTime;
                
                // Prime them to be read into.
                LevelSettings.CompletedPoints = -1;
                LevelSettings.CompletedDeaths = -1;
                LevelSettings.CompletedTime = -1f;

                LevelSettings.CompletedPoints = this.completePoints;
                LevelSettings.CompletedDeaths = this.completeDeaths;
                LevelSettings.CompletedTime = this.completeTime;

            }

        }

        #endregion

        // The amount of pixels per unit, essentially, the quality.
        private static LDtkComponentProject m_CurrentLevelData = null;
        public static LDtkComponentProject CurrentLevelData {
            get { return m_CurrentLevelData; }
            set { 
                m_FirstRoomName = "START";
                m_CurrentLevelData = value;
                SaveSystem.CheckForLevelSettings();
            }
        }

        // The start room name.
        private static string m_FirstRoomName = "";
        public static string FirstRoomName => m_FirstRoomName;

        // The 
        public static string MechanicsRoom = "MECHANICS";

        // Gets the level name based on the given index.
        public static string LevelName => GetLevelName(m_CurrentLevelData);
        public static string GetLevelName(LDtkComponentProject ldtkData) {
            LdtkJson json = ldtkData.FromJson();
            string levelName = json.LevelNamePattern;
            levelName = levelName.Replace('_', ' ');
            levelName = levelName.Split('%')[0];
            return levelName;
        }

        public static string FileName => GetFileName(m_CurrentLevelData);
        public static string GetFileName(LDtkComponentProject ldtkData) {
            LdtkJson json = ldtkData.FromJson();
            string levelName = json.LevelNamePattern;
            levelName = levelName.Split('%')[0];
            levelName = levelName.ToLower();
            return levelName;
        }

        // Gets the number of stars in the level.
        public static int MaxPoints => GetPointsAvailable(m_CurrentLevelData);
        public static int GetPointsAvailable(LDtkComponentProject ldtkData) {
            LdtkJson json = ldtkData.FromJson();
            int total = 0;

            for (int n = 0; n < json.Levels.Length; n++) {
                if (json.Levels[n].Identifier != LevelSettings.MechanicsRoom) {
                    List<LDtkTileData> entityData = LDtkReader.GetLayerData(json.Levels[n], "ENTITIES"); // Fuck me.
                    total += entityData.FindAll(entity => entity.vectorID == LDtkTileData.ScoreOrbID).Count;
                }
            }
            return total;
        }

        // The most amount of stars collected in a completed run.
        private static int m_CompletedDeaths = -1;
        public static int CompletedDeaths {
            get { return m_CompletedDeaths; }
            set {
                if (m_CompletedDeaths == -1 || value == -1) {
                    m_CompletedDeaths = value;
                }
                else if (value < m_CompletedDeaths ) {
                    m_CompletedDeaths = value;
                }
            }

        }

        // The most amount of stars collected in a completed run.
        // This needs to be an array in order to properly track 
        // the actual stars position. Its not just a number.
        private static int m_CompletedPoints = 0;
        public static int CompletedPoints {
            get { return m_CompletedPoints; }
            set {
                if (m_CompletedPoints == -1 || value == -1) {
                    m_CompletedPoints = value;
                }
                else if (value > m_CompletedPoints) {
                    m_CompletedPoints = value;
                }
            }
        }

        // The best time in a completed run.
        private static float m_CompletedTime = -1f;
        public static float CompletedTime {
            get { return m_CompletedTime; }
            set {
                if (m_CompletedTime == -1f || value == -1f) {
                    m_CompletedTime = value;
                }
                else if (value < m_CompletedTime) {
                    m_CompletedTime = value;
                }
            }
        }
        
        // Need to store these in case a player wants to
        // save and exit mid run.
        private static int m_CurrentPoints = 0;
        public static int CurrentPoints {
            get { return m_CurrentPoints; }
            set { m_CurrentPoints = value; }
        }

        private static int m_CurrentDeaths = 0;
        public static int CurrentDeaths {
            get { return m_CurrentDeaths; }
            set { m_CurrentDeaths = value; }
        }
        
        private static float m_CurrentTime = 0;
        public static float CurrentTime {
            get { return m_CurrentTime; }
            set { m_CurrentTime = value; }
        }

    }

}