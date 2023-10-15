// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// // Platformer.
// using Platformer.Levels.LDtk;
// using Platformer.Levels.Tilemaps;

// namespace Platformer.Levels.Tilemaps {

//     /// <summary>
//     /// Stores specific data on how to generate the level.
//     /// </summary>
//     [RequireComponent(typeof(UnityEngine.Grid))]
//     public class TilemapManager : MonoBehaviour {

//         // Initializes the world.
//         void Start() {
//             List<Room> rooms = Collect(m_JSON, transform);
//             LoadMap(rooms);
//         }

//         // Collects all the levels from the LDtk file.
//         private static List<Room> Collect(LdtkJson json, Transform transform) {
//             List<Room> rooms = new List<Room>();
//             for (int i = 0; i < json.Levels.Length; i++) {
//                 Room room = Instantiate(m_RoomBase.gameObject);
//                 room.gameObject.name = json.Levels[i].Identifier;
//                 room.transform.SetParent(transform);
//                 rooms.Add(room);
//             }
//             return rooms;
//         }

//         public void LoadMap(List<Room> rooms) {
//             // Itterate through and load all the level data.
//             for (int i = 0; i < rooms.Count; i++) {
//                 List<LDtkTileData> tileData = LDtkReader.GetLayerData(rooms[i].ldtkLevel, m_LDtkLayers.Ground);
//                 m_TilemapManager.GenerateMap(rooms[i], tileData);
//             }
//         }

//         public void GenerateMap(Room room, List<LDtkTileData> tileData) {

//             List<LDtkTileData> groundData = tileData.FindAll(data => data.vectorID == LDtkTileData.GROUND_ID);
//             for (int i = 0; i < tileData.Count; i++) {
//                 Vector3Int tilePosition = room.GridToTilePosition(tileData[i].gridPosition);
//                 this.groundMap.SetTile(tilePosition, this.groundTile);
//                 this.groundMaskMap.SetTile(tilePosition, this.maskTile);
//             }

//             for (int i = 0; i < room.height; i++) {
//                 for (int j = 0; j < room.width; j++) {
//                     Vector3Int tilePosition = new Vector3Int(room.worldPosition.x + j, room.worldPosition.y - i, 0);
//                     this.backgroundMap.SetTile(tilePosition, this.backgroundTile);
//                 }
//             }
                       
//         }

//     }
    
// }