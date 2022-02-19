using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LDtkUnity;

using LDtkTileData = LevelLoader.LDtkTileData;

public class Minimap : MonoBehaviour {

    public List<Level> discoveredLevels = new List<Level>();
    public RuleTile groundTile;
    public AnimatedTile playerTile;

    public TileBase snailShell;
    public TileBase hangingShell;
    public TileBase coin;
    public TileBase chest;
    public TileBase grass;
    public TileBase hangingGrass;

    public TileBase fireSpirit;
    public TileBase lightSpirit;

    public Tilemap tilemap;

    void Start() {



    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.W)) {
            offset += Vector3Int.up * 8;
            ShowLevels();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            offset += Vector3Int.left * 8;
            ShowLevels();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            offset += Vector3Int.down * 8;
            ShowLevels();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            offset += Vector3Int.right * 8;
            ShowLevels();
        }

    }

    public Vector3Int offset;

    public void Init() {

        Hearthbox[] hearthboxes = (Hearthbox[])GameObject.FindObjectsOfType(typeof(Hearthbox));

        discoveredLevels = new List<Level>();
        for (int i = 0; i < hearthboxes.Length; i++) {
            if (hearthboxes[i].discovered) {
                discoveredLevels.Add(hearthboxes[i].level);
            }
        }

        offset = Vector3Int.zero;
        ShowLevels();
        gameObject.SetActive(true);
    }

    private void ShowLevels() {

        tilemap.ClearAllTiles();

        // Get the json file from the LDtk Data.
        LdtkJson json = GameRules.MainLoader.lDtkData.FromJson();

        Vector3Int playerTilePosition = WorldToTilePosition(GameRules.MainPlayer.transform.position);

        for (int i = 0; i < json.Levels.Length; i++) {

            // Match the levels
            bool foundLevel = false;
            for (int j = 0; j < discoveredLevels.Count; j++) {
                if (json.Levels[i].Identifier == discoveredLevels[j].levelName) {
                    foundLevel = true;
                    break;
                }
            }
            if (foundLevel) {
                print("found level");

                int height = (int)(json.Levels[i].WorldY / json.DefaultGridSize);
                int width = (int)(json.Levels[i].WorldX / json.DefaultGridSize);


                List<LDtkTileData> groundData = LoadLayer(json.Levels[i], LevelLoader.GroundLayer, (int)json.DefaultGridSize);
                LoadTiles(tilemap, groundData, height, width, playerTilePosition + offset);

                List<LDtkTileData> decorData = LoadLayer(json.Levels[i], LevelLoader.DecorLayer, (int)json.DefaultGridSize);
                List<LDtkTileData> snailData = LoadLayer(json.Levels[i], LevelLoader.SnailLayer, (int)json.DefaultGridSize);
                List<LDtkTileData> spiritData = LoadLayer(json.Levels[i], LevelLoader.SpiritLayer, (int)json.DefaultGridSize);

                LoadTiles(tilemap, decorData, height, width, playerTilePosition + offset, new Vector2Int(0, 1), coin);
                LoadTiles(tilemap, decorData, height, width, playerTilePosition + offset, new Vector2Int(1, 1), chest);
                LoadTiles(tilemap, decorData, height, width, playerTilePosition + offset, new Vector2Int(1, 0), grass);
                LoadTiles(tilemap, decorData, height, width, playerTilePosition + offset, new Vector2Int(3, 0), hangingGrass);
                LoadTiles(tilemap, spiritData, height, width, playerTilePosition + offset, new Vector2Int(0, 0), fireSpirit);
                LoadTiles(tilemap, spiritData, height, width, playerTilePosition + offset, new Vector2Int(1, 1), lightSpirit);
                LoadTiles(tilemap, snailData, height, width, playerTilePosition + offset, new Vector2Int(0, 0), snailShell);
                LoadTiles(tilemap, snailData, height, width, playerTilePosition + offset, new Vector2Int(1, 0), hangingShell);

            }

        }

        if (-offset.x < bounds.x && -offset.x > -bounds.x && -offset.y < bounds.y && -offset.y > -bounds.y) {
            tilemap.SetTile(-offset, playerTile);
        }

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

    private static Vector2Int bounds = new Vector2Int(18 * 4, 13 * 4);

    // Set all the tiles in a tilemap.
    private void LoadTiles(Tilemap tilemap, List<LDtkTileData> data, int height, int width, Vector3Int playerTilePosition) {
        Vector3Int offset = new Vector3Int(width, height, 0) - playerTilePosition;
        for (int i = 0; i < data.Count; i++) {
            Vector3Int tilePosition = (Vector3Int)data[i].gridPosition + offset;
            print(tilePosition);
            if (tilePosition.x < bounds.x && tilePosition.x > -bounds.x && tilePosition.y < bounds.y && tilePosition.y > -bounds.y) {
                tilemap.SetTile(tilePosition, groundTile);
            }
        }
    }

    // Set all the tiles in a tilemap.
    private void LoadTiles(Tilemap tilemap, List<LDtkTileData> data, int height, int width, Vector3Int playerTilePosition, Vector2Int id, TileBase tile) {
        Vector3Int offset = new Vector3Int(width, height, 0) - playerTilePosition;
        for (int i = 0; i < data.Count; i++) {
            if (data[i].vectorID == id) {
                Vector3Int tilePosition = (Vector3Int)data[i].gridPosition + offset;
                if (tilePosition.x < bounds.x && tilePosition.x > -bounds.x && tilePosition.y < bounds.y && tilePosition.y > -bounds.y) {
                    tilemap.SetTile(tilePosition, tile);
                }
            }
        }
    }

    public Vector3Int WorldToTilePosition(Vector3 position) {
        return new Vector3Int((int)Mathf.Round(position.x), -(int)(Mathf.Round(position.y) - 1f), 0);
    }


}
