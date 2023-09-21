/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Entity = Platformer.Entities.Entity;
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(Entity))]
    public class Platform : MonoBehaviour {

        #region Variables.

        /* --- Components --- */

        // The box collider attached to this platform.
        protected Entity m_Entity = null;
        public Entity entity => m_Entity;

        /* --- Parameters --- */

        // The position that this platform was spawned at.
        protected Vector3 m_Origin;
        public Vector3 Origin => m_Origin;

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

        // The animation that plays when the platform is being pressed.
        [SerializeField]
        private TransformAnimation m_OnPressedAnimation;
        // The current animation being played.
        protected TransformAnimation m_CurrentAnimation;

        #endregion

        #region Methods.

        // Used to cache references.
        void Awake() {
            m_Entity = GetComponent<Entity>();
        }

        // Initialize the platform.
        public void Start() {
            // Cache the origin.
            m_Origin = transform.position;

            // Collision settings.
            gameObject.layer = Game.Physics.CollisionLayers.PlatformLayer;

            // Release the platform as the default state.
            m_CurrentAnimation = null;
            m_Entity.Renderer.transform.Reset();
            OnReleased();

        }

        // Runs once every frame.
        void Update() {
            m_Pressed = CheckPressed(transform.position.y, m_Entity.CollisionContainer);

            // If the platform was just pressed.
            if (m_Pressed && !m_CachePressed) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
                OnPressed();
            }
            // If the platform was just released.
            else if (m_CachePressed && !m_Pressed) {
                OnReleased();
            }

            m_CachePressed = m_Pressed;
        }

        void FixedUpdate() {
            if (m_CurrentAnimation != null) {
                m_Entity.Renderer.transform.Animate(m_CurrentAnimation, Time.fixedDeltaTime);
                if (m_CurrentAnimation.AnimationTimer.Ratio >= 1f) {
                    m_CurrentAnimation = null;
                    m_Entity.Renderer.transform.Reset();
                }
            }
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

        // If the platform was just pressed.
        public void OnPressed() {
            m_CurrentAnimation = m_OnPressedAnimation;
            m_CurrentAnimation.AnimationTimer.Set(0f);
        }

        // If the platform was just released.
        public void OnReleased() {
            // m_SpriteShapeController.transform.localPosition = new Vector3(0f, RELEASED_OFFSET, 0f); 
        }

        #endregion



    }


}
