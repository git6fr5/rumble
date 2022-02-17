using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour {

    public Environment environment;

    public Tilemap floorMap;

    public int height;
    public int width;
    public int gridSize;

    public int worldHeight;
    public int worldWidth;

    public List<Entity> animals;
    public List<Entity> obstacles;

    public List<Vector3Int> nonEmptyTiles;

    public Vector2Int gridOrigin => new Vector2Int(worldWidth, worldHeight);
    public Vector2Int connectPosition;
    public Vector2Int controlPosition;

    public Vector3 GridToWorldPosition(Vector2Int gridPosition) {
        return new Vector3((gridPosition.x + gridOrigin.x) + 0.5f, - (gridPosition.y + gridOrigin.y) + 0.5f, 0f);
    }

    public Vector3Int GridToTilePosition(Vector2Int gridPosition) {
        return new Vector3Int(gridPosition.x + gridOrigin.x, -(gridPosition.y + gridOrigin.y), 0);
    }

    void OnDrawGizmos() {

        Gizmos.color = Color.blue;
        Vector3 startTilePosition = (Vector3)GridToTilePosition(gridOrigin);
        Gizmos.DrawWireSphere(startTilePosition, 1f);

        Gizmos.color = Color.red;
        Vector3 endTilePosition = (Vector3)GridToTilePosition(connectPosition + gridOrigin);
        Gizmos.DrawWireSphere(endTilePosition, 0.75f);
    }

}
