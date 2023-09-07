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

//
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Tilemaps {

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Grid))]
    public class TilemapManager : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // The ground map.
        public Tilemap groundCollision { get; private set; }

        // The ground map mask.
        public Tilemap ground0 { get; private set; }
        public Tilemap ground1 { get; private set; }
        public Tilemap ground2 { get; private set; }

        // The ground map mask.
        public Tilemap groundEdge { get; private set; }

        // The water map.
        public Tilemap waterMap { get; private set; }

        // The background map.
        public Tilemap backgroundMap { get; private set; }

        // The tiles for the ground map.
        [field: SerializeField] 
        public RuleTile groundTile { get; private set; }

        // The tiles for the ground mask.
        [field: SerializeField] 
        public RuleTile maskTile { get; private set; }

        // The tiles for the ground water.
        [field: SerializeField] 
        public WaterTile waterTile { get; private set; }

        // The tiles for the background.
        [field: SerializeField] 
        public BackgroundTile backgroundTile { get; private set; }

        #endregion

        public void OnGameLoad() {
            // InitializeBackgroundLayer();
            InitializeGroundLayers();
            // InitializeGroundMaskLayer();
            // InitializeWaterLayer();
        }

        public void InitializeGroundLayers() {
            groundCollision = new GameObject("Ground", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();

            groundCollision.transform.SetParent(transform);
            groundCollision.transform.localPosition = Vector3.zero;
            groundCollision.gameObject.layer = Game.Physics.CollisionLayers.TileLayer;
            groundCollision.GetComponent<TilemapRenderer>().sortingLayerName = Game.Visuals.RenderingLayers.TileLayer;
            groundCollision.GetComponent<TilemapRenderer>().sortingOrder = Game.Visuals.RenderingLayers.TileOrder;

            ground0 = InitializeGroundLayer(10);
            ground1 = InitializeGroundLayer(20);
            ground2 = InitializeGroundLayer(30);
            groundEdge = InitializeGroundLayer(40);

        }

        public Tilemap InitializeGroundLayer(int order) {
            Tilemap map = new GameObject("Ground Mask", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            map.GetComponent<TilemapRenderer>().sortingLayerName = "Foreground";
            map.GetComponent<TilemapRenderer>().sortingOrder = order; // 10000;
            map.transform.SetParent(transform);
            map.transform.localPosition = Vector3.zero;
            return map;
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

        public void Fill(List<Room> rooms) {
            
        }

        [System.Serializable]
        public class LDtkTileEntity {
            public TileBase tile;
            public Vector2Int vectorID;
        }

        public List<LDtkTileEntity> maskTiles = new List<LDtkTileEntity>();

        public void GenerateMap(Room room, Tilemap map, List<LDtkTileData> tileData) {

            // List<LDtkTileData> groundData = tileData.FindAll(data => data.vectorID == LDtkTileData.GROUND_ID);

            for (int i = 0; i < tileData.Count; i++) {

                TileBase maskTile = maskTiles.Find(tileEnt => tileEnt.vectorID == tileData[i].vectorID)?.tile;
                // print(tileData[i].vectorID);
                
                if (groundTile != null) {

                    Vector3Int tilePosition = room.GridToTilePosition(tileData[i].gridPosition);

                    this.groundCollision.SetColliderType(tilePosition, UnityEngine.Tilemaps.Tile.ColliderType.Grid);
                    this.groundCollision.SetTile(tilePosition, this.groundTile);
                    map.SetTile(tilePosition, maskTile);

                }

            }

            map.RefreshAllTiles();

        }

    }
    
}