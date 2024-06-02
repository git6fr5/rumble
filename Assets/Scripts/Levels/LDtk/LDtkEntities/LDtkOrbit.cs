// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Levels;
using Platformer.Entities;
using Platformer.Entities.Components;
using Platformer.Entities.Utility;

namespace Platformer.Levels.LDtk {

    public static class LDtkOrbit {

        public static Vector2Int FULCRUM_NODE = new Vector2Int(0, 0);
        public static Vector2Int B_NODE = new Vector2Int(1, 0);

        public static List<LDtkEntity> fulcrums = new List<LDtkEntity>();

        public static void Clear() {
            // fulcrums.CleanObjects();
            // fulcrums = new Dictionary<Vector2Int, List<LDtkEntity>>();
        }

        public static void SetOrbit(this LDtkEntity entity, List<LDtkTileData> orbitData, Vector2Int sectionOrigin) {
            LDtkTileData orbitTile = orbitData.Find(orbitTile => orbitTile.gridPosition == entity.GridPosition);
            if (orbitTile != null) {
                
                (LDtkTileData, int, Vector2Int) fulcrumData = FindFulcrum(orbitTile, orbitData);
                if (fulcrumData.Item1 != null) {

                    // Orbiting orbiter = entity.gameObject.AddComponent<Orbiting>();
                    // Transform center = new GameObject(entity.gameObject.name + " fulcrum").transform;
                    // center.SetParent(entity.transform.parent);
                    // LDtkEntity.SetPosition(center, sectionOrigin, fulcrumData.Item1.gridPosition, entity.GridSize);

                    // orbiter.SetOrbit(center, fulcrumData.Item2);
                    
                    // if (!fulcrums.ContainsKey(fulcrum.gridPosition)) {
                    //     fulcrums.Add(fulcrum.gridPosition, new List<LDtkEntity>());
                    // }
                    // fulcrums[fulcrum.gridPosition].Add(entity);

                    LDtkEntity f = fulcrums.Find(f => f.GridPosition == fulcrumData.Item1.gridPosition);
                    entity.transform.SetParent(f.transform);

                }

            }

        }

        public static void CreateFulcrums(List<LDtkTileData> orbitData, List<LDtkTileData> pathData, LevelSection levelSection) {
            fulcrums = new List<LDtkEntity>();
            
            foreach (LDtkTileData tileData in orbitData) {
                if (tileData.vectorID == FULCRUM_NODE) {

                    LDtkEntity fulcrum = new GameObject("fulcrum", typeof(Orbiting), typeof(LDtkEntity)).GetComponent<LDtkEntity>();
                    // fulcrum.GetComporevolutionDuration = 2f;

                    fulcrum.SetGridValues(tileData.gridPosition, tileData.gridSize);
                    fulcrum.SetCurrentVectorID(tileData.vectorID);
                    fulcrum.SetPosition(levelSection.WorldPosition);
                    fulcrum.transform.SetParent(levelSection.transform);
                    fulcrum.SetPath(pathData);

                    fulcrums.Add(fulcrum);


                }
            }

        }

        public static (LDtkTileData, int, Vector2Int) FindFulcrum(LDtkTileData orbitTile, List<LDtkTileData> orbitData) {

            int xOffset = 0;
            if (orbitTile.vectorID.x >= 5) {
                xOffset = 5 * (int)Mathf.Floor((float)orbitTile.vectorID.x / 5f);
            }
            Vector2Int currentPosition = orbitTile.gridPosition;
            Vector2Int gridDirection = GetGridDirection(orbitTile, xOffset);

            int distance = 0; 
            // bool continueSearch = true;
            while (distance < 50) {
                distance += 1;

                currentPosition += gridDirection;
                LDtkTileData fulcrumTile = orbitData.Find(fulcrum => fulcrum.gridPosition == currentPosition && fulcrum.vectorID == FULCRUM_NODE);

                if (fulcrumTile != null) {
                    return (fulcrumTile, distance, gridDirection);
                }

            }

            return (null, 0, Vector2Int.zero);
        }

        public static Vector2Int GetGridDirection(LDtkTileData orbitTile, int xOffset) {
            Vector2 worldDirection = Quaternion.Euler(0f, 0f, -90f * orbitTile.vectorID.x - xOffset) * Vector2.up;
            Vector2Int gridDirection = new Vector2Int((int)Mathf.Round(worldDirection.x), (int)Mathf.Round(worldDirection.y));
            gridDirection.y *= -1;
            return gridDirection;
        }

    }

}