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

/* --- Definitions --- */
using Game = Platformer.GameManager;

namespace Platformer.Levels {

    /// <summary>
    ///
    /// <summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class LevelSection : MonoBehaviour {

        #region Enumerations.

        public enum State {
            Loaded,
            Unloaded
        }

        #endregion

        #region Fields.

        /* --- Constants --- */

        // The slight shave off the boundary box for entering/exiting.
        private const float BOUNDARYBOX_SHAVE = 0.375f; // 0.1f; // 0.775f;

        /* --- Components --- */

        // The trigger box for the level.
        public CameraNode cameraNode;

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
        public List<LDtkEntity> entities { get; private set; } = new List<LDtkEntity>();

        // The positions where the player can be loaded.
        [field: SerializeField, ReadOnly] 
        public List<Vector2Int> loadPositions { get; private set; } = new List<Vector2Int>();

        #endregion

        public void Preload(int jsonID, LdtkJson  json) {
            transform.localPosition = Vector3.zero;
            ReadJSONData(json, jsonID);
            CreateCameraNode();

            // List<LDtkTileData> controlData = LDtkReader.GetLayerData(json.Levels[jsonID], Game.Level.LDtkLayers.Control);
            // GetLoadPoints(controlData);
        }

        void FixedUpdate() {
            if (state != State.Loaded) { return; }

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

        public void CreateCameraNode() {
            if (cameraNode != null) { DestroyImmediate(cameraNode.gameObject); }

            cameraNode = new GameObject(gameObject.name + " Camera Node", typeof(CameraNode)).GetComponent<CameraNode>();
            cameraNode.transform.SetParent(transform);

            BoxCollider2D box = cameraNode.gameObject.GetComponent<BoxCollider2D>();
            box.size = new Vector2((float)(width - 1.99f * BOUNDARYBOX_SHAVE), (float)(height - 1.99f * BOUNDARYBOX_SHAVE));
            // box.offset = worldCenter;
            box.isTrigger = true;

            cameraNode.transform.position = worldCenter; // transform.position;
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<BoxCollider2D>().size = box.size * 1.05f;
            GetComponent<BoxCollider2D>().offset = worldCenter;

        }

        public void GenerateEntities(LDtkEntityManager entityManager, LDtkLayers ldtkLayers) {
            entities.RemoveAll(entity => entity == null);
            entities = entityManager.Generate(this, ldtkLayers);
            entities = entities.FindAll(entity => entity != null);
        }

        public void DestroyEntities() {
            // entities = LDtkEntity.Destroy(entities);
            for (int i = 0; i < entities.Count; i++) {
                DestroyImmediate(entities[i].gameObject);
            }
            entities.RemoveAll(entity => entity == null);
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
