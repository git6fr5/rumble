/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
// Platformer.
using Platformer.Levels;

namespace Platformer.Levels.LDtk {

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
    public class LDtkTilemapManager : MonoBehaviour {

        [SerializeField]
        private Grid m_Grid;

        [SerializeField]
        private Tilemap m_DecorationMap;

        [SerializeField]
        private Tilemap m_CollisionMap;

        [SerializeField] 
        private List<LDtkTileEntity> m_DecorationTiles = new List<LDtkTileEntity>();

        [SerializeField]
        private TileBase m_CollisionTile;

        // Loads the map layouts for all the given levels.
        public void Refresh(List<LevelSection> sections, string layerName) {
            m_DecorationMap.ClearAllTiles();
            for (int i = 0; i < sections.Count; i++) {
                GenerateDecorationSection(sections[i], layerName);
            }
            RefreshCollisionMap();
        }

        public void RefreshCollisionMap() {
            if (m_DecorationMap == null || m_CollisionMap == null) { return; }

            m_CollisionMap.ClearAllTiles();
            foreach (Vector3Int pos in m_DecorationMap.cellBounds.allPositionsWithin) {
                if (m_DecorationMap.HasTile(pos)) {
                    m_CollisionMap.SetTile(pos, m_CollisionTile);
                }
            }
        }

        public void GenerateDecorationSection(LevelSection section, string layerName) {
            List<LDtkTileData> tileData = LDtkReader.GetLayerData(section.ldtkLevel, layerName);

            for (int i = 0; i < tileData.Count; i++) {
                TileBase tile = m_DecorationTiles.Find(tileEnt => tileEnt.vectorID == tileData[i].vectorID)?.tile;
                Vector3Int tilePosition = section.GridToTilePosition(tileData[i].gridPosition);
                m_DecorationMap.SetTile(tilePosition, tile);
            }
            m_DecorationMap.RefreshAllTiles();
        }
        
    }
    
}