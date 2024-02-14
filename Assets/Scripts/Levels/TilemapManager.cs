/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
// Platformer.
using Platformer.Levels.Tilemaps;

namespace Platformer.Levels.Tilemaps {

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(UnityEngine.Grid))]
    public class TilemapManager : MonoBehaviour {

        [SerializeField]
        private Tilemap m_ForegroundMap;
        public TileBase m_ToTile;

        [SerializeField]
        private Tilemap m_Grid;
        public TileBase m_BlockTile;

        [Header("Controls")]
        public bool m_Refresh = false;
        public bool m_Replace = false;

        void OnEnable() {
            m_Refresh = false;
        }

        void Update() {
            if (!Application.isPlaying) {

                if (m_Refresh) {
                    LoadGrid(m_ForegroundMap, m_Grid, m_BlockTile);
                    m_Refresh = false;
                }

                if (m_Replace) {
                    ReplaceAll(m_ToTile, m_ForegroundMap);
                    m_Replace = false;
                }

            }
        }


        public static void LoadGrid(Tilemap map, Tilemap grid, TileBase blockTile) {
            if (map == null || grid == null) { return; }

            grid.ClearAllTiles();
            foreach (Vector3Int pos in map.cellBounds.allPositionsWithin) {
                if (map.HasTile(pos)) {
                    grid.SetTile(pos, blockTile);
                }
            }
        }

        public static void ReplaceAll(TileBase toTile, Tilemap map) {
            if (toTile == null || map == null) { return; }

            foreach (Vector3Int pos in map.cellBounds.allPositionsWithin) {
                if (map.HasTile(pos)) {
                    map.SetTile(pos, toTile);
                }
            }
        }

    }
    
}