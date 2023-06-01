// TODO: Clean

/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using LDtkUnity;
// Platformer.
using Platformer.Character;
using Platformer.Levels;
using Platformer.Levels.LDtk;
using Platformer.Levels.Tilemaps;
using Platformer.Levels.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels {

    /// <summary>
    ///
    /// <summary>
    public class Room : MonoBehaviour {

        #region Enumerations.

        public enum State {
            Loaded,
            Unloaded
        }

        #endregion

        #region Fields.

        /* --- Constants --- */

        // The slight shave off the boundary box for entering/exiting.
        private const float BOUNDARYBOX_SHAVE = 0.775f;

        /* --- Components --- */

        // The trigger box for the level.
        private BoxCollider2D box => GetComponent<BoxCollider2D>();

        /* --- Members --- */

        // Whether this level is currently loaded.
        [SerializeField, ReadOnly]
        private State state = State.Unloaded;  

        // The id of this level.
        [field: SerializeField, ReadOnly]
        public int id { get; private set; } = 0;
        
        // The name of this level.
        [field: SerializeField, ReadOnly] 
        public string roomName { get; private set; } = "";

        // The actual ldtk data of the level. 
        [field: SerializeField, ReadOnly] 
        public LDtkUnity.Level ldtkLevel { get; private set; } = null;

        // The dimensions of the level.
        [SerializeField, ReadOnly] 
        private Vector2Int dimensions;

        // The height of the level based on the dimensions
        public int height => dimensions.y;

        // The width of the level based on the dimensions
        public int width => dimensions.x;

        // The position of the bottom left corner of the level in the world.
        [SerializeField, ReadOnly] 
        public Vector2Int worldPosition;

        // The position of the center of the level in the world.
        public Vector2 worldCenter => GetCenter(this.width, this.height, this.worldPosition);

        // The entities currently loaded into the level.
        [field: SerializeField, ReadOnly] 
        public List<Entity> entities { get; private set; } = new List<Entity>();

        // The positions where the player can be loaded.
        [field: SerializeField, ReadOnly] 
        public List<Vector2Int> loadPositions { get; private set; } = new List<Vector2Int>();

        #endregion

        public void Preload(int jsonID, LdtkJson  json) {
            transform.localPosition = Vector3.zero;
            ReadJSONData(json, jsonID);
            CreateBoundaryBox();

            List<LDtkTileData> controlData = LDtkReader.GetLayerData(json.Levels[jsonID], Game.Level.LDtkLayers.Control);
            GetLoadPoints(controlData);
        }

        void FixedUpdate() {
            if (state != State.Loaded) { return; }

            for (int i = 0; i < dimensions.y; i++) {
                for (int j = 0; j < dimensions.x; j++) {
                    Vector3Int position = new Vector3Int(worldPosition.x + j, worldPosition.y - i, 0);
                    Game.Level.Tilemaps.waterMap.RefreshTile(position);
                }
            }

        }

        public void ReadJSONData(LdtkJson  json, int jsonID) {
            id = jsonID;
            ldtkLevel = json.Levels[jsonID];
            roomName = json.Levels[jsonID].Identifier;
            dimensions.y = (int)(json.Levels[jsonID].PxHei / json.DefaultGridSize);
            dimensions.x = (int)(json.Levels[jsonID].PxWid / json.DefaultGridSize);
            worldPosition.y = (int)(json.Levels[jsonID].WorldY / json.DefaultGridSize);
            worldPosition.x = (int)(json.Levels[jsonID].WorldX / json.DefaultGridSize);
        }

        public void CreateBoundaryBox() {
            gameObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2((float)(width - BOUNDARYBOX_SHAVE), (float)(height - BOUNDARYBOX_SHAVE));
            box.offset = worldCenter;
            box.isTrigger = true;
        }

        public void GenerateEntities(List<LDtkTileData> entityData, List<LDtkTileData> controlData, List<Entity> entityReferences, string layerName) {
            entities.RemoveAll(entity => entity == null);
            entities = Entity.Generate(entities, entityData, controlData, entityReferences, transform, worldPosition, layerName);
        }

        public void DestroyEntities() {
            entities = Entity.Destroy(entities);
            entities.RemoveAll(entity => entity == null);
        }

        public void GetLoadPoints(List<LDtkTileData> controlData) {
            loadPositions = new List<Vector2Int>();
            for (int j = 0; j < controlData.Count; j++) {
                if (controlData[j].vectorID == LDtkTileData.LOADPOINT_ID) {
                    loadPositions.Add(controlData[j].gridPosition);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Game.Level.Load(this);
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Game.Level.Unload(this);
            }
        }

        public static Vector2 GetCenter(int width, int height, Vector2Int gridOrigin) {
            Vector2Int origin = new Vector2Int(width / 2, height / 2);
            Vector2 offset = new Vector2( width % 2 == 0 ? 0.5f : 0f, height % 2 == 1 ? 0f : -0.5f);
            // Assuming a grid size of 16.
            return (Vector2)GridToWorldPosition(origin, gridOrigin, 16) - offset;
        }

        public Vector3 GridToWorldPosition(Vector2Int gridPosition, int gridSize) {
            return GridToWorldPosition(gridPosition, this.worldPosition, gridSize);
        }
        
        public static Vector3 GridToWorldPosition(Vector2Int gridPosition, Vector2Int gridOrigin, int gridSize) {
            float ratio = (float)gridSize / (float)LDtkReader.GridSize;

            return new Vector3((ratio * gridPosition.x + gridOrigin.x) + 0.5f, - (ratio * gridPosition.y + gridOrigin.y) + 0.5f, 0f);
        }

        public Vector3Int GridToTilePosition(Vector2Int gridPosition) {
            return GridToTilePosition(gridPosition, this.worldPosition);
        }

        public static Vector3Int GridToTilePosition(Vector2Int gridPosition, Vector2Int gridOrigin) {
            return new Vector3Int(gridPosition.x + gridOrigin.x, -(gridPosition.y + gridOrigin.y), 0);
        }

    }

}
