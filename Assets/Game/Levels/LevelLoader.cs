/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class LevelLoader : MonoBehaviour {

    /* --- Static Variables --- */
    // Layer Names
    public static string AnimalLayer = "Animal_Layer";
    public static string ObstacleLayer = "Obstacle_Layer";
    public static string ControlLayer = "Control_Layer";
    public static string FloorLayer = "Tile_Layer";

    /* --- Data Structures --- */
    public class LDtkTileData {

        /* --- Properties --- */
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public int index;

        /* --- Constructor --- */
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
        }

    }

    /* --- Components --- */
    [SerializeField] private LDtkComponentProject lDtkData;
    [SerializeField] private Level level;

    /* --- Parameters --- */
    [SerializeField] public bool load;
    [SerializeField] private int id;


    /* --- Unity --- */
    // Runs once before the first frame.
    private void Update() {
        if (load) {
            ResetLevel(level);
            OpenLevel(id);
            load = false;
        }
    }

    /* --- Methods --- */
    private void OpenLevel(int id) {
        LDtkLevel ldtkLevel = GetLevelByID(lDtkData, id);
        OpenLevel(ldtkLevel);
    }

    private LDtkLevel GetLevelByID(LDtkComponentProject lDtkData, int id) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        // Read the json data.
        level.gridSize = (int)json.DefaultGridSize;
        level.height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        level.width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

        // Grab the level by the id.
        if (id < json.Levels.Length && id >= 0) {
            return json.Levels[id];
        }
        Debug.Log("Could not find room");
        return null;
    }

    protected void OpenLevel(LDtkLevel ldtkLevel) {

        if (ldtkLevel != null) {

            // Load the entity data.
            int gridSize = level.gridSize;
            List<LDtkTileData> floorData = LoadLayer(ldtkLevel, FloorLayer, gridSize);
            List<LDtkTileData> controlData = LoadLayer(ldtkLevel, ControlLayer, gridSize);
            List<LDtkTileData> animalData = LoadLayer(ldtkLevel, AnimalLayer, gridSize);
            List<LDtkTileData> obstacleData = LoadLayer(ldtkLevel, ObstacleLayer, gridSize);

            // Grab the data from the level.
            List<Entity> animalList = level.environment.animals;
            List<Entity> obstacleList = level.environment.obstacles;
            Tilemap floormap = level.floorMap;

            // Instatiantate and set up the entities using the data.
            level.animals = LoadEntities(animalData, animalList);
            level.obstacles = LoadEntities(obstacleData, obstacleList);
            level.nonEmptyTiles = LoadTiles(level, floormap, floorData);

            // Set the controls.
            SetControls(controlData, level.obstacles);
        }

    }

    private void ResetLevel(Level level) {

        GameRules.MainPlayer.transform.SetParent(null);

        if (level.animals != null) {
            print("Resetting Entities");
            for (int i = 0; i < level.animals.Count; i++) {
                Destroy(level.animals[i].gameObject);
            }
        }
        level.animals = new List<Entity>();

        if (level.obstacles != null) {
            print("Resetting Obstacles");
            for (int i = 0; i < level.obstacles.Count; i++) {
                Destroy(level.obstacles[i].gameObject);
            }
        }
        level.obstacles = new List<Entity>();

        if (level.nonEmptyTiles != null) {
            print("Resetting Tiles");
            for (int i = 0; i < level.nonEmptyTiles.Count; i++) {
                level.floorMap.SetTile(level.nonEmptyTiles[i], null);
            }
        }
        level.nonEmptyTiles = new List<Vector3Int>();

    }

    private LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    // Returns the vector ID's of all the tiles in the layer.
    private List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, List<LDtkTileData> layerData = null) {

        if (layerData == null) { layerData = new List<LDtkTileData>(); }

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            // Itterate through the tiles in the layer and get the directions at each position.
            for (int index = 0; index < layer.GridTiles.Length; index++) {

                // Get the tile at this index.
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / gridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / gridSize;

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(vectorID, gridPosition, index);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    private List<Entity> LoadEntities(List<LDtkTileData> entityData, List<Entity> entityList) {

        List<Entity> entities = new List<Entity>();

        for (int i = 0; i < entityData.Count; i++) {
            // Get the entity based on the environment.
            Entity entityBase = level.environment.GetEntityByVectorID(entityData[i].vectorID, entityList);
            if (entityBase != null) {

                // Instantiate the entity
                Vector3 entityPosition = level.GridToWorldPosition(entityData[i].gridPosition);
                Entity newEntity = Instantiate(entityBase.gameObject, entityPosition, Quaternion.identity, level.transform).GetComponent<Entity>();
                newEntity.transform.localPosition = entityPosition;

                // Set up the entity.
                newEntity.gameObject.SetActive(true);
                newEntity.gridPosition = entityData[i].gridPosition;

                // Add the entity to the list
                entities.Add(newEntity);
            }
        }
        return entities;
    }

    // Set all the tiles in a tilemap.
    private List<Vector3Int> LoadTiles(Level level, Tilemap tilemap, List<LDtkTileData> data) {
        List<Vector3Int> nonEmptyTiles = new List<Vector3Int>();
        for (int i = 0; i < data.Count; i++) {
            TileBase tile = level.environment.floorTile;
            // if (data[i].vectorID != new Vector2Int(0))
            Vector3Int tilePosition = level.GridToTilePosition(data[i].gridPosition);
            tilemap.SetTile(tilePosition, tile);
            //
            nonEmptyTiles.Add(tilePosition);
        }

        return nonEmptyTiles;
    }

    private Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
        for (int i = 0; i < data.Count; i++) {
            if (gridPosition == data[i].gridPosition) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }

    private void SetControls(List<LDtkTileData> controlData, List<Entity> obstacles) {

        // Set the player.
        for (int i = 0; i < controlData.Count; i++) {
            if (controlData[i].vectorID == new Vector2Int(5, 0)) {
                Vector3 controlPosition = level.GridToWorldPosition(controlData[i].gridPosition);
                GameRules.MainPlayer.transform.position = controlPosition;
                GameRules.MainPlayer.think = true;
                GameRules.MainPlayer.body.velocity = Vector2.zero;
                GameRules.MainPlayer.gameObject.SetActive(true);

            }

        }

        // Set the obstacles.
        for (int i = 0; i < obstacles.Count; i++) {

            MovingPlatform movingPlatform = obstacles[i].GetComponent<MovingPlatform>();
            print(i);
            if (movingPlatform != null) {
                Vector2Int gridPosition = obstacles[i].gridPosition;
                SetMovingPlatform(movingPlatform, gridPosition, controlData);
            }
        }

    }

    private void SetMovingPlatform(MovingPlatform movingPlatform, Vector2Int gridPosition, List<LDtkTileData> controlData) {

        // Get the control point.
        LDtkTileData controlPoint = null;
        for (int i = 0; i < controlData.Count; i++) {
            if (controlData[i].gridPosition == gridPosition) {
                controlPoint = controlData[i];
            }
        }
        if (controlPoint == null) {
            return;
        }
        print("found control point");
       
        // Get the direction.
        int directionID = controlPoint.vectorID.x;
        Vector2Int direction = Vector2Int.zero;
        if (directionID == 0) {
            direction = Vector2Int.down;
        }
        else if (directionID == 1) {
            direction = Vector2Int.up;
        }
        else if (directionID == 2) {
            direction = Vector2Int.right;
        }
        else if (directionID == 3) {
            direction = Vector2Int.left;
        }
        if (direction == Vector2Int.zero) {
            return;
        }
        print("found direction");

        // Find the second point.
        // Super inefficient.
        LDtkTileData beaconPoint = null;
        Vector2Int beaconID = new Vector2Int(4, 0);
        Vector2Int nextPosition = gridPosition + direction;
        int recursions = 0;
        while (beaconPoint == null && recursions < 50) {
            for (int i = 0; i < controlData.Count; i++) {
                if (controlData[i].gridPosition == nextPosition) {
                    if (controlData[i].vectorID == beaconID) {
                        // Found beacon.
                        beaconPoint = controlData[i];
                        break;
                    }
                }
            }
            recursions += 1;
            nextPosition += direction;
        }
        if (beaconPoint == null) {
            return;
        }
        print("found beacon");

        // Create the path points.
        Transform pointA = new GameObject("Point A").transform;
        pointA.position = level.GridToWorldPosition(controlPoint.gridPosition);
        Transform pointB = new GameObject("Point B").transform;
        pointB.position = level.GridToWorldPosition(beaconPoint.gridPosition);

        List<Transform> points = new List<Transform>();
        points.Add(pointA);
        points.Add(pointB);

        // Raycast out to find the different blocks.
        MovingPlatform prevPlatform = movingPlatform;
        int size = 1;
        bool didNotFindAnything = false;
        List<MovingPlatform> destroyThesePlatforms = new List<MovingPlatform>();
        recursions = 0;

        while (!didNotFindAnything && recursions < 50) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(prevPlatform.transform.position, Vector2.right, 1f);
            for (int i = 0; i < hits.Length; i++) {
                MovingPlatform nextPlatform = hits[i].collider.GetComponent<MovingPlatform>();
                if (nextPlatform != null && nextPlatform != prevPlatform) {
                    size += 1;
                    prevPlatform = nextPlatform;
                    destroyThesePlatforms.Add(nextPlatform);
                    recursions += 1;
                    break;
                }
            }
            didNotFindAnything = true;
        }
        for (int i = 0; i < destroyThesePlatforms.Count; i++) {
            Destroy(destroyThesePlatforms[i].gameObject);
        }
        destroyThesePlatforms = null;

        Transform endPoint = new GameObject("End Point").transform;
        endPoint.SetParent(movingPlatform.transform);
        endPoint.localPosition = Vector3.right * size;

        movingPlatform.Init(endPoint, points);

    }

}
