/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Input;
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using ShadowBlock = Platformer.Objects.Blocks.GhostBlock;

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
        private const float TETHER_DISTANCE = 8f;

        // The strength of the tether.
        private const float BASE_TETHER_STRENGTH = 0.8f;

        // The return of the ghost 's speed
        private const float GHOST_RETURN_SPEED = 18f;

        // The friction the ghost hand feels.
        private const float FRICTION = 0.05f;

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

        // An index to the particle that is associated with the ghost timer.
        [SerializeField] 
        private int m_CircleEffectIndex = -1;

        // The animation that plays when the ghost is active.
        [SerializeField]
        private Sprite[] m_GhostModeAnimation = null;

        #endregion

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // Dashing.
            if (character.Input.Action1.Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
                // The character should start dashing.
                OnStartGhostMode(character);

                // Release the input and reset the refresh.
                character.Input.Action1.ClearPressBuffer();
                m_Refreshed = false;
            }

            // Dashing.
            if (character.Input.Action1.Released && m_ActionPhase == ActionPhase.MidAction) {
                // The character should start dashing.
                OnEndGhostMode(character);

                // Release the input and reset the refresh.
                character.Input.Action1.ClearReleaseBuffer();
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

            character.Body.Stop();

            if (m_Corpse == null) {
                m_Corpse = new GameObject("Corpse", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D)).GetComponent<Rigidbody2D>();
                // Platformer.Objects.Decorations.BridgeRope rope = new GameObject("Corpse", typeof(Platformer.Objects.Decorations.BridgeRope)).GetComponent<Platformer.Objects.Decorations.BridgeRope>();
                // rope.endpoint = character.transform;
                // rope.startpoint = m_Corpse.transform;
                // rope.transform.parent = m_Corpse.transform;
            }
            m_Corpse.gameObject.SetActive(true);
            if (character.Input.Direction.Normal == Vector2.zero) {
                m_Corpse.transform.position = character.transform.position + (Vector3)character.Input.Direction.MostRecent * 1f;
            }
            else {
                m_Corpse.transform.position = character.transform.position + (Vector3)character.Input.Direction.Normal * 1f;
            }
            m_Corpse.GetComponent<BoxCollider2D>().size = new Vector3(0.5f, 0.5f);
            m_Corpse.GetComponent<BoxCollider2D>().isTrigger = false;
            m_Corpse.GetComponent<SpriteRenderer>().sprite = m_CorpseSprite;
            m_Corpse.SetWeight(0f, 3f);
            m_Corpse.velocity = Vector2.zero;
            m_Corpse.ReleaseXY();

            m_CircleEffectIndex = Game.Visuals.Effects.PlayCircleEffect(m_GhostModeDuration, character.transform, Vector3.zero);
            character.Animator.Push(m_GhostModeAnimation, CharacterAnimator.AnimationPriority.ActionActive);
        
        }

        private void OnEndGhostMode(CharacterController character) {
            // Stop the shadow timer.
            m_GhostTimer.Stop();
            m_ActionPhase = ActionPhase.PostAction;

            m_Corpse.GetComponent<BoxCollider2D>().isTrigger = true;

            Vector3 directionToAnchor = (character.transform.position - m_Corpse.transform.position).normalized;            
            m_Corpse.velocity = directionToAnchor * GHOST_RETURN_SPEED;

            Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);

        }

        private void OnGhostReachedAnchor(CharacterController character) {
            character.transform.eulerAngles = Vector3.zero;
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

            float rotationFactor = m_Corpse.velocity.magnitude / m_GhostSpeed;
            character.transform.Rotate(BASE_CORPSE_ROTATE_SPEED * rotationFactor, dt);
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

            Vector3 directionToAnchor = (character.transform.position - m_Corpse.transform.position).normalized;            
            m_Corpse.velocity = directionToAnchor * GHOST_RETURN_SPEED;
            
            float distanceToAnchor = (character.transform.position - m_Corpse.transform.position).magnitude;   
            if (distanceToAnchor < 0.5f) {
                OnGhostReachedAnchor(character);
            }

        }

    }
}