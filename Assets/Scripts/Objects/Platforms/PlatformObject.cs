/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Obstacle = Platformer.Obstacle;
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class PlatformObject : Obstacle {

        #region Variables.

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Hitbox = null;
        
        // The animator that renders this platform.
        [SerializeField]
        protected PlatformAnimator m_Animator = null;

        /* --- Parameters --- */

        // The position that this platform was spawned at.
        protected Vector3 m_Origin;

        // Whether this platform is being pressed down.
        [SerializeField, ReadOnly]
        protected bool m_Pressed = false;
        public bool Pressed => m_Pressed;
        
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
            // m_Hitbox = GetComponent<BoxCollider2D>();
            // m_Hitbox.isTrigger = false;
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

        #endregion



    }


}
