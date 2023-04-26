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
        public override void SetRotation() {
            float rotation = this.GetRotation();
            DecorationController decor = GetComponent<DecorationController>();
            if (decor != null) {
                decor.SetRotation(rotation);
            }
            Grass grass = GetComponent<Grass>();
            if (grass != null) {
                grass.SetRotation(rotation);
            } 
            Arrow arrow = GetComponent<Arrow>();
            if (arrow != null) {
                arrow.SetRotation(rotation);
            }
        }
    }
}