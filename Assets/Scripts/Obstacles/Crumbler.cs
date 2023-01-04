// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityExtensions;
// // Platformer.
// using Platformer.Objects.Platforms;

// /* --- Definitions --- */
// using Game = Platformer.Management.GameManager;
// using CharacterController = Platformer.Character.CharacterController;

// namespace Platformer.Objects {

//     public interface IAlternator {

//         void OnStartCrumble() {
            
//         }

//         void OnCrumble() {
            
//         }

//         void OnChange(bool enable) {
            
//         }

//     }

//     ///<summary>
//     ///
//     ///<summary>
//     [RequireComponent(typeof(Crumbler))]
//     public class CrumblingPlatform : PlatformObject, ICrumbler {

//         #region Enumerations.

//         public enum CrumbleState {
//             None,
//             Crumbling,
//             Reforming
//         }

//         #endregion

//         #region Variables.

//         // Whether this platform is crumbling.
//         [SerializeField] 
//         private CrumbleState m_CrumbleState = CrumbleState.None;
        
//         // The duration this is crumbling for.
//         [SerializeField] 
//         private float m_CrumbleDuration = 0.5f;

//         // The time it takes for this to reform.
//         [SerializeField] 
//         private float m_ReformDuration = 1f;

//         // Tracks whether        
//         [SerializeField] 
//         private Timer m_CrumbleTimer = new Timer(0f, 0f);

//         // The base strength this shakes with while crumbling.
//         [SerializeField] 
//         private float m_ShakeStrength = 0.12f;
        
//         // The adjusted shake strength.
//         private float Strength => m_ShakeStrength * m_CrumbleTimer.InverseRatio;

//         // The sound this plays while crumbling
//         [SerializeField] 
//         private AudioClip m_WhileCrumblingSound = null;
        
//         // The sound this plays on crumbling.
//         [SerializeField] 
//         private AudioClip m_OnCrumbleSound = null;

//         // The sound this plays on reforming.
//         [SerializeField] 
//         private AudioClip m_OnReformSound = null;

//         #endregion

//         #region Methods.

//         void Start() {
//             m_ICrumbler.StartCrumbler();
//         }

//         // Runs once every frame.
//         // Having to do this is a bit weird.
//         void Update() {
            
//             // What to do for each state.
//             switch (m_CrumbleState) {
//                 case CrumbleState.None:
//                     if (m_PressedDown) { StartCrumble(); }
//                     break;
//                 case CrumbleState.Crumbling:
//                     transform.Shake(m_Origin, Strength); // Should this be in while crumbling?
//                     break;
//                 default:
//                     break;
//             }
        
//         }

//         // Runs once every fixed interval.
//         void FixedUpdate() {
//             bool finished = m_CrumbleTimer.TickDown(Time.fixedDeltaTime);

//             // Whenever the crumble timer hits 0.
//             if (finished) {

//                 switch (m_CrumbleState) {
//                     case CrumbleState.Crumbling:
//                         Crumble();
//                         break;
//                     case CrumbleState.Reforming:
//                         Reform();
//                         break;
//                     default:
//                         break;
//                 }

//             }

//             // What to do for each state.
//             switch (m_CrumbleState) {
//                 case CrumbleState.Crumbling:
//                     WhileCrumbling(Time.fixedDeltaTime);
//                     break;
//                 case CrumbleState.Reforming:
//                     WhileReforming(Time.fixedDeltaTime);
//                     break;
//                 default:
//                     break;
//             }

//         }

//         private void StartCrumble() {
//             m_CrumbleState = CrumbleState.Crumbling;
//             m_CrumbleTimer.Start(m_CrumbleDuration);
//             m_ICrumbler.OnStartCrumble();
//         }

//         private void Crumble() {
//             m_CrumbleState = CrumbleState.Reforming;
//             m_CrumbleTimer.Start(m_ReformDuration);
//             m_Hitbox.enabled = false;
//             m_SpriteShapeRenderer.enabled = false;
            
//             Game.Audio.Sounds.PlaySound(m_OnCrumbleSound, 0.15f);
//             m_ICrumbler.OnCrumble();
//         }

//         private void Reform() {
//             m_Hitbox.enabled = true;
//             m_CrumbleState = CrumbleState.None;
//             m_SpriteShapeRenderer.enabled = true;

//             Game.Audio.Sounds.PlaySound(m_OnReformSound, 0.15f);
//             m_ICrumbler.OnReform();
//         }

//         private void WhileCrumbling(float dt) {
//             Game.Audio.Sounds.PlaySound(m_WhileCrumblingSound, Mathf.Sqrt(m_CrumbleTimer.InverseRatio) * 1f);
//         }

//         private void WhileReforming(float dt) {

//         }

//         #endregion

//     }

// }
