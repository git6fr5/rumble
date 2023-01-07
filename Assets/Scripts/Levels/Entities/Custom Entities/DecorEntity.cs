/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;
using Platformer.Objects.Decorations;

namespace Platformer.Levels.Entities {

    /// <summary>
    /// An entity object readable by the level loader.
    /// That specifically inteprets control data in order to get
    /// a position within a cycle.
    /// <summary>
    public class DecorEntity : RotatableEntity {

        // Take the control data and turn it into a period offset.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            Vector3[] path = this.GetPath(index, controlData, 1);
            int offset = this.GetOffset(index, controlData);
            float rotation = this.GetRotation();
            Grass grass = GetComponent<Grass>();
            if (grass != null) {
                grass.Init(offset, rotation, path);
            } 
            Arrow arrow = GetComponent<Arrow>();
            if (arrow != null) {
                arrow.Init(offset, rotation, path);
            }
        }
    }
}