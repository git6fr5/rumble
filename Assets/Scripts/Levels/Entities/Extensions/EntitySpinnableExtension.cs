/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Entities {

    public interface ISpinnable {
        void SetSpin(int direction);
    }

    public static class EntitySpinnableExtensions {

        // The distance that we search right for another platform.
        public const float SEARCH_DISTANCE = 0.25f;

        // Take the control data and turn it into a period offset.
        public static void SetSpin(this Entity entity, int index, List<LDtkTileData> controlData) {

            ISpinnable spinnable = entity.GetComponent<ISpinnable>();
            if (spinnable == null) {
                return;
            }
            
            int spin = entity.GetSpin(index, controlData);
            spinnable.SetSpin(spin);

        }

        // The logic of turning the ldtk data into a length.
        public static int GetSpin(this Entity entity, int index, List<LDtkTileData> controlData) {
            if (controlData[index].vectorID.y != 2) { return 0; }

            Vector3 position = entity.transform.position;
            
            // Itterate right until we are no longer touching
            // a platform entity.
            int length = 0;
            bool continueSearch = true;
            while (continueSearch && length < 50) {
                // Can I do this purely though LDtk?
                length += 1;
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.right;
                GameObject spinningObject = Game.Physics.Collisions.ILineOfSight<ISpinnable>(position + offset, Vector2.right, Game.Physics.CollisionLayers.Ground, SEARCH_DISTANCE); 
                // Suree.... on the platform layer.
                if (spinningObject != null) {
                    spinningObject.transform.SetParent(entity.transform);
                }
                continueSearch = spinningObject != null ? true : false;
            }

            length = 0;
            continueSearch = true;
            while (continueSearch && length < 50) {
                // Can I do this purely though LDtk?
                length += 1;
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.left;
                GameObject spinningObject = Game.Physics.Collisions.ILineOfSight<ISpinnable>(position + offset, Vector2.left, Game.Physics.CollisionLayers.Ground, SEARCH_DISTANCE); 
                // Suree.... on the platform layer.
                if (spinningObject != null) {
                    spinningObject.transform.SetParent(entity.transform);
                }
                continueSearch = spinningObject != null ? true : false;
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