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
    /// water tile.
    /// <summary>
    [Serializable]
    public class WaterTile : Tile {

        #region Fields.

        /* --- Constants --- */

        // The opacity of the water.
        private const float OPACITY = 0.75f;

        /* --- Member Variables --- */

        // A list of the animations in the tileset.
        [SerializeField] 
        public TileAnimation[] animations;

        // A reference to the Water Tile mapping for easy reference.
        private Dictionary<int, int> mapping => CustomTileMappings.WaterTileMapping;

        #endregion

        #region Methods.

        // Gets the data and sets the sprite.
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            int neighbours = new NeighbourTileArray(position, tilemap).binaryValue;

            // If there is an index this is mapped to then animate the tile.
            if (this.mapping != null && this.mapping.ContainsKey(neighbours)) {
                int animationIndex = this.mapping[neighbours];
                tileData.sprite = animations[animationIndex].currentFrame;
            }
            else if (this.animations != null && this.animations.Length > 0 && this.animations[0].sprites.Length > 0) {
                tileData.sprite = animations[0].sprites[0];
            }
            else {
                tileData.sprite = null;
            }

            // The opacity and collision type.
            tileData.color = new Color(1f, 1f, 1f, OPACITY);
            tileData.colliderType = Tile.ColliderType.Grid;

        }

        #if UNITY_EDITOR
        // Creates the tile asset.
        [MenuItem("Assets/Create/2D/Custom Tiles/WaterTile")]
        public static void CreateWaterTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save WaterTile", "New WaterTile", "Asset", "Save WaterTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WaterTile>(), path);
        }
        #endif

        #endregion

    }

}

