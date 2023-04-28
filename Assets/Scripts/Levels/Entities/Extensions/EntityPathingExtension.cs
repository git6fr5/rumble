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

    public interface IPathable {
        void SetPath(PathingData length);
    }

    public struct PathingData {
        Vector2 Direction;
        float Distance;
    } 

    public static class EntityPathing {

        public static void SetPathing(this Entity entity, int index, List<LDtkTileData> controlData) {
            TPathable pathing = entity.GetComponent<IPathable>();
            if (pathing == null) { 
                return; 
            }
            
            PathingData path = entity.GetPath(index, controlData, length - 1);
            pathing.SetPath(path);

        }

        // The logic of turning the ldtk data into a path.
        public static PathingData GetPath(this Entity entity, int index, List<LDtkTileData> controlData, int length = 0) {
            Vector3 position = entity.transform.position;

            // Cache the direction of the path.
            Vector2 worldDirection = Quaternion.Euler(0f, 0f, -90f * controlData[index].vectorID.x) * Vector2.up;
            Vector2Int gridDirection = new Vector2Int((int)Mathf.Round(worldDirection.x), (int)Mathf.Round(worldDirection.y));
            gridDirection.y *= -1;

            // Cache all the stop nodes in the scene.
            List<LDtkTileData> stopNodes = controlData.FindAll(data => data.vectorID == LDtkTileData.ControlStopID);

            // Itterate in the direction of the path until reaching a stop node.
            int distance = 0;
            bool continueSearch = true;
            while (continueSearch && distance < 50) {
                distance += 1;
                LDtkTileData stopNode = stopNodes.Find(node => node.gridPosition == controlData[index].gridPosition + distance * gridDirection);
                if (stopNode != null) {
                    continueSearch = false;
                    break;
                }
            }

            return new PathingData(worldDirection, distance);

        }
        
    }
}