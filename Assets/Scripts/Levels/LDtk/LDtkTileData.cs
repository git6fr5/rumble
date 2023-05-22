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
    /// A useful data structure for reading and matching LDtk data to game data.
    ///<summary>
    public class LDtkTileData {

        #region Fields.

        /* --- Constants --- */

        public static readonly Vector2Int LOADPOINT_ID = new Vector2Int(0, 0);

        public static readonly Vector2Int GROUND_ID = new Vector2Int(0, 0);

        public static readonly Vector2Int WATER_ID = new Vector2Int(1, 0);

        public const float LIGHTING_CONTROLS = 3;

        public const float WEATHER_CONTROLS = 4;

        public static Vector2Int ControlStopID = new Vector2Int(1, 0);

        public static Vector2Int ScoreOrbID = new Vector2Int(5, 3);

        /* --- Members --- */
        
        public Vector2Int vectorID { get; private set; } = new Vector2Int(0, 0);

        public Vector2Int gridPosition { get; private set; } = new Vector2Int(0, 0);

        public int index { get; private set; } = 0;

        public int gridSize { get; private set; } = 0;

        #endregion

        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index, int gridSize) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
            this.gridSize = gridSize;
        }

    }

}
    