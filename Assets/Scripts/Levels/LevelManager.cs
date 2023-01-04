/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.Levels;
using Platformer.Levels.LDtk;
using Platformer.Levels.Tilemaps;
using Platformer.Levels.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using SaveSystem = Platformer.Management.SaveSystem;
using ScoreOrb = Platformer.Objects.Orbs.ScoreOrb;

namespace Platformer.Management {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class LevelManager: MonoBehaviour {
        
        /* --- Variables --- */
        #region Variables

        // Handles all the entity functionality.
        [SerializeField] 
        public EntityManager m_EntityManager;
        public EntityManager Entities => m_EntityManager;

        // Handles all the tilemap functionality.
        [SerializeField] 
        public TilemapManager m_TilemapManager;
        public TilemapManager Tilemaps => m_TilemapManager;

        // The JSON data corresponding to the given ldtk data.
        [HideInInspector]
        private LDtkLayers m_LDtkLayers = new LDtkLayers();
        public LDtkLayers LDtkLayers => m_LDtkLayers;

        // A reference to all the created levels.
        [SerializeField, ReadOnly] 
        public List<Room> m_Rooms = new List<Room>();

        // The last loaded room.
        [SerializeField, ReadOnly]
        private Room m_CurrentRoom = null;
        public Room CurrentRoom => m_CurrentRoom;

        // The name of the first room to be loaded.
        [SerializeField] 
        public string m_FirstRoomName = "";
        
        // The given LDtk file.
        [SerializeField] 
        private LDtkComponentProject m_LDtkData;

        // The JSON data corresponding to the given ldtk data.
        [HideInInspector]
        private LdtkJson m_JSON;

        [HideInInspector]
        private int m_Deaths = 0;

        [HideInInspector]
        private int m_Points = 0;
        
        #endregion

        // Initializes the world.
        public void OnGameLoad() {
            m_FirstRoomName = LevelSettings.FirstRoomName != "" ? LevelSettings.FirstRoomName : m_FirstRoomName;
            if (LevelSettings.CurrentLevelData == null) {
                LevelSettings.CurrentLevelData = m_LDtkData;
            }
            m_LDtkData = LevelSettings.CurrentLevelData;
            // Load the sub-managers.
            m_TilemapManager.OnGameLoad();
            m_EntityManager.OnGameLoad();
            // Read and collect the data.
            m_JSON = m_LDtkData.FromJson();
            m_Rooms = Collect(m_JSON, transform);
            // Load the maps for all the levels.
            Preload();
            MoveToLoadPoint(m_FirstRoomName, Game.MainPlayer.transform);
        }

        /* --- start room manager --- */
        
        // Collects all the levels from the LDtk file.
        private static List<Room> Collect(LdtkJson json, Transform transform) {
            List<Room> rooms = new List<Room>();
            for (int i = 0; i < json.Levels.Length; i++) {
                Room room = new GameObject(json.Levels[i].Identifier, typeof(Room)).GetComponent<Room>();
                room.transform.SetParent(transform);
                room.Preload(i, json);
                rooms.Add(room);
            }
            return rooms;
        }

        // This should go in tilemap manaager

        // Loads the map layouts for all the given levels.
        public void Preload() {
            // Load the custom tile mappings.
            CustomTileMappings.CreateGroundTileMapping();
            CustomTileMappings.CreateWaterTileMapping();

            // Itterate through and load all the level data.
            for (int i = 0; i < m_Rooms.Count; i++) {
                List<LDtkTileData> tileData = LDtkReader.GetLayerData(m_Rooms[i].ldtkLevel, m_LDtkLayers.Ground);
                m_TilemapManager.GenerateMap(m_Rooms[i], tileData);
            }

            // Refresh all the maps once after all the data has been loaded.
            m_TilemapManager.waterMap.RefreshAllTiles();
            m_TilemapManager.groundMap.RefreshAllTiles();
            m_TilemapManager.groundMaskMap.RefreshAllTiles();
            // Level.GroundMap.GetComponent<ShadowCaster2DTileMap>().Generate(0.5f);

        }

        public void MoveToLoadPoint(string roomName, Transform playerTransform) {
            Room room = m_Rooms.Find(level => level.roomName == roomName);
            if (room.loadPositions != null && room.loadPositions.Count > 0) {
                Vector3 position = Room.GridToWorldPosition(room.loadPositions[0], room.worldPosition);
                playerTransform.position = position;
                Rigidbody2D body = playerTransform.GetComponent<Rigidbody2D>();
                if (body != null) {
                    body.velocity = Vector2.zero;
                }
            }
        }

        // Resets the current room.
        public void Reset() {
            // Should go somewhere saying custom.
            Platformer.Objects.Blocks.BlockObject.ResetAll();
            Platformer.Objects.Orbs.OrbObject.ResetAll();
            Game.Visuals.Camera.RecolorScreen(Game.Visuals.DefaultPalette);
        }

        // Loads the entities for 
        public void Load(Room room) {
            if (room.ldtkLevel != null) {
                // Load the data.
                List<LDtkTileData> entityData = LDtkReader.GetLayerData(room.ldtkLevel, m_LDtkLayers.Entity);
                List<LDtkTileData> controlData = LDtkReader.GetLayerData(room.ldtkLevel, m_LDtkLayers.Control);

                // Load the level.
                room.GenerateEntities(entityData, controlData, m_EntityManager.All);
                room.Settings(controlData);
                m_CurrentRoom = room;
                Game.Visuals.Camera.SetTarget(m_CurrentRoom.worldCenter);
            }
        }

        public void Unload(Room room) {
            if (m_CurrentRoom == room) {
                m_CurrentRoom = null;
            }
            room.DestroyEntities();
            Platformer.Objects.Spitters.Projectile.DeleteAll(); // Should go somewhere saying custom.
        }

        /* --- end room manager --- */

        public void AddDeath() {
            m_Deaths += 1;
            SaveSystem.SaveLevelSettings();
        }

        public void AddPoint(ScoreOrb scoreOrb) {
            m_Points += 1;
        }

        public void OnSaveAndQuit() {
            LevelSettings.CurrentPoints = m_Points;
            LevelSettings.CurrentDeaths = m_Deaths;
            LevelSettings.CurrentTime = Game.Physics.Time.Ticks;
        }

        public void OnComplete() {
            LevelSettings.CompletedPoints = m_Points;
            LevelSettings.CompletedDeaths = m_Deaths;
            LevelSettings.CompletedTime = Game.Physics.Time.Ticks;
        }

    }

}
    