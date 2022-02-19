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
    public static string ControlLayer = "Control";
    public static string DecorLayer = "Decor";
    public static string SnailLayer = "Snails";
    public static string SpiritLayer = "Spirits";
    public static string PlatformLayer = "Platforms";
    public static string GroundLayer = "Ground";

    public static Vector2Int CheckPointID = new Vector2Int(0, 3);

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
    [SerializeField] public LDtkComponentProject lDtkData;
    [SerializeField] public Level level;

    /* --- Parameters --- */
    [SerializeField] public bool load;
    [SerializeField] public int id;


    /* --- Unity --- */
    // Runs once before the first frame.
    protected virtual void Update() {
        if (load) {
            ResetLevel(level);
            OpenLevel(level, id);
            load = false;
        }
    }

    /* --- Methods --- */
    public void OpenLevel(Level level, int id, bool resetPlayer = true) {
        LDtkLevel ldtkLevel = GetLevelByID(level, lDtkData, id);
        OpenLevel(level, ldtkLevel, resetPlayer);
    }

    protected LDtkLevel GetLevelByID(Level level, LDtkComponentProject lDtkData, int id) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        for (int i = 0; i < json.Levels.Length; i++) {
            // print(json.Levels[i].Identifier);
            if (json.Levels[i].Identifier == "Level_" + id.ToString()) {

                // Read the json data.
                level.gridSize = (int)json.DefaultGridSize;
                level.height = (int)(json.Levels[i].PxHei / json.DefaultGridSize);
                level.width = (int)(json.Levels[i].PxWid / json.DefaultGridSize);

                level.worldHeight = (int)(json.Levels[i].WorldY / json.DefaultGridSize);
                level.worldWidth = (int)(json.Levels[i].WorldX / json.DefaultGridSize);

                return json.Levels[i];

            }
        }
        
        Debug.Log("Could not find room");
        return null;
    }

    public void OpenLevelByName(Level level, string levelName, bool resetPlayer = true) {
        LDtkLevel ldtkLevel = GetLevelByName(level, lDtkData, levelName);
        OpenLevel(level, ldtkLevel, resetPlayer);
    }

    protected LDtkLevel GetLevelByName(Level level, LDtkComponentProject lDtkData, string levelName) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        for (int i = 0; i < json.Levels.Length; i++) {
            // print(json.Levels[i].Identifier);
            if (json.Levels[i].Identifier == levelName) {

                // Read the json data.
                level.gridSize = (int)json.DefaultGridSize;
                level.height = (int)(json.Levels[i].PxHei / json.DefaultGridSize);
                level.width = (int)(json.Levels[i].PxWid / json.DefaultGridSize);

                level.worldHeight = (int)(json.Levels[i].WorldY / json.DefaultGridSize);
                level.worldWidth = (int)(json.Levels[i].WorldX / json.DefaultGridSize);

                return json.Levels[i];

            }
        }

        Debug.Log("Could not find room");
        return null;
    }

    protected void OpenLevel(Level level, LDtkLevel ldtkLevel, bool resetPlayer = true) {

        if (ldtkLevel != null) {

            // Load the entity data.
            int gridSize = level.gridSize;
            List<LDtkTileData> controlData = LoadLayer(ldtkLevel, ControlLayer, gridSize);
            List<LDtkTileData> decorData = LoadLayer(ldtkLevel, DecorLayer, gridSize);
            List<LDtkTileData> snailData = LoadLayer(ldtkLevel, SnailLayer, gridSize);
            List<LDtkTileData> spiritData = LoadLayer(ldtkLevel, SpiritLayer, gridSize);
            List<LDtkTileData> platformData = LoadLayer(ldtkLevel, PlatformLayer, gridSize);
            List<LDtkTileData> groundData = LoadLayer(ldtkLevel, GroundLayer, gridSize);

            // Grab the data from the level.
            // List<Entity> animalList = level.environment.animals;
            // List<Entity> obstacleList = level.environment.obstacles;
            Tilemap floormap = level.floorMap;

            // Instatiantate and set up the entities using the data.
            if (level.controls == null || level.controls.Count == 0) {
                level.controls = LoadEntities(level, controlData, level.environment.controls);
            }
            if (level.decor == null || level.decor.Count == 0) {
                level.decor = LoadEntities(level, decorData, level.environment.decor);
            }
            level.snails = LoadEntities(level, snailData, level.environment.snails);
            level.spirits = LoadEntities(level, spiritData, level.environment.spirits);
            level.platforms = LoadEntities(level, platformData, level.environment.platforms);
            level.nonEmptyTiles = LoadTiles(level, floormap, groundData);

            // Set the controls.
            SetPlatforms(level, controlData, level.platforms);
            SetControls(level);
        }

    }

    public virtual void Reset() {
        ResetLevel(level);
    }

    protected void ResetLevel(Level level) {

        // GameRules.MainPlayer.transform.SetParent(null);

        ResetEntities(ref level.decor);
        ResetEntities(ref level.snails);
        ResetEntities(ref level.spirits);
        ResetEntities(ref level.platforms);

        if (level.nonEmptyTiles != null) {
            for (int i = 0; i < level.nonEmptyTiles.Count; i++) {
                level.floorMap.SetTile(level.nonEmptyTiles[i], null);
            }
        }
        level.nonEmptyTiles = new List<Vector3Int>();

    }

    private void ResetEntities(ref List<Entity> entities) {
        if (entities != null) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] != null) {
                    Destroy(entities[i].gameObject);
                }
            }
        }
        entities = new List<Entity>();
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
    protected List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, List<LDtkTileData> layerData = null) {

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

    private List<Entity> LoadEntities(Level level, List<LDtkTileData> entityData, List<Entity> entityList) {

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

    private void SetControls(Level level) {

        // Set the obstacles.
        for (int i = 0; i < level.controls.Count; i++) {
            LevelLabel levelLabel = level.controls[i].GetComponent<LevelLabel>();
            if (levelLabel != null) {
                levelLabel.Init(level.levelName);
            }
            if (level.controls[i].vectorID == new Vector2Int(0, 3)) {
                foreach (Transform child in level.controls[i].transform) {
                    Hearthbox hearthbox = child.GetComponent<Hearthbox>();
                    if (hearthbox != null) {
                        hearthbox.Init(level);
                    }
                }
            }
            
        }

    }

    private void SetPlatforms(Level level, List<LDtkTileData> controlData, List<Entity> platforms, bool resetPlayer = true) {

        // Set the obstacles.
        for (int i = 0; i < platforms.Count; i++) {
            MovingPlatform movingPlatform = platforms[i].GetComponent<MovingPlatform>();
           // print("found platform");
            if (movingPlatform != null) {
                Vector2Int gridPosition = platforms[i].gridPosition;
                SetMovingPlatform(level, movingPlatform, gridPosition, controlData);
            }

        }

    }

    private void SetMovingPlatform(Level level, MovingPlatform movingPlatform, Vector2Int gridPosition, List<LDtkTileData> controlData) {

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
        // print("found control point");

        // Get the speed.
        int speedID = controlPoint.vectorID.y;
        float speed = 0f;
        if (speedID == 0) {
            // slow
            speed = MovingPlatform.SlowSpeed;
        }
        else if (speedID == 1) {
            // mid
            speed = MovingPlatform.MidSpeed;
        }
        else if (speedID == 2) {
            // fast
            speed = MovingPlatform.FastSpeed;
        }
        else {
            return;
        }

        // Get the direction.
        int directionID = controlPoint.vectorID.x;
        Vector2Int direction = Vector2Int.zero;
        if (directionID == 0) {
            direction = Vector2Int.right;
        }
        else if (directionID == 1) {
            direction = Vector2Int.down;
        }
        else if (directionID == 2) {
            direction = Vector2Int.left;
        }
        else if (directionID == 3) {
            direction = Vector2Int.up;
        }
        else {
            return;
        }
        // print("found direction");

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
        // print("found beacon");

        // Raycast out to find the different blocks.
        MovingPlatform prevPlatform = movingPlatform;
        int size = 1;
        bool didNotFindAnything = false;
        List<MovingPlatform> destroyThesePlatforms = new List<MovingPlatform>();
        recursions = 0;

        while (!didNotFindAnything && recursions < 50) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(prevPlatform.transform.position + Vector3.right * 0.5f, Vector3.right * 0.25f, 1f);
            // print(prevPlatform.transform.position);
            didNotFindAnything = true;
            for (int i = 0; i < hits.Length; i++) {
                MovingPlatform nextPlatform = hits[i].collider.GetComponent<MovingPlatform>();
                if (nextPlatform != null && nextPlatform != prevPlatform) {
                    size += 1;
                    prevPlatform = nextPlatform;
                    destroyThesePlatforms.Add(nextPlatform);
                    recursions += 1;
                    didNotFindAnything = false;
                    break;
                }
            }
        }
        for (int i = 0; i < destroyThesePlatforms.Count; i++) {
            Destroy(destroyThesePlatforms[i].gameObject);
        }
        destroyThesePlatforms = null;

        Transform endPoint = new GameObject("End Point").transform;
        endPoint.SetParent(movingPlatform.transform);
        endPoint.localPosition = Vector3.right * size;

        // Create the path points.
        Transform pointA = new GameObject("Point A").transform;
        pointA.position = level.GridToWorldPosition(controlPoint.gridPosition);
        Transform pointB = new GameObject("Point B").transform;
        pointB.position = level.GridToWorldPosition(beaconPoint.gridPosition);
        if (direction.x > 0) {
            pointB.position -= new Vector3(direction.x, 0f, 0f) * (size - 1);
        }

        List<Transform> points = new List<Transform>();
        points.Add(pointA);
        points.Add(pointB);

        movingPlatform.Init(endPoint, points, speed);

    }

}
