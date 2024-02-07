// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityExtensions;
// // Platformer.
// using Platformer.Objects.Blocks;

// /* --- Definitions --- */
// using Game = Platformer.GameManager;
// using CharacterController = Platformer.Character.CharacterController;
// using SwitchOrb = Platformer.Objects.Orbs.SwitchOrb;

// namespace Platformer.Objects.Blocks {

//     ///<summary>
//     /// 
//     ///<summary>
//     public class SwitchBlock : BlockObject {

//         #region Methods.

//         protected override bool CheckActivationCondition() {
//             SwitchOrb[] switchOrbs = (SwitchOrb[])GameObject.FindObjectsOfType(typeof(SwitchOrb));
//             for (int i = 0; i < switchOrbs.Length; i++) {
//                 if (!switchOrbs[i].Collected) {
//                     return false;
//                 }
//             }
//             return true;
//         }

//         protected override void OnActivation() {
//             base.OnActivation();
//             m_Hitbox.enabled = false;
//         }

//         protected override void OnDeactivation() {
//             base.OnDeactivation();
//             ShoveAnythingInsideOut();
//             m_Hitbox.enabled = true;
//         }

//         private void ShoveAnythingInsideOut() {
//             Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(transform.position, 0.05f, Game.Physics.CollisionLayers.Characters);
//             for (int i = 0; i < colliders.Length; i++) {
//                 float direction = Mathf.Sign(colliders[i].transform.position.x - transform.position.x);
//                 if (direction == 0f) {
//                     direction = 1f;
//                 }
//                 colliders[i].transform.position = new Vector2(colliders[i].transform.position.x + 0.5f * direction, colliders[i].transform.position.y);
//             }
//         }

//         // Resets the block.
//         public override void Reset() {
//             m_Hitbox.enabled = true;
//             base.Reset();
//         }

//         #endregion

//     }

// }