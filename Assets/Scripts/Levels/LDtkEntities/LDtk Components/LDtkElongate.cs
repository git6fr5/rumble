// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Levels;
using Platformer.Entities.Components;
using Platformer.Entities.Utility;

namespace Platformer.Levels {

    public static class LDtkElongate {

        public static void SetLength(this LDtkEntity entity, List<LDtkTileData> entityData) {
            Elongatable elongatable = entity.GetComponent<Elongatable>();
            if (elongatable != null) {
                int length = GetLength(entity, entityData);
                elongatable.SetLength(length);
            }
        }

        public static int GetLength(LDtkEntity entity, List<LDtkTileData> entityData) {

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