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
    /// That specifically inteprets control data in order to get
    /// a position within a cycle.
    /// <summary>
    public class SpitterEntity : RotatableEntity {

        // Take the control data and turn it into a period offset.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            Vector3[] path = this.GetPath(index, controlData, 1);
            int offset = this.GetOffset(index, controlData);
            float rotation = this.GetRotation();
            Objects.Spitters.SpitterObject spitter = GetComponent<Objects.Spitters.SpitterObject>();
            if (spitter != null) {
                spitter.Init(offset, rotation, path);
            } 
        }

    }
}