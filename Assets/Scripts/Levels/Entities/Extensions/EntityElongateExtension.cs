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

        public static void SetLength(this Entity entity, List<LDtkTileData> entityData) {
            IElongatable elongatable = entity.GetComponent<IElongatable>();
            if (elongatable == null) { 
                return; 
            }
            
            int length = entity.GetLength(entityData);
            elongatable.SetLength(length);

        }

        public static int GetLength(this Entity entity, List<LDtkTileData> entityData) {

            LDtkTileData left = entityData.Find(tile => tile.gridPosition == entity.GridPosition + Vector2Int.left && tile.vectorID == entity.VectorID);
            if (left != null) {
                return -1;
            }

            int length = 0;
            while (length < 50) {
                length += 1;
                LDtkTileData right = entityData.Find(tile => tile.gridPosition == entity.GridPosition + length * Vector2Int.right && tile.vectorID == entity.VectorID);
                if (right == null) {
                    break;
                }
            }

            return length;

        }

    }

}