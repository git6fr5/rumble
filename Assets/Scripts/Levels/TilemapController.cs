/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
// Gobblefish.
using Gobblefish.Extensions;
// Platformer.
using Platformer.Levels;

namespace Platformer.Levels {

    /// <summary>
    /// A tile entity used to match tiles to the ldtk file.
    /// </summary>
    [System.Serializable]
    public class LDtkTileEntity {
        public TileBase tile;
        public Vector2Int vectorID;
    }

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(UnityEngine.Grid))]
    public class TilemapController : MonoBehaviour {

        [SerializeField]
        private Grid m_Grid;
        public Grid Grid => m_Grid;

        [SerializeField]
        private Tilemap m_LevelMap;

        [SerializeField]
        private Tilemap m_CollisionMap;

        [SerializeField] 
        private List<LDtkTileEntity> m_LevelTiles = new List<LDtkTileEntity>();

        [SerializeField]
        private TileBase m_CollisionTile;

        [SerializeField]
        private GameObject m_CollisionBlock;

        // Loads the map layouts for all the given levels.
        public void RefreshLevel(List<LevelSection> sections, string layerName) {
            m_LevelMap.ClearAllTiles();
            for (int i = 0; i < sections.Count; i++) {
                List<LDtkTileData> tileData = LDtkReader.GetLayerData(sections[i].ldtkLevel, layerName);
                GenerateLevelSection(m_LevelMap, sections[i], m_LevelTiles, tileData);
            }
            RefreshGrid(m_LevelMap, m_CollisionMap, m_CollisionTile);
        }


        public static void RefreshGrid(Tilemap levelMap, Tilemap gridMap, TileBase gridTile) {
            if (levelMap == null || gridMap == null) { return; }

            gridMap.ClearAllTiles();
            foreach (Vector3Int pos in levelMap.cellBounds.allPositionsWithin) {
                if (levelMap.HasTile(pos)) {
                    gridMap.SetTile(pos, gridTile);
                }
            }
        }

        public static void GenerateLevelSection(Tilemap levelMap, LevelSection levelSection, List<LDtkTileEntity> levelTiles, List<LDtkTileData> tileData) {
            for (int i = 0; i < tileData.Count; i++) {
                TileBase tile = levelTiles.Find(tileEnt => tileEnt.vectorID == tileData[i].vectorID)?.tile;
                Vector3Int tilePosition = levelSection.GridToTilePosition(tileData[i].gridPosition);
                levelMap.SetTile(tilePosition, tile);
            }
            levelMap.RefreshAllTiles();
        }

        public void ConvertToBlocks(BoundsInt bounds) {

            for (int i = bounds.yMin; i <= bounds.yMax; i++) {
                for (int j = bounds.xMin; j <= bounds.xMax; j++) {
                    Vector3Int position = new Vector3Int(j, i, 0);
                    Sprite sprite = m_LevelMap.GetSprite(position);
                    if (sprite != null) {
                        CreateBlock(sprite, position);
                    }
                }
            }

            for (int i = bounds.yMin; i <= bounds.yMax; i++) {
                for (int j = bounds.xMin; j <= bounds.xMax; j++) {
                    Vector3Int pos = new Vector3Int(j, i, 0);
                    m_LevelMap.SetTile(pos, null);
                    m_CollisionMap.SetTile(pos, null);
                }
            }

        }

        public void CreateBlock(Sprite sprite, Vector3Int position) {
            // Create the block.
            GameObject newObject = Instantiate(m_CollisionBlock);
            newObject.name = "Block " + position.ToString();
            newObject.transform.position = m_Grid.GetCellCenterWorld(position);
            newObject.GetComponent<SpriteRenderer>().sprite = sprite;
            // Set the transform.
            Matrix4x4 matrix = m_LevelMap.GetTransformMatrix(position);
            newObject.transform.FromMatrix(matrix);
            // Set the block active.
            newObject.SetActive(true);
        }

        
    }
    
}