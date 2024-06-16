/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
// Platformer.
using Platformer.Levels;
using Gobblefish;

namespace Platformer.Levels.LDtk {


    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    public class LDtkTilemapManager : MonoBehaviour {

        [SerializeField]
        private Grid m_Grid;

        [SerializeField]
        private Tilemap m_CollisionMap;

        [SerializeField]
        private Tilemap[] m_Submaps;
        private Tilemap m_TrackingColorMap;

        [SerializeField]
        private TileBase m_ChangingTile;

        [SerializeField]
        private TileBase m_StaticTile;

        // Loads the map layouts for all the given levels.
        public void Refresh(List<LevelSection> sections, string layerName) {
            print("is this running on load");

            if (m_Submaps != null) {
                for (int i = 0; i < m_Submaps.Length; i++) {
                    if (m_Submaps[i] != null) {
                        GameObject.DestroyImmediate(m_Submaps[i].gameObject);
                    }
                }
            }
            if (m_TrackingColorMap != null) {
                GameObject.DestroyImmediate(m_TrackingColorMap.gameObject);
            }

            m_CollisionMap.ClearAllTiles();

            ColorSwap.ColorSwappingLayer[] layers = ColorSwap.ColorSwapManager.Instance.coloredLayers;
            m_Submaps = new Tilemap[layers.Length];

            for (int i = 0; i < layers.Length; i++) {
                m_Submaps[i] = MakeMap();
            }

            m_TrackingColorMap = MakeMap();
            ColorSwap.TrackPlayerColor t = m_TrackingColorMap.gameObject.AddComponent<ColorSwap.TrackPlayerColor>();
            t.offset = 2;

            for (int i = 0; i < sections.Count; i++) {
                GenerateSection(sections[i], layerName);
            }

        }

        private Tilemap MakeMap() {
            Tilemap colMap = Instantiate(m_CollisionMap.gameObject).GetComponent<Tilemap>();
            colMap.GetComponent<TilemapRenderer>().sortingLayerName = "Foreground";
            colMap.GetComponent<TilemapRenderer>().sortingOrder = 0;
            colMap.gameObject.SetActive(true);
            colMap.transform.SetParent(m_CollisionMap.transform.parent);
            colMap.transform.localPosition = m_CollisionMap.transform.localPosition;
            return colMap;
        }

        public void GenerateSection(LevelSection section, string layerName) {
            List<LDtkTileData> tileData = LDtkReader.GetLayerData(section.ldtkLevel, layerName);
            List<LDtkTileData> colorData = LDtkReader.GetLayerData(section.ldtkLevel, "COLOR");

            for (int i = 0; i < tileData.Count; i++) {

                if (tileData[i].vectorID.x == 0) {
                    Vector3Int tilePosition = section.GridToTilePosition(tileData[i].gridPosition);
                    m_TrackingColorMap.SetTile(tilePosition, m_ChangingTile);
                }
                else if (tileData[i].vectorID.x == 1) {
                    LDtkTileData color = colorData.Find(n=>n.gridPosition == tileData[i].gridPosition);
                    int mapNumber = 0;
                    if (color != null) {
                        mapNumber = color.vectorID.x; 
                    }

                    if (mapNumber < m_Submaps.Length) {
                        Vector3Int tilePosition = section.GridToTilePosition(tileData[i].gridPosition);
                        m_Submaps[mapNumber].SetTile(tilePosition, m_StaticTile);
                    }     
                }
                                  
            }

            for (int i = 0; i < m_Submaps.Length; i++) {
                m_Submaps[i].RefreshAllTiles();
                ColorSwap.ColorSwapManager.Instance.SwapColor(m_Submaps[i].transform, i);
            }

        }
        
    }
    
}