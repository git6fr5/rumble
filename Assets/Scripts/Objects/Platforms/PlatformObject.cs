/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformObject : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The height of a platform.
        public const float PLATFORM_HEIGHT = 5f/16f;

        // In order to properly center the platform.
        protected const float COLLIDER_INSET = 2f/16f;

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Hitbox = null;
        
        // The animator that renders this platform.
        [SerializeField]
        protected PlatformAnimator m_Animator = null;

        /* --- Parameters --- */

        // The position that this platform was spawned at.
        protected Vector3 m_Origin;

        [SerializeField, ReadOnly]
        protected float m_Length = 0f;
        // public float AdjustedLength => m_Length - 2f * (0.5f - COLLIDER_OFFSET); 
        public float HitboxLength => m_Length == 1f ? 1f : m_Length - 2f * COLLIDER_INSET; 
        
        // The objects that are attached to the platform.
        [SerializeField, ReadOnly] 
        protected List<Transform> m_CollisionContainer = new List<Transform>();
        
        // Whether this platform is being pressed down.
        [SerializeField, ReadOnly]
        protected bool m_Pressed = false;
        
        // Whether this platform is being pressed down.
        [SerializeField, ReadOnly]
        protected bool m_CachePressed = false;

        // The sound to be played when this platform is pressed.
        [SerializeField] 
        private AudioClip m_OnPressedSound;

        #endregion

        #region Methods.

        // Initialize the platform.
        public void Start() {
            // Cache the origin.
            m_Origin = transform.position;

            // Collision settings.
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_Hitbox.isTrigger = false;
            gameObject.layer = Game.Physics.CollisionLayers.PlatformLayer;

            // Rendering settings.
            m_Animator.Initialize(this);

        }

        // Runs once every frame.
        protected virtual void Update() {
            m_Pressed = CheckPressed(transform.position.y, m_CollisionContainer);

            // If the platform was just pressed.
            if (m_Pressed && !m_CachePressed) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
                m_Animator.OnPressed();
            }
            // If the platform was just released.
            else if (m_CachePressed && !m_Pressed) {
                m_Animator.OnReleased();
            }

            m_CachePressed = m_Pressed;
        }

        
        // Check if a character is standing on top of this.
        public static bool CheckPressed(float platformHeight, List<Transform> collisionContainer) {
            if (collisionContainer.Count == 0) { return false; }

            for (int i = 0; i < collisionContainer.Count; i++) {
                CharacterController character = collisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    float characterFeetHeight = character.Body.position.y + character.Collider.offset.y - character.Collider.radius;
                    bool isAbove = characterFeetHeight > platformHeight;
                    bool movingVertically = character.Falling || character.Rising;
                    if (isAbove && !movingVertically) {
                        return true;
                    }
                }
            }
            return false;
        }

        // Runs when something collides with this platform.
        private void OnCollisionEnter2D(Collision2D collision) {
            // Check if there is a character.
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (!m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Add(character.transform);
            }
            
        }

        // Runs when something exit this platform.
        private void OnCollisionExit2D(Collision2D collision) {
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Remove(character.transform);
            }
        }

        #endregion

    }
}
