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

// namespace Platformer.Objects.Blocks {

//     ///<summary>
//     /// 
//     ///<summary>
//     [RequireComponent(typeof(Rigidbody2D))]
//     public class GhostBlock : BlockObject {

//         #region Variables.

//         /* --- Constants --- */

//         // The amount of friction the ghost block experiences while active.
//         public const float FRICTION = 0.2f;

//         // The amount of drag the ghost block experiences while active.
//         public const float DRAG = 0.2f;

//         // The amount of mass the block has (for collisions).
//         public const float MASS = 0.9f;

//         /* --- Components --- */
        
//         // The rigidbody attached to this component.
//         private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();

//         /* --- Members --- */

//         private bool m_CorpseTouched = false;
        
//         #endregion

//         #region Methods.

//         protected override bool CheckActivationCondition() {
//             return Game.MainPlayer.Ghost.Enabled && Game.MainPlayer.Ghost.GhostModeActive;
//         }

//         protected override void OnActivation() {
//             base.OnActivation();
//             m_Body.ReleaseAll();
//         }

//         protected override void OnDeactivation() {
//             base.OnDeactivation();
//             m_Body.Freeze();
//         }

//         // Runs once when something enters this area.
//         protected virtual void OnCollisionEnter2D(Collision2D collision) {
//             if (collision.gameObject.name == "Corpse") {
//                 m_CorpseTouched = true;
//             }
//         }

//         // Runs once when something enters this area.
//         protected virtual void OnCollisionExit2D(Collision2D collision) {
//             if (collision.gameObject.name == "Corpse") {
//                 m_CorpseTouched = false;
//             }
//         }

//         // Runs while the block is released.
//         protected override void WhileActive() {
//             if (m_CorpseTouched) {
//                 m_Body.ReleaseXY();
//                 m_Body.Slowdown(1f - FRICTION);
//             }
//             else {
//                 m_Body.ReleaseAll();
//                 m_Body.Slowdown(1f - FRICTION / 3f);
//             }
//         }

//         // Resets the block.
//         public override void Reset() {
//             transform.eulerAngles = Vector3.zero;
//             m_Body.SetAngularDrag(DRAG);
//             m_Body.SetWeight(0f, MASS);
//             m_Body.Freeze();
//             base.Reset();
//         }

//         #endregion

//     }

// }