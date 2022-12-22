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
    public class BackgroundTile : Tile {

        #region Fields

        /* --- Member Variables --- */

        // A list of the sprites in the tileset.
        [SerializeField] 
        public Sprite[] sprites;

        #endregion

        // Gets the data and sets the sprite.
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            // Guard clause.
            if (this.sprites == null || this.sprites.Length == 0) {
                return;
            }

            // Get the sprite based on the neighbour data.
            tileData.sprite = sprites[UnityEngine.Random.Range(0, this.sprites.Length)];
            tileData.colliderType = Tile.ColliderType.Grid;
        }

        #if UNITY_EDITOR
        // Creates the tile asset.
        [MenuItem("Assets/Create/2D/Custom Tiles/Background Tile")]
        public static void CreateBackgroundTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save BackgroundTile", "New BackgroundTile", "Asset", "Save BackgroundTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BackgroundTile>(), path);
        }
        #endif


    }

}
