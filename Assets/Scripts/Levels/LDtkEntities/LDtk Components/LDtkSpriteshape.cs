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

    public static class LDtkSpriteshape {

        public static void SetLength(this LDtkEntity entity, List<LDtkTileData> entityData) {
            SpriteShapeController spriteShapeController = entity.GetComponentInChildren<SpriteShapeController>();
            if (spriteShapeController != null) {
                int length = GetLength(entity, entityData);
                bool valid = SetSpriteshapePoints(length, spriteShapeController);
                entity.gameObject.SetActive(valid);
            }
        }

        public static bool SetSpriteshapePoints(int length, SpriteShapeController spriteShapeController) {

            Spline spline = spriteShapeController.spline;

            // In the special case that the length of this is 0 or less.
            if (length <= 2) {
                // Invoke("DontNeedThisOne", 0f);
                return false;
            }

            length -= 2;
            spline.Clear();
            spline.InsertPointAt(0, 0.5f * Vector3.right);
            spline.InsertPointAt(1, (length + 0.5f) * Vector3.right);
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
            return true;

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