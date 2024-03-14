/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;

namespace Platformer.Levels.LDtk {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class LDtkReader : MonoBehaviour {

        // Grid Size
        public const int GRID_SIZE = 16;

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LDtkEntityManager m_LDtkEntityManager;

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LDtkTilemapManager m_LDtkTilemapManager;

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LevelManager m_LevelManager;

        // The JSON data corresponding to the given ldtk data.
        [SerializeField]
        private LDtkLayers m_LDtkLayers = new LDtkLayers();

        // The given LDtk file.
        [SerializeField] 
        private LDtkComponentProject m_LDtkData;

        [Header("Controls")]
        public bool m_Reload;

        // The JSON data corresponding to the given ldtk data.
        private LdtkJson m_JSON;

        void OnEnable() {
            m_Reload = false;
            if (!Application.isPlaying) {
                OnReload();
            }
        }

        void Awake() {
            if (!Application.isPlaying) {
                OnReload();
            }
        }

        void Update() {
            if (m_Reload) {
                OnReload();
                m_Reload = false;
            }
        }

        public void OnReload() {
            m_JSON = m_LDtkData.FromJson();

            List<LevelSection> sections = CollectSections(m_JSON);
            m_LDtkTilemapManager.Refresh(sections, m_LDtkLayers.Ground);    

            for (int i = 0; i < sections.Count; i++) {
                sections[i].transform.parent = transform;
                sections[i].DestroyEntities();
                sections[i].GenerateEntities(m_LDtkEntityManager, m_LDtkLayers);
            }

            m_LevelManager.SetSections(sections);
            
        }

        // Collects all the levels from the LDtk file.
        private static List<LevelSection> CollectSections(LdtkJson json) {
            List<LevelSection> sections = new List<LevelSection>();
            for (int i = 0; i < json.Levels.Length; i++) {
                LevelSection section = LevelSection.New(i, json);
                sections.Add(section);
            }
            return sections;
        }

        public static List<LDtkTileData> GetLayerData(LDtkUnity.Level ldtkLevel, string layerName) {
            List<LDtkTileData> layerData = new List<LDtkTileData>();

            LDtkUnity.LayerInstance layer = GetLayerInstance(ldtkLevel, layerName);
            if (layer != null) { 
                for (int index = 0; index < layer.GridTiles.Length; index++) {
                    LDtkUnity.TileInstance tile = layer.GridTiles[index];
                    LDtkTileData tileData = new LDtkTileData(GetVectorID(tile), GetGridPosition(tile, (int)layer.GridSize), index, (int)layer.GridSize);
                    layerData.Add(tileData);
                }
            }
            return layerData;
        }

        public static LDtkUnity.LayerInstance GetLayerInstance(LDtkUnity.Level ldtkLevel, string layerName) {
            for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
                LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
                if (layer.IsTilesLayer && layer.Identifier == layerName) {
                    return layer;
                }
            }
            return null;
        }

        private static Vector2Int GetVectorID(LDtkUnity.TileInstance tile) {
            return new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / GRID_SIZE;
        }

        private static Vector2Int GetGridPosition(LDtkUnity.TileInstance tile, int gridSize) {
            return tile.UnityPx / gridSize;
        }

        protected static Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
            LDtkTileData tileData = data.Find(tileData => tileData != null && tileData.gridPosition == gridPosition);
            return (Vector2Int?)tileData?.vectorID;
        }

    } 

}
    