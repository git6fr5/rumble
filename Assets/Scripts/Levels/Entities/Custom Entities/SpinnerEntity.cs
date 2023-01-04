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

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Entities {

    /// <summary>
    /// An entity object readable by the level loader.
    /// That specifically inteprets control data in order to get
    /// a position within a cycle.
    /// <summary>
    public class SpinnerEntity : Entity {

        // Take the control data and turn it into a period offset.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            float spin = this.GetSpin<SpinnerEntity>(index, controlData);
            Objects.Spinners.SpinnerObject spinner = GetComponent<Objects.Spinners.SpinnerObject>();
            if (spinner != null) {
                spinner.Init(spin);
            } 
        }

        // The logic of turning the ldtk data into a length.
        public int GetSpin<TEntity>(int index, List<LDtkTileData> controlData) where TEntity : Entity {
            if (controlData[index].vectorID.y != 2) { return 0; }

            Vector3 position = transform.position;
            
            // Itterate right until we are no longer touching
            // a platform entity.
            int length = 0;
            bool continueSearch = true;
            while (continueSearch && length < 50) {
                // Can I do this purely though LDtk?
                length += 1;
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.right;
                TEntity tEntity = Game.Physics.Collisions.LineOfSight<TEntity>(position + offset, Vector2.right, Game.Physics.CollisionLayers.Platform, EntityExtensions.SEARCH_DISTANCE); 
                // Suree.... on the platform layer.
                if (tEntity != null) {
                    tEntity.transform.SetParent(transform);
                }
                continueSearch = tEntity != null ? true : false;
            }

            length = 0;
            continueSearch = true;
            while (continueSearch && length < 50) {
                // Can I do this purely though LDtk?
                length += 1;
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.left;
                TEntity tEntity = Game.Physics.Collisions.LineOfSight<TEntity>(position + offset, Vector2.left, Game.Physics.CollisionLayers.Platform, EntityExtensions.SEARCH_DISTANCE); 
                // Suree.... on the platform layer.
                if (tEntity != null) {
                    tEntity.transform.SetParent(transform);
                }
                continueSearch = tEntity != null ? true : false;
            }

            
            if (controlData[index].vectorID.x == 0f) {
                return -1;
            }
            else {
                return 1;
            }
        }

    }
}