using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Platformer.Levels.LDtk;
using UnityEngine.Tilemaps;

namespace ColorSwap {

    [ExecuteInEditMode]
    public class ColorSwapManager : MonoBehaviour {

        public static ColorSwapManager Instance = null;

        void Update() {
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != null && Instance != this) {
                Debug.Log("Warning: Two color swap managers in scene.");
            }
        }

        public ColorSwappingLayer[] coloredLayers;

        public void ColorFromLDtk(LDtkEntity entity, List<LDtkTileData> colorData) {

            LDtkTileData tile = colorData.Find(tile => tile.gridPosition == entity.GridPosition);

            SwapColor swapper = entity.GetComponent<SwapColor>(); // eee woops
            if (swapper) {
               swapper.Set(tile);
               return;
            }

            if (tile == null) {
                SetColor(entity.transform, coloredLayers[0].color);
                SetLayer(entity.transform, coloredLayers[0].layer);
            }
            else {
                if (tile.vectorID.x < coloredLayers.Length) {
                    SetColor(entity.transform, coloredLayers[tile.vectorID.x].color);
                    SetLayer(entity.transform, coloredLayers[tile.vectorID.x].layer);
                }
            }
        }

        public void SwapColor(Transform transform, int index, bool swapColor = true, bool swapLayer = true) {
            if (index < coloredLayers.Length) {
                if (swapColor) { SetColor(transform, coloredLayers[index].color); }
                if (swapLayer) { SetLayer(transform, coloredLayers[index].layer); }
            }
        }

        public void SwapColorInGame(Transform transform, int index, bool onlyColor = false) {
            if (!Application.isPlaying) { return; }
            if (index < coloredLayers.Length) {
                SetColor(transform, coloredLayers[index].color);
                SetLayer(transform, coloredLayers[index].layer);
            }
        }

        public void SwapColorInGameByLayer(Transform transform, int layer, int offset = 0, bool onlyColor = false) {
            if (!Application.isPlaying) { return; }
            for (int i = 0; i < coloredLayers.Length; i++) {
                if (layer == coloredLayers[i].layer) {
                    int n = (i + offset) % coloredLayers.Length;
                    SetColor(transform, coloredLayers[n].color);
                    SetLayer(transform, coloredLayers[n].layer);
                    return;
                }
            }
        }

        public static void SetColor(Transform transform, Color color) {
            recursive_Color(transform, color);
        }

        private static void recursive_Color(Transform transform, Color color) {
            if (transform.gameObject.layer == LayerMask.NameToLayer("Water")) { return; }

            SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer) {
                spriteRenderer.color = color;
            }

            SpriteShapeRenderer spriteShapeRenderer = transform.GetComponent<SpriteShapeRenderer>();
            if (spriteShapeRenderer) {
                spriteShapeRenderer.color = color;
            }

            Tilemap tilemapRenderer = transform.GetComponent<Tilemap>();
            if (tilemapRenderer) {
                tilemapRenderer.color = color;
            }

            foreach (Transform child in transform) {
                recursive_Color(child, color);
            }
        }

         public static void SetLayer(Transform transform, int layer) {
            recursive_Layer(transform, layer);
        }

        private static void recursive_Layer(Transform transform, int layer) {
            if (transform.gameObject.layer == LayerMask.NameToLayer("Water")) { return; }
            transform.gameObject.layer = layer;
            foreach (Transform child in transform) {
                recursive_Layer(child, layer);
            }
        }

    }

}