/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
using UnityEngine.Rendering;
// Gobblefish.
using Gobblefish.Input;
using Gobblefish.Extensions;
// Platformer.
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.GameManager;
// using ShadowBlock = Platformer.Objects.Blocks.GhostBlock;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class GhostAction : CharacterAction {

        #region Variables.

        /* --- Constants --- */

        // The base speed with which the corpse rotates at.
        private const float BASE_CORPSE_ROTATE_SPEED = 180f;

        // The maximum distance before applying pressure between the corpse and anchor.
        private const float TETHER_DISTANCE = 12f;

        // The strength of the tether.
        private const float BASE_TETHER_STRENGTH = 0.8f;

        // The return of the ghost 's speed
        private const float GHOST_RETURN_SPEED = 25f;

        // The friction the ghost hand feels.
        private const float FRICTION = 0.2f;

        /* --- Members --- */

        // The time it takes to fully charge.
        [SerializeField]
        private float m_GhostModeDuration = 4f;

        // The speed the ghost hand moves at.
        [SerializeField]
        private float m_GhostAcceleration = 60f;

        // The acceleration with which the ghost hand moves at.
        [SerializeField]
        private float m_GhostSpeed = 8f;

        // The timer that tracks how much has been charged.
        [SerializeField]
        private Timer m_GhostTimer = new Timer(0f, 0f);
        public bool GhostModeActive => m_ActionPhase == ActionPhase.MidAction && m_GhostTimer.Active;

        // The sprite thats used for the ghost.
        [SerializeField]
        private Sprite m_CorpseSprite = null;

        // The ghost anchor.
        [HideInInspector]
        private Rigidbody2D m_Corpse;

        // The animation that plays when the ghost is active.
        [SerializeField]
        private Sprite[] m_GhostModeAnimation = null;

        //
        private Quaternion m_CachedRotation;

        [SerializeField]
        private VolumeProfile m_GhostVolumeProfile;

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);

            m_CachedRotation = Quaternion.identity;

            if (!enable) {
                if (m_GhostTimer.Active) {
                    OnGhostReachedAnchor(character);
                }
                m_ActionPhase = ActionPhase.None;
                m_GhostTimer.Stop();
                character.Animator.Remove(m_GhostModeAnimation);
            }

        }

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // Ghost.
            if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
                OnStartGhostMode(character);
                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }

            // Ghost.
            if (character.Input.Actions[1].Released && m_ActionPhase == ActionPhase.MidAction) {
                OnEndGhostMode(character);
                character.Input.Actions[1].ClearReleaseBuffer();
                m_Refreshed = false;
            }

        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && !m_GhostTimer.Active ? true : m_Refreshed;
            
            // Tick down the shadow mode.
            bool finished = m_GhostTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.MidAction:
                        OnEndGhostMode(character);
                        break;
                    default:
                        break;
                }

            }

            switch (m_ActionPhase) {
                case ActionPhase.MidAction:
                    WhileInGhostMode(character, dt);
                    break;
                case ActionPhase.PostAction:
                    WhileEndingGhostMode(character, dt);
                    break;
                default:
                    break;
            }
            
        }

        private void OnStartGhostMode(CharacterController character) {
            character.Default.Enable(character, false);

            // Start the shadow mode timer.
            m_GhostTimer.Start(m_GhostModeDuration);
            m_ActionPhase = ActionPhase.MidAction;

            // m_CachedRotation = character.transform.localRotation;
            BoundsInt bounds = Game.Graphics.MainCamera.GetBoundsInt(Game.Level.Maps.Grid);
            Game.Level.Maps.ConvertToBlocks(bounds.Pad(2));
            Game.Graphics.PostProcessor.SetVolumeProfile(m_GhostVolumeProfile);

            character.Body.Stop();

            if (m_Corpse == null) {
                m_Corpse = new GameObject("Corpse", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D)).GetComponent<Rigidbody2D>();
                // Platformer.Objects.LevelEditing.BridgeRope rope = new GameObject("Corpse", typeof(Platformer.Objects.LevelEditing.BridgeRope)).GetComponent<Platformer.Objects.LevelEditing.BridgeRope>();
                // rope.endpoint = character.transform;
                // rope.startpoint = m_Corpse.transform;
                // rope.transform.parent = m_Corpse.transform;
            }
            m_Corpse.gameObject.SetActive(true);
            
            // if (character.Input.Direction.Normal == Vector2.zero) {
            //     m_Corpse.transform.position = character.transform.position + (Vector3)character.Input.Direction.MostRecent * 1f;
            // }
            // else {
            //     m_Corpse.transform.position = character.transform.position + (Vector3)character.Input.Direction.Normal * 1f;
            // }

            m_Corpse.gameObject.layer = LayerMask.NameToLayer("Character Ghost");
            m_Corpse.transform.position = character.transform.position;
            m_Corpse.GetComponent<BoxCollider2D>().size = new Vector3(0.5f, 0.5f);
            m_Corpse.GetComponent<BoxCollider2D>().isTrigger = false;
            m_Corpse.GetComponent<SpriteRenderer>().sprite = m_CorpseSprite;
            m_Corpse.SetWeight(0f, 3f);
            m_Corpse.velocity = Vector2.zero;
            m_Corpse.ReleaseXY();

            // m_CircleEffectIndex = Game.Visuals.Effects.PlayCircleEffect(m_GhostModeDuration, character.transform, Vector3.zero);
            character.Animator.Push(m_GhostModeAnimation, CharacterAnimator.AnimationPriority.ActionActive);
        
        }

        private void OnEndGhostMode(CharacterController character) {
            // Stop the shadow timer.
            m_GhostTimer.Stop();
            m_ActionPhase = ActionPhase.PostAction;

            m_Corpse.GetComponent<BoxCollider2D>().isTrigger = true;

            Vector3 directionToAnchor = (character.transform.position - m_Corpse.transform.position).normalized;            
            m_Corpse.velocity = directionToAnchor * GHOST_RETURN_SPEED;

            Game.Graphics.PostProcessor.RemoveVolumeProfile(m_GhostVolumeProfile);
            
            // character.transform.localRotation = m_CachedRotation;
            // Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);

        }

        private void OnGhostReachedAnchor(CharacterController character) {
            // character.transform.localRotation = m_CachedRotation;

            character.Default.Enable(character, true);
            character.Body.Stop();
            
            m_ActionPhase = ActionPhase.None;

            if (m_Corpse != null) {
                m_Corpse.gameObject.SetActive(false);
            }
            character.Animator.Remove(m_GhostModeAnimation);
            
        }

        private void WhileInGhostMode(CharacterController character, float dt) {
            Vector2 direction = character.Input.Direction.Normal;
            m_Corpse.AddVelocity(m_GhostAcceleration * direction * dt);

            if (m_Corpse.velocity.magnitude > m_GhostSpeed) {
                m_Corpse.SetVelocity(m_GhostSpeed * m_Corpse.velocity.normalized);
            }

            // float rotationFactor = m_Corpse.velocity.magnitude / m_GhostSpeed;
            // character.transform.Rotate(BASE_CORPSE_ROTATE_SPEED * rotationFactor, dt);
            character.Body.velocity *= 0.9f;

            float distanceToAnchor = (character.transform.position - m_Corpse.transform.position).magnitude;
            float distanceNextStep = (character.transform.position - (m_Corpse.transform.position + (Vector3)m_Corpse.velocity * dt)).magnitude;
            bool movingAway = distanceNextStep > distanceToAnchor;
            
            if (distanceToAnchor > TETHER_DISTANCE && movingAway) {
                m_Corpse.velocity *= (TETHER_DISTANCE / distanceToAnchor) * BASE_TETHER_STRENGTH;
            }

            if (direction == Vector2.zero) {
                m_Corpse.Slowdown(1f - FRICTION);
                if (m_Corpse.velocity.magnitude <= Game.Physics.Collisions.CollisionPrecision) {
                    m_Corpse.velocity = Vector2.zero;
                }
            }

            // float angle = Vector2.SignedAngle(Vector2.right, m_Corpse.velocity);
            // m_Corpse.transform.eulerAngles = 180f * Vector3.up * flip + Vector3.forward * angle;

        }

        
        private void WhileEndingGhostMode(CharacterController character, float dt) {

            // character.transform.localRotation = m_CachedRotation;

            Vector3 directionToAnchor = (character.transform.position - m_Corpse.transform.position).normalized;            
            m_Corpse.velocity = directionToAnchor * GHOST_RETURN_SPEED;
            
            float distanceToAnchor = (character.transform.position - m_Corpse.transform.position).magnitude;   
            if (distanceToAnchor < 0.5f) {
                OnGhostReachedAnchor(character);
            }

        }

    }
}