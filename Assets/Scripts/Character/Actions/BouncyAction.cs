// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// // Gobblefish.
// using Gobblefish.Audio;
// // Platformer.
// using Platformer.Physics;

// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [CreateAssetMenu(fileName="BouncyAction", menuName ="Actions/Bouncy")]
//     public class BouncyAction : CharacterAction {

//         #region Variables.
        
//         // The speed of the actual bounce.
//         [SerializeField] 
//         private float m_BounceSpeed = 24f;

//         // The speed of the actual bounce.
//         [SerializeField] 
//         private float m_BounceWeight = 24f;

//         // The speed of the actual bounce.
//         [SerializeField] 
//         private float m_HangWeight = 24f;

//         // The speed of the actual bounce.
//         [SerializeField] 
//         private float m_BounceDuration = 0.2f;

//         // The speed of the actual bounce.
//         [SerializeField] 
//         private float m_PostBounceDuration = 0.2f;

//         [SerializeField]
//         private Timer m_BounceTimer = new Timer(0f, 0f);

//         // The direction the player was facing before the bounce started.
//         [HideInInspector]
//         protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

//         // The sprites this is currently animating through.
//         [SerializeField]
//         protected Sprite[] m_PrebounceAnimation = null;

//         // The sprites this is currently animating through.
//         [SerializeField]
//         protected Sprite[] m_BounceAnimation = null;

//         // The sprites this is currently animating through.
//         [SerializeField]
//         protected Sprite[] m_PostbounceAnimation = null;

//         // The visual effect that plays at the start of the bounce.
//         [SerializeField]
//         protected VisualEffect m_StartBounceEffect;

//         // The visual effect that plays at the start of the bounce.
//         [SerializeField]
//         protected AudioSnippet m_StartBounceSound;

//         // The visual effect that plays at the start of the bounce.
//         [SerializeField]
//         protected VisualEffect m_EndBounceEffect;


//         [SerializeField]
//         private PhysicsMaterial2D m_BouncyMaterial;

//         [SerializeField]
//         private PhysicsMaterial2D m_DefaultMaterial;

//         #endregion

//         // When enabling/disabling this ability.
//         public override void Enable(CharacterController character, bool enable = true) {
//             base.Enable(character, enable);
//             m_ActionPhase = ActionPhase.None;
//             OnEndBounce(character);

//             if (!enable) {
//                 character.Animator.Remove(m_PrebounceAnimation);
//                 character.Animator.Remove(m_PostbounceAnimation);
//                 character.Animator.Remove(m_BounceAnimation);
//             }

//         }

//         // When this ability is activated.
//         public override void InputUpdate(CharacterController character) {
//             if (!m_Enabled) { return; }

//             // Bouncing.
//             // m_ActionPhase == ActionPhase.None && 
//             if (character.Input.Actions[1].Pressed && m_Refreshed && !character.OnGround) {
//                 // The character should start bouncing.
//                 OnStartBounce(character);

//                 // Release the input and reset the refresh.
//                 character.Input.Actions[1].ClearPressBuffer();
//                 m_Refreshed = false;
//             }

//             // Bouncing.
//             // if (character.Input.Actions[1].Released && m_ActionPhase != ActionPhase.None) {
//             //     // The character should start bouncing.
//             //     OnEndBounce(character);

//             //     // Release the input and reset the refresh.
//             //     character.Input.Actions[1].ClearReleaseBuffer();
//             //     m_Refreshed = false;
//             // }

//         }
        
//         // Refreshes the settings for this ability every interval.
//         public override void PhysicsUpdate(CharacterController character, float dt){
//             if (!m_Enabled) { return; }

//             // Whether the power has been reset by touching ground after using it.
//             m_Refreshed = character.OnGround && m_ActionPhase == ActionPhase.None ? true : m_Refreshed;

//             // Tick down the bounce timer.
//             bool finished = m_BounceTimer.TickDown(dt);

//             // If swapping states.
//             if (finished) { 

//                 switch (m_ActionPhase) {
//                     case ActionPhase.MidAction:
//                         OnStartPostbounce(character);
//                         break;
//                     case ActionPhase.PostAction:
//                         OnEndBounce(character);
//                         break;
//                     default:
//                         break;
//                 }

//             }


//             // If in a phase.
//             switch (m_ActionPhase) {
//                 case ActionPhase.MidAction:
//                     WhileBouncing(character, dt);
//                     break;
//                 case ActionPhase.PostAction:
//                     WhilePostbouncing(character, dt);
//                     break;
//                 default:
//                     break;
//             }

//         }
       
//         Vector2 cachedDirection = Vector2.zero;
//         protected virtual void OnStartBounce(CharacterController character) {
//             character.Default.Enable(character, false);
//             character.Body.SetWeight(m_BounceWeight);

//             // Vector2 direction = character.Body.velocity; // character.Input.Direction.Normal; // new Vector2(character.FacingDirection, 0f); // 
//             // direction = direction.normalized;
//             // cachedDirection = direction;
            
//             // character.Body.SetVelocity(direction * m_BounceSpeed);

//             character.Body.ClampRiseSpeed(-5f);
            

//             character.Collider.sharedMaterial = m_BouncyMaterial;
//             m_ActionPhase = ActionPhase.MidAction;

//             m_BounceTimer.Start(m_BounceDuration);

//             // Replace the animation.
//             character.Animator.Remove(m_PrebounceAnimation);
//             character.Animator.Push(m_BounceAnimation, CharacterAnimator.AnimationPriority.ActionActive);
//             character.Animator.PlayAudioVisualEffect(m_StartBounceEffect, m_StartBounceSound);
//             if (character.Default.Trail != null) { character.Default.Trail.Play(); }
//         }

//         protected void OnStartPostbounce(CharacterController character) {
//             m_BounceTimer.Start(m_PostBounceDuration);

//             m_ActionPhase = ActionPhase.PostAction;
//         }

//         // End the bounce.
//         protected void OnEndBounce(CharacterController character) {
//             character.Default.Enable(character, true);

//             character.Animator.Remove(m_BounceAnimation);
//             character.Animator.Remove(m_PostbounceAnimation);
//             character.Collider.sharedMaterial = m_DefaultMaterial;
            
//             m_BounceTimer.Stop();

//             m_ActionPhase = ActionPhase.None;
//             if (character.Default.Trail != null) { character.Default.Trail.Stop(); }
//         }

//         private void WhileBouncing(CharacterController character, float dt) {
//             // if (cachedDirection != character.Body.velocity.normalized) {
//             //     cachedDirection = character.Body.velocity.normalized;
//             //     character.Body.velocity *= 1.02f;
//             // }
//             if (character.Body.velocity.y > 0f) { 
//                 // character.Body.velocity *= 1.2f;
//                 character.Body.SetWeight(m_HangWeight);
//                 m_Refreshed = true;
//                 OnStartPostbounce(character);
//             }
//             // character.Body.ClampSpeed(m_BounceSpeed * 1.5f);

//         }

//         private void WhilePostbouncing(CharacterController character, float dt) {
//             // if (cachedDirection != character.Body.velocity.normalized) {
//             //     cachedDirection = character.Body.velocity.normalized;
//             //     character.Body.velocity *= 1.02f;
//             // }

//             // if (character.Body.velocity.y < 0f) {
//             //     character.Body.SetWeight(m_HangWeight);
//             // }
            
//             // if (character.OnGround) { 
//             //     OnEndBounce(character);
//             // }
//             // character.Body.ClampSpeed(m_BounceSpeed * 1.5f);
//         }

//     }
// }