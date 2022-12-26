/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;
using Platformer.Objects;

namespace Platformer.Levels.Entities {

    /// <summary>
    /// An entity object readable by the level loader.
    /// That specifically inteprets control data in order to set up
    /// a platform.
    /// <summary>
    public class PlatformEntity : Entity {

        #region Methods.

        // Take the control data and turn it into a length and path.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            int length = this.GetLength<PlatformEntity>();
            Vector3[] path = this.GetPath(index, controlData, length);
            Objects.Platforms.PlatformObject platform = GetComponent<Objects.Platforms.PlatformObject>();
            if (platform != null) {
                platform.Init(length, path);
            }
        }

        #endregion.
    }
}