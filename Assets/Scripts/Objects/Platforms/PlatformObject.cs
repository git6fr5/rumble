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
using IInitializable = Platformer.Levels.Entities.IInitializable;
using IElongatable = Platformer.Levels.Entities.IElongatable;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformObject : MonoBehaviour, IInitializable, IElongatable {

        #region Variables.

        /* --- Constants --- */

        // The height of a platform.
        private const float PLATFORM_HEIGHT = 5f/16f;

        // In order to properly center the platform.
        protected const float OFFSET = 6f/16f;

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Hitbox = null;
        
        // The sprite shape renderer attached to this platform.
        [SerializeField]
        protected SpriteShapeRenderer m_SpriteShapeRenderer = null;
        
        // The sprite shape controller. attached to this platform.
        [SerializeField]
        protected SpriteShapeController m_SpriteShapeController = null;

        // The spline attached to the sprite shape.
        protected Spline m_Spline = null;

        /* --- Parameters --- */

        // The position that this platform was spawned at.
        protected Vector3 m_Origin;

        [SerializeField, ReadOnly]
        protected float m_Length = 0f;
        public float AdjustedLength = m_Length - 2f * (0.5f - OFFSET); 
        
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

        // The alternate shape of the platform when it is pressed, if there is one.
        [SerializeField] 
        protected SpriteShape m_PressedShape = null;
        
        // The default shape of the platform, to be cached.
        [SerializeField, ReadOnly]
        protected SpriteShape m_DefaultShape = null;

        #endregion

        #region Methods.

        // Initialize the platform.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin.
            transform.position = worldPosition;
            m_Origin = worldPosition;

            // Collision settings.
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_Hitbox.isTrigger = false;
            gameObject.layer = Game.Physics.CollisionLayers.PlatformLayer;

            // Rendering settings.
            m_SpriteShapeRenderer.sortingLayerName = Game.Visuals.Rendering.PlatformLayer;
            m_SpriteShapeRenderer.sortingOrder = Game.Visuals.Rendering.PlatformOrder;
            m_Spline = m_SpriteShapeController.spline;
            m_DefaultShape = m_SpriteShapeController.spriteShape;

        }

        // Set the controls from the LDtk files.
        public virtual void SetLength(int length) {
            m_BaseLength = (float)length;
            
            // Set the hitbox length.
            m_Hitbox.size = new Vector2(AdjustedLength, PLATFORM_HEIGHT );
            m_Hitbox.offset = new Vector2(AdjustedLength - 2f * OFFSET, 1f - PLATFORM_HEIGHT) / 2f;

            m_Spline.Clear();
            m_Spline.InsertPointAt(0, -OFFSET * Vector3.right);
            m_Spline.InsertPointAt(1, (-OFFSET + AdjustedLength) * Vector3.right);
            m_Spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            m_Spline.SetTangentMode(1, ShapeTangentMode.Continuous);

        }

        // Runs once every frame.
        protected virtual void Update() {
            m_Pressed = CheckPressed();

            // If the platform was just pressed.
            if (m_Pressed && !m_CachePressed) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
                m_SpriteShapeController.transform.localPosition = new Vector3(0f, 0f, 0f); 
                if (m_PressedShape != null) {
                    m_SpriteShapeController.spriteShape = m_PressedShape;
                }
            }
            // If the platform was just released.
            else if (m_CachePressed && !m_Pressed) {
                m_SpriteShapeController.transform.localPosition = new Vector3(0f, 1f/16f, 0f); 
                if (m_SpriteShapeController.spriteShape != m_DefaultShape) {
                    m_SpriteShapeController.spriteShape = m_DefaultShape;
                }
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
