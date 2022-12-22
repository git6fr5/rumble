/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.Tilemaps;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Tilemaps {

    ///<summary>
    /// A simple array of sprites for animating custom tiles.
    /// Yes. This is equivalent to using a 2D array.
    /// But doing it like this is just easier to read.
    ///<summary>
    [System.Serializable]
    public class TileAnimation {

        // The rate at which this animation plays.
        protected const float FRAME_RATE = 4;

        // The sprites used for animating this tile.
        [field: SerializeField] 
        public Sprite[] sprites { get; private set; }

        // Returns the current frame of this animation.
        public Sprite currentFrame => GetFrame(Game.Physics.Time.Ticks);

        // Gets the frame based on the given ticks.
        public Sprite GetFrame(float ticks) {
            int frame = (int)Mathf.Floor(ticks * FRAME_RATE) % sprites.Length;
            return sprites[frame];
        }

    }

}