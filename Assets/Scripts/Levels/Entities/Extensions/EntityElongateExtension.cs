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

    public interface IElongatable {
        void SetLength(int length);
    }

    public static class EntityElongateExtension {

        #region Variables.

        /* --- Constants --- */

        // The distance that we search right for another platform.
        public const float SEARCH_DISTANCE = 0.25f;

        #endregion

        public static void SetLength<TElongatable>(this Entity entity, int index, List<LDtkTileData> controlData) where TElongatable : IElongatable {
            TElongatable elongatable = entity.GetComponent<TElongatable>();
            if (elongatable == null) { 
                return; 
            }
            
            int length = entity.GetLength<TEl();
            elongatable.SetLength(length);

        }

        // The logic of turning the ldtk data into a length.
        public static int GetLength<TElongatable>(this Entity entity) where TElongatable : IElongatable {
            Vector3 position = entity.transform.position;
            
            // To cache the entities we want to delete after.
            List<TElongatable> garbage = new List<IElongatable>();

            // Itterate right until we are no longer touching
            // a platform entity.
            int length = 0;
            bool continueSearch = true;
            while (continueSearch && length < 50) {
                length += 1;
                continueSearch = false;
                // Can I do this purely though LDtk?
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.right;
                TElongatable tEntity = Game.Physics.Collisions.LineOfSight<TElongate>(position + offset, Vector2.right, Game.Physics.CollisionLayers.Platform, SEARCH_DISTANCE); 
                if (tEntity != null) {
                    continueSearch = true;
                    garbage.Add(tEntity);
                }
            }

            // Delete the garabage collected.
            for (int i = 0; i < garbage.Count; i++) {
                MonoBehaviour.Destroy(garbage[i].gameObject);
            }
            Debug.Log(length);
            return length;
        }

    }

}