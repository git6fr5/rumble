// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using LDtkUnity;

// /* --- Definitions --- */
// using Game = Platformer.GameManager;
// // using SaveSystem = Platformer.Management.SaveSystem;

// namespace Platformer.Levels {

//     /// <summary>
//     /// Loads all the levels in the world from the LDtk file.
//     /// </summary>
//     public class PlayerManager: MonoBehaviour {

//         [HideInInspector]
//         private int m_Deaths = 0;
//         public int Deaths => m_Deaths;

//         [HideInInspector]
//         private int m_Points = 0;
//         public int Points => m_Points;

//         // Resets the current room.
//         public void Reset() {
//             // Platformer.Objects.Blocks.BlockObject.ResetAll();
//             // Platformer.Objects.Orbs.OrbObject.ResetAll();
//         }

//         // Loads the entities for
//         public void Load(LevelSection section) {
//             // for (int i = 0; i < section.Pieces.Length; i++) {
//             //     section.Pieces[i].SetActive(true);
//             // }
//         }

//         public void Unload(LevelSection section) {
//             // for (int i = 0; i < section.Pieces.Length; i++) {
//             //     section.Pieces[i].SetActive(false);
//             // }
//             // Platformer.Objects.Spitters.Projectile.DeleteAll(); // Should go somewhere saying custom.
//         }

//         public void AddDeath() {
//             m_Deaths += 1;
//         }

//         public void AddPoint() {
//             m_Points += 1;
//         }

//         public void OnSaveAndQuit() {
//         }

//         public void OnComplete() {

//         }

//     }

// }
