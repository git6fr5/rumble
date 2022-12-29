/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Tilemaps;

namespace Platformer.Levels.Tilemaps {

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Grid))]
    public class TilemapManager : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // The ground map.
        public Tilemap groundMap { get; private set; }

        // The ground map mask.
        public Tilemap groundMaskMap { get; private set; }

        // The water map.
        public Tilemap waterMap { get; private set; }

        // The background map.
        public Tilemap backgroundMap { get; private set; }

        // The tiles for the ground map.
        [field: SerializeField] 
        public GroundTile groundTile { get; private set; }

        // The tiles for the ground mask.
        [field: SerializeField] 
        public GroundTile maskTile { get; private set; }

        // The tiles for the ground water.
        [field: SerializeField] 
        public WaterTile waterTile { get; private set; }

        // The tiles for the background.
        [field: SerializeField] 
        public BackgroundTile backgroundTile { get; private set; }

        #endregion

        public void OnGameLoad() {
            InitializeBackgroundLayer();
            InitializeGroundLayer();
            InitializeGroundMaskLayer();
            InitializeWaterLayer();
        }

        public void InitializeGroundLayer() {
            groundMap = new GameObject("Ground", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
            // m_GroundMap.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Foreground;
            // m_GroundMap.color = Screen.ForegroundColorShift;

            groundMap.gameObject.AddComponent<Rigidbody2D>();
            groundMap.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            groundMap.gameObject.AddComponent<CompositeCollider2D>();
            groundMap.gameObject.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
            groundMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
            // GroundMap.gameObject.AddComponent<ShadowCaster2DTileMap>();

            groundMap.transform.SetParent(transform);
            groundMap.transform.localPosition = Vector3.zero;
            groundMap.gameObject.layer = LayerMask.NameToLayer("Ground");

        }

        public void InitializeGroundMaskLayer() {
            groundMaskMap = new GameObject("Ground Mask", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            // m_GroundMapMask.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Midground;
            groundMaskMap.GetComponent<TilemapRenderer>().sortingOrder = 10000;
            // GroundMapMask.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            groundMaskMap.transform.SetParent(transform);
            groundMaskMap.transform.localPosition = Vector3.zero;
        }

        public void InitializeBackgroundLayer() {
            backgroundMap = new GameObject("Background", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            // m_GroundMapMask.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Midground;
            backgroundMap.GetComponent<TilemapRenderer>().sortingOrder = -10000;
            // GroundMapMask.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            backgroundMap.transform.SetParent(transform);
            backgroundMap.transform.localPosition = Vector3.zero;
        }


        public void InitializeWaterLayer() {
            waterMap = new GameObject("Water", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
            // m_WaterMap.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Foreground;
            waterMap.GetComponent<TilemapCollider2D>().isTrigger = true;
            waterMap.transform.SetParent(transform);
            waterMap.transform.localPosition = Vector3.zero;
            waterMap.gameObject.layer = LayerMask.NameToLayer("Water");
        }

        public void GenerateMap(Room room, List<LDtkTileData> tileData) {

            List<LDtkTileData> groundData = tileData.FindAll(data => data.vectorID == LDtkTileData.GROUND_ID);
            for (int i = 0; i < tileData.Count; i++) {
                Vector3Int tilePosition = room.GridToTilePosition(tileData[i].gridPosition);
                this.groundMap.SetTile(tilePosition, this.groundTile);
                this.groundMaskMap.SetTile(tilePosition, this.maskTile);
            }

            for (int i = 0; i < room.height; i++) {
                for (int j = 0; j < room.width; j++) {
                    Vector3Int tilePosition = new Vector3Int(room.worldPosition.x + j, room.worldPosition.y - i, 0);
                    this.backgroundMap.SetTile(tilePosition, this.backgroundTile);
                }
            }
                       
            // List<LDtkTileData> waterData = tileData.FindAll(data => data.vectorID == LDtkTileData.WATER_ID);
            // for (int i = 0; i < tileData.Count; i++) {
            //     Vector3Int tilePosition = room.GridToTilePosition(tileData[i].gridPosition);
            //     this.waterMap.SetTile(tilePosition, this.waterTile);
            // } 
        }

    }
    
}