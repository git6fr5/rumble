using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Stores specific data on how to generate the level.
/// </summary>
public class Environment : MonoBehaviour {

    /* --- Data Structures --- */
    public class FloorTile : TileBase {

        Sprite[] sprites;
        public void Init(Sprite[] sprites) {
            this.sprites = sprites;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.colliderType = Tile.ColliderType.Sprite;
            tileData.sprite = this.sprites[0];
        }

    }

    [System.Serializable]
    public class ColorScheme {

        public Color particleBirthColor;
        public Color particleMidColor;
        public Color particleDeathColor;

    }

    /* --- Components --- */
    // Entities.
    [SerializeField] public Transform animalParentTransform; // The location to look for the entities.
    [SerializeField] public Transform obstacleParentTransform; // The location to look for the entities.
    // Tiles.
    [SerializeField] public RuleTile floorTile; // A set of sprites used to tile the floor of the level.

    /* --- Parameters --- */
    [SerializeField] public ColorScheme colorScheme;

    /* --- Properties --- */
    // [SerializeField, ReadOnly] public RuleTile floorTile; // The set of floor tiles generated from the floor sprites.
    [SerializeField, ReadOnly] public List<Entity> animals; // The set of entities found from the parent transform.
    [SerializeField, ReadOnly] public List<Entity> obstacles; // The set of entities found from the parent transform.

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        RefreshTiles();
        RefreshEntities();
    }

    /* --- Tile Methods --- */
    public void RefreshTiles() {
        // floorTile = (FloorTile)ScriptableObject.CreateInstance(typeof(FloorTile));
        // floorTile.Init(floorSprites);
        // floorTile = floorSprites;
    }

    /* --- Entity Methods --- */
    // Refreshes the set of entities.
    void RefreshEntities() {
        animals = new List<Entity>();
        obstacles = new List<Entity>();
        foreach (Transform child in animalParentTransform) {
            FindAllEntitiesInTransform(child, ref animals);
        }
        foreach (Transform child in obstacleParentTransform) {
            FindAllEntitiesInTransform(child, ref obstacles);
        }
    }

    // Recursively searches through the transform for all entity components.
    void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {

        // If we've found an entity, don't go any deeper.
        if (parent.GetComponent<Entity>() != null) {
            entityList.Add(parent.GetComponent<Entity>());
        }
        else if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child, ref entityList);
            }
        }
    }

    // Returns the first found entity with a matching ID.
    public Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
        for (int i = 0; i < entityList.Count; i++) {
            if (entityList[i].vectorID == vectorID) {
                return entityList[i];
            }
        }
        return null;
    }

}
