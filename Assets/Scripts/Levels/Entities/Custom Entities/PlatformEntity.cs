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
    /// That specifically inteprets control data in order to set up
    /// a platform.
    /// <summary>
    public class PlatformEntity : Entity {

        #region Variables.

        /* --- Constants --- */

        // The distance that we search right for another platform.
        public const float SEARCH_DISTANCE = 0.25f;

        #endregion

        #region Methods.

        // Take the control data and turn it into a length and path.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            int length = PlatformEntity.GetLength(transform.position);
            Vector3[] path = PatrolEntity.GetPath(transform.position, index, controlData, length);
            // Platform platform = GetComponent<Platform>();
            // if (platform != null) {
            //     platform.Init(length, path);
            // }
        }

        // The logic of turning the ldtk data into a length.
        public static int GetLength(Vector3 position) {
            // To cache the entities we want to delete after.
            List<PlatformEntity> garbage = new List<PlatformEntity>();

            // Itterate right until we are no longer touching
            // a platform entity.
            int length = 0;
            bool continueSearch = true;
            while (continueSearch && length < 50) {
                length += 1;
                continueSearch = false;
                // Can I do this purely though LDtk?
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.right;
                PlatformEntity platformEntity = Game.Physics.Collisions.LineOfSight<PlatformEntity>(position + offset, Vector2.right, Game.Physics.CollisionLayers.Platform, SEARCH_DISTANCE); 
                if (platformEntity != null) {
                    continueSearch = true;
                    garbage.Add(platformEntity);
                }
            }

            // Delete the garabage collected.
            for (int i = 0; i < garbage.Count; i++) {
                Destroy(garbage[i].gameObject);
            }
            return length;
        }

        #endregion.
    }
}