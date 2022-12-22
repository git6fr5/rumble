/* --- Libraries --- */
// System.
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
// Platformer.
using Platformer.Levels.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platformer.Levels.Tilemaps {

    /// <summary>
    /// A custom tile that defines an automatically tiling
    /// ground tile.
    /// <summary>
    [Serializable]
    public class GroundTile : Tile {

        #region Fields

        /* --- Member Variables --- */

        // A list of the sprites in the tileset.
        [SerializeField] 
        public Sprite[] sprites;

        // A reference to the Ground Tile mapping for easy reference.
        private Dictionary<int, int> mapping => CustomTileMappings.GroundTileMapping;

        #endregion

        // Gets the data and sets the sprite.
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            // Guard clause.
            if (this.mapping == null || this.sprites == null || this.sprites.Length == 0) {
                return;
            }

            // Get the sprite based on the neighbour data.
            int neighbours = new NeighbourTileArray(position, tilemap).binaryValue;
            int index = this.mapping.ContainsKey(neighbours) ? this.mapping[neighbours] : 0;
            tileData.sprite = sprites[index];
            tileData.colliderType = Tile.ColliderType.Grid;
        }

        #if UNITY_EDITOR
        // Creates the tile asset.
        [MenuItem("Assets/Create/2D/Custom Tiles/GroundTile")]
        public static void CreateGroundTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save GroundTile", "New GroundTile", "Asset", "Save GroundTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GroundTile>(), path);
        }
        #endif


    }

}
