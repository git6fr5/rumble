/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.Levels.LDtk;

namespace Platformer.Levels.LDtk {

    ///<summary>
    ///
    ///<summary>
    public class LDtkReader {

        // Grid Size
        public static int GridSize = 16;

        // Grid Size
        public static int GroundGridSize = 16;

        // Tile Size
        public static int TileSize = 16;

        public static List<LDtkTileData> GetLayerData(LDtkUnity.Level ldtkLevel, string layerName, int gridSize) {
            List<LDtkTileData> layerData = new List<LDtkTileData>();

            LDtkUnity.LayerInstance layer = GetLayerInstance(ldtkLevel, layerName);
            if (layer != null) { 
                for (int index = 0; index < layer.GridTiles.Length; index++) {
                    LDtkUnity.TileInstance tile = layer.GridTiles[index];
                    LDtkTileData tileData = new LDtkTileData(GetVectorID(tile), GetGridPosition(tile, gridSize), index, gridSize);
                    layerData.Add(tileData);
                }
            }
            return layerData;
        }

        public static LDtkUnity.LayerInstance GetLayerInstance(LDtkUnity.Level ldtkLevel, string layerName) {
            for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
                LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
                if (layer.IsTilesLayer && layer.Identifier == layerName) {
                    return layer;
                }
            }
            return null;
        }

        private static Vector2Int GetVectorID(LDtkUnity.TileInstance tile) {
            return new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / TileSize;
        }

        private static Vector2Int GetGridPosition(LDtkUnity.TileInstance tile, int gridSize) {
            return tile.UnityPx / gridSize;
        }

        protected static Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
            LDtkTileData tileData = data.Find(tileData => tileData != null && tileData.gridPosition == gridPosition);
            return (Vector2Int?)tileData?.vectorID;
        }

    } 

}
    