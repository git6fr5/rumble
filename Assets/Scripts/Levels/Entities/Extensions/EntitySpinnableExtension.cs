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

        // Take the control data and turn it into a period offset.
        public static void SetSpin(this Entity entity) {

            TSpinnable spinnable = entity.GetComponent<ISpinnable>();
            if (spinnable == null) {
                return;
            }
            
            float spin = GetSpin(index, controlData);
            spinnable.SetSpin(spin);

        }

        // The logic of turning the ldtk data into a length.
        public int GetSpin(int index, List<LDtkTileData> controlData) {
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
                ISpinnable tEntity = Game.Physics.Collisions.LineOfSight<ISpinnable>(position + offset, Vector2.right, Game.Physics.CollisionLayers.Platform, EntityExtensions.SEARCH_DISTANCE); 
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
                ISpinnable tEntity = Game.Physics.Collisions.LineOfSight<ISpinnable>(position + offset, Vector2.left, Game.Physics.CollisionLayers.Platform, EntityExtensions.SEARCH_DISTANCE); 
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