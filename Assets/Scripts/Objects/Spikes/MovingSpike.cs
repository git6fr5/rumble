// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityExtensions;
// // Platformer.
// using Platformer.Objects.Spikes;

// /* --- Definitions --- */
// using Game = Platformer.Management.GameManager;
// using CharacterController = Platformer.Character.CharacterController;

// namespace Platformer.Objects.Spikes {

//     ///<summary>
//     ///
//     ///<summary>
//     [RequireComponent(typeof(BoxCollider2D))]
//     public class FallingSpike : SpikeObject {

//         #region Enumerations.

//         public enum MoveState {
//             Looking, 
//             Crumbling, 
//             Falling, 
//             Reforming
//         }

//         #endregion

//         #region Variables.

//         /* --- Constants --- */

//         // The period with which this cycles.
//         private const float PERIOD = 4f;

//         // The ellipse with which this cycles.
//         private static Vector2 ELLIPSE = new Vector2(0f, 8f/16f);

//         /* --- Components --- */

//         // The body attached to this gameObject 
//         private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();

//         /* --- Members --- */

//         // Tracks how long this is crumbling for
//         [SerializeField] 
//         private Timer m_CycleTimer = new Timer(0f, 0f);

//         // Tracks how long this is crumbling for
//         [SerializeField] 
//         private Timer m_PauseTimer = new Timer(0f, 0f);

//         // The strength with which this shakes while crumbling
//         [SerializeField] 
//         private float m_ShakeStrength;
//         private float Strength => m_ShakeStrength * m_CrumbleTimer.InverseRatio;

//         // public Sparkle m_Sparkle;
        
//         #endregion
        
//         // Runs once every fixed interval.
//         void FixedUpdate() {
//             bool finished = m_CycleTimer.TickDown(Time.fixedDeltaTime);

//             // What to do for each state.
//             switch (m_FallState) {
//                 case FallState.Moving:
//                     WhileMoving();
//                     break;
//                 case FallState.Waiting:
//                     WhileWaiting();
//                     break;
//                 default:
//                     break;
//             }

//             m_CycleTimer.Cycle(Time.fixedDeltaTime);
//             transform.Cycle(m_CycleTimer.Value, m_CycleTimer.MaxValue, m_Origin, ELLIPSE);

//         }

//         private void WhileMoving() {

//         }

//     }

// }
