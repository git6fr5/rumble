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

    /// <summary>
    /// 
    /// <summary>
    public static class EntityExtensions {

        #region Variables.

        /* --- Constants --- */

        // The distance that we search right for another platform.
        public const float SEARCH_DISTANCE = 0.25f;

        #endregion

        // The logic of turning the ldtk data into a period offset.
        public static int GetOffset(this Entity entity, int index, List<LDtkTileData> controlData) {
            if (controlData[index].vectorID.y != 1) {
                return 0;
            }
            return controlData[index].vectorID.x;
        }

        // The logic of turning the ldtk data into a length.
        public static int GetLength<TEntity>(this Entity entity) where TEntity : Entity {
            Vector3 position = entity.transform.position;
            
            // To cache the entities we want to delete after.
            List<TEntity> garbage = new List<TEntity>();

            // Itterate right until we are no longer touching
            // a platform entity.
            int length = 0;
            bool continueSearch = true;
            while (continueSearch && length < 50) {
                length += 1;
                continueSearch = false;
                // Can I do this purely though LDtk?
                Vector3 offset = ((length - 1f) + 0.5f) * Vector3.right;
                TEntity tEntity = Game.Physics.Collisions.LineOfSight<TEntity>(position + offset, Vector2.right, Game.Physics.CollisionLayers.Platform, SEARCH_DISTANCE); 
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

        // The logic of turning the ldtk data into a path.
        public static Vector3[] GetPath(this Entity entity, int index, List<LDtkTileData> controlData, int length = 0) {
            Vector3 position = entity.transform.position;

            // Cache the direction of the path.
            Vector2 temp = Quaternion.Euler(0f, 0f, -90f * controlData[index].vectorID.x) * Vector2.up;
            Vector2Int direction = new Vector2Int((int)Mathf.Round(temp.x), (int)Mathf.Round(temp.y));
            direction.y *= -1;

            // Cache all the stop nodes in the scene.
            List<LDtkTileData> stopNodes = controlData.FindAll(data => data.vectorID == LDtkTileData.ControlStopID);

            // Itterate in the direction of the path until reaching a stop node.
            int distance = 0;
            bool continueSearch = true;
            while (continueSearch && distance < 50) {
                distance += 1;
                LDtkTileData stopNode = stopNodes.Find(node => node.gridPosition == controlData[index].gridPosition + distance * direction);
                if (stopNode != null) {
                    continueSearch = false;
                    break;
                }
            }

            // Convert the start and end nodes into world positions.
            List<Vector3> path = new List<Vector3>();
            path.Add(position);
            if (direction.x != 0f) {
                path.Add(position + (distance - length) * (Vector3)temp);
            }
            else {
                path.Add(position + (distance) * (Vector3)temp);
            }
            return path.ToArray();

        }

    }
}