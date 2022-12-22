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
    /// Organizes mappings for custom tiles to grab an index from a spritesheet.
    ///<summary>
    public class CustomTileMappings {

        #region Fields.

        /* --- Constants --- */

        // An easy reference for a list of diagonals.
        private static List<int> DIAGONALS = new List<int>() { 1, 4, 32, 128 };

        /* --- Static --- */

        // The mapping for the custom ground tile.
        public static Dictionary<int, int> GroundTileMapping { get; private set; } = new Dictionary<int, int>();

        // The mapping for the custom water tile.
        public static Dictionary<int, int> WaterTileMapping { get; private set; } = new Dictionary<int, int>();

        #endregion

        #region Methods.
        
        // Creates the mapping for the custom ground tile.
        public static void CreateGroundTileMapping() {
            GroundTileMapping = new Dictionary<int, int>();

            // Floating Block
            GroundTileMapping.Add(0, 0);

            // 9x9
            // Upper Row.
            GroundTileMapping.AddAllKeyCombinations(16 + 64 + 128, new List<int>{ 1, 4, 32 }, 1); // Left + Up
            GroundTileMapping.AddAllKeyCombinations(8 + 16 + 32 + 64 + 128, new List<int>{ 1, 4 }, 2); // Up
            GroundTileMapping.AddAllKeyCombinations(8 + 32 + 64, new List<int>{ 1, 4, 128 }, 3); // Right + up
            // Center Row.
            GroundTileMapping.AddAllKeyCombinations(2 + 4 + 16 + 64 + 128, new List<int>{ 1, 32 }, 4); // Left
            GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 32 + 64 + 128, 5); // Center
            GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8 + 32 + 64, new List<int>{ 4, 128 }, 6); // Right
            // Lower Row.
            GroundTileMapping.AddAllKeyCombinations(2 + 4 + 16, new List<int>{ 1, 32, 128 }, 7); // Down + Left
            GroundTileMapping.AddAllKeyCombinations(1 + 2 + 4 + 8 + 16, new List<int>{ 32, 128}, 8); // Down
            GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8, new List<int>{ 4, 32, 128 }, 9); // Down + Right

            // Pillars
            GroundTileMapping.AddAllKeyCombinations(64, DIAGONALS, 10); // Pillar Up
            GroundTileMapping.AddAllKeyCombinations(2 + 64, DIAGONALS, 11); // Pillar Center
            GroundTileMapping.AddAllKeyCombinations(2, DIAGONALS, 12); // Pillar Down

            // Platforms
            GroundTileMapping.AddAllKeyCombinations(16, DIAGONALS, 13);
            GroundTileMapping.AddAllKeyCombinations(8 + 16, DIAGONALS, 14);
            GroundTileMapping.AddAllKeyCombinations(8, DIAGONALS, 15);

            // 1 Corner, No Borders
            GroundTileMapping.Add(1 + 2 + 8 + 16 + 32 + 64 + 128, 16);
            GroundTileMapping.Add(2 + 4 + 8 + 16 + 32 + 64 + 128, 17);
            GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 32 + 64, 18);
            GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 64 + 128, 19);

            // 1 Corner, 1 Vertial Border
            GroundTileMapping.AddAllKeyCombinations(2 + 16 + 64 + 128, new List<int>{ 32 }, 20);
            GroundTileMapping.AddAllKeyCombinations(2 + 8 + 32 + 64, new List<int>{ 128 }, 21);
            GroundTileMapping.AddAllKeyCombinations(2 + 4 + 16 + 64, new List<int>{ 1 }, 22);
            GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8 + 64, new List<int>{ 4 }, 23);

            // 1 Corner, 2 Border
            GroundTileMapping.AddAllKeyCombinations(2 + 16, new List<int>{ 32, 128 }, 24);
            GroundTileMapping.AddAllKeyCombinations(2 + 8, new List<int>{ 32, 128 }, 25);
            GroundTileMapping.AddAllKeyCombinations(64 + 16, new List<int>{ 1, 4 }, 26);
            GroundTileMapping.AddAllKeyCombinations(64 + 8, new List<int>{ 1, 4 }, 27);

            // 2 Corner, 1 Border
            GroundTileMapping.AddAllKeyCombinations(2 + 16 + 64, new List<int>{ 1, 32 }, 28);
            GroundTileMapping.AddAllKeyCombinations(2 + 8 + 64, new List<int>{ 4, 128 }, 29);
            GroundTileMapping.AddAllKeyCombinations(8 + 16 + 64, new List<int>{ 1, 4 }, 30);
            GroundTileMapping.AddAllKeyCombinations(2 + 8 + 16, new List<int>{ 32, 128 }, 31);

            // 2 Corner, No Border
            GroundTileMapping.Add(2 + 8 + 16 + 32 + 64 + 128, 32);
            GroundTileMapping.Add(2 + 4 + 8 + 16 + 64 + 128, 33);
            GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 64, 34);
            GroundTileMapping.Add(1 + 2 + 8 + 16 + 32 + 64, 35);

            // 1 Corner, 1 Horizontal Border
            GroundTileMapping.AddAllKeyCombinations(8 + 16 + 64 + 128, new List<int>{ 1, 4 }, 36);
            GroundTileMapping.AddAllKeyCombinations(8 + 16 + 32 + 64, new List<int>{ 1, 4 }, 37);
            GroundTileMapping.AddAllKeyCombinations(2 + 4 + 8 + 16, new List<int>{ 32, 128 }, 38);
            GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8 + 16, new List<int>{ 32, 128 }, 39);

            // 3 Corners
            GroundTileMapping.Add(2 + 8 + 16 + 64 + 128, 40);
            GroundTileMapping.Add(2 + 8 + 16 + 32 + 64, 41);
            GroundTileMapping.Add(2 + 4 + 8 + 16 + 64, 42);
            GroundTileMapping.Add(1 + 2 + 8 + 16 + 64, 43);

            // 2 Corners Adjacent
            GroundTileMapping.Add(1 + 2 + 8 + 16 + 64 + 128, 44);
            GroundTileMapping.Add(2 + 4 + 8 + 16 + 32 + 64, 45);

            // All 4 Corners.
            GroundTileMapping.Add(2 + 8 + 16 + 64, 48);

        }

        // Creates the mapping for the custom water tile.
        public static void CreateWaterTileMapping() {
            WaterTileMapping = new Dictionary<int, int>();
            WaterTileMapping.AddAllKeyCombinations(64, new List<int>{ 1, 4, 8, 16, 32, 128 }, 0); // Surface
            WaterTileMapping.AddAllKeyCombinations(2 + 64, new List<int>{ 1, 4, 8, 16, 32, 128 }, 1); // Center Column
            WaterTileMapping.AddAllKeyCombinations(2, new List<int>{ 1, 4, 8, 16, 32, 128 }, 2); // Bottom
            WaterTileMapping.AddAllKeyCombinations(0, new List<int>{ 8, 16 }, 3); // Bottom
        }

        #endregion

    }


}
