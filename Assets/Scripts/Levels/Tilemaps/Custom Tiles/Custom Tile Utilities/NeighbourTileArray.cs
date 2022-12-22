/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
// Platformer.
using Platformer.Levels.Tilemaps;

namespace Platformer.Levels.Tilemaps {

    ///<summary>
    /// Create a array of boolean integers that
    /// tracks whether the neighbouring spaces of a tile
    /// at a given position are filled.
    ///<summary>
    public class NeighbourTileArray {

        #region Fields.

        // Stores the neigbouring tile data.
        private int[] bits;

        // Converts the bit array into a base 2 ID.
        public int binaryValue => GetBinaryValue();

        #endregion

        #region Methods.

        // Constructs the bit array.
        public NeighbourTileArray(Vector3Int position, ITilemap tilemap) {
            // Initializing the bit array.
            this.bits = new int[8];
            // Cardinal tiles.
            this.bits[1] = tilemap.GetTile(position + Vector3Int.up) != null ? 1 : 0;
            this.bits[3] = tilemap.GetTile(position + Vector3Int.left) != null ? 1 : 0;
            this.bits[6] = tilemap.GetTile(position + Vector3Int.down) != null ? 1 : 0;
            this.bits[4] = tilemap.GetTile(position + Vector3Int.right) != null ? 1 : 0;
            // Diagonal tiles.
            this.bits[2] = tilemap.GetTile(position + Vector3Int.right + Vector3Int.up) != null ? 1 : 0;
            this.bits[0] = tilemap.GetTile(position + Vector3Int.left + Vector3Int.up) != null ? 1 : 0;
            this.bits[5] = tilemap.GetTile(position + Vector3Int.left + Vector3Int.down) != null ? 1 : 0;
            this.bits[7] = tilemap.GetTile(position + Vector3Int.right + Vector3Int.down) != null ? 1 : 0;

        }

        private int GetBinaryValue() {
            int val = 0;
            for (int i = 0; i < bits.Length; i++) {
                val += (int)Mathf.Pow(2, i) * bits[i];
            }
            return val;
        }

        #endregion

    }

}