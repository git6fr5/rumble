/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using Platformer.Utilities;
using Platformer.Obstacles;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteShapeController))]
    public class Platform : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before the platform registers that it has been pressed. 
        private const float PRESS_BUFFER = 0.06f;

        // The height of a platform.
        private const float PLATFORM_HEIGHT = 5f/16f;

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Hitbox => GetComponent<BoxCollider2D>();
        
        // The sprite shape renderer attached to this platform.
        protected SpriteShapeRenderer m_SpriteShapeRenderer => GetComponent<SpriteShapeRenderer>();
        
        // The sprite shape controller. attached to this platform.
        protected SpriteShapeController m_SpriteShapeController => GetComponent<SpriteShapeController>();

        // The spline attached to the sprite shape.
        protected Spline m_Spline => m_SpriteShapeController.spline;

        /* --- Members --- */

        // The position that this platform was spawned at.
        [HideInInspector]
        protected Vector3 m_Origin;
        
        // The path that this platform follows.
        [HideInInspector] 
        protected Vector3[] m_Path = null;
        
        // The current position in the path that the path is following.
        [SerializeField, ReadOnly] 
        protected int m_PathIndex;
        
        // The objects that are attached to the platform.
        [SerializeField, ReadOnly] 
        protected List<Transform> m_CollisionContainer = new List<Transform>();
        
        // The timer that checks how recently the platform has been pressed.
        [SerializeField, ReadOnly] 
        private float m_PressedTimer = new Timer(0f, PRESS_BUFFER);
        
        // Whether this platform was just pressed down (being a little lax).
        [SerializeField, ReadOnly] 
        private bool m_OnPressedDown;

        // The sound to be played when this platform is pressed.
        [SerializeField] 
        private AudioClip m_OnPressedSound;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            m_Origin = transform.position;
            gameObject.layer = Game.Physics.CollisionLayers.Platform;
            m_SpriteShapeRenderer.sortingLayerName = Game.Camera.Visuals.RenderingLayers.Foreground;
        }

        // Initalizes from the LDtk files.
        public virtual void Init(int length, Vector3[] path) {
            m_Path = path;
            EditSpline(m_SpriteShapeController.spline, length);
            EditHitbox(m_Hitbox, length, PLATFORM_HEIGHT);
        }

        // Runs once every frame.
        protected virtual void Update() {
            bool pressedOnPrevFrame = m_PressedDown;
            bool pressedThisFrame = IsPressedDown(transform.position, m_CollisionContainer, ref m_PressedDown);

            m_OnPressedDown = !pressedOnPrevFrame && pressedThisFrame && m_PressedTimer.Ratio == 1f;
            if (m_OnPressedDown) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
            }
            m_PressedTimer.TriangleTickDownIf(Time.deltaTime, pressedThisFrame);

        }

        // Runs when something collides with this platform.
        private void OnCollisionEnter2D(Collision2D collision) {
            // Check if there is a character.
            CharacterController character = collision.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            Transform transform = character.transform; 
            if (!container.Contains(transform)) {
                container.Add(transform);
            }
            
        }

        // Runs when something exit this platform.
        private void OnCollisionExit2D(Collision2D collision) {
            // Check if there is a character.
            CharacterController character = collision.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            Transform transform = character.transform; 
            if (container.Contains(transform)) {
                container.Remove(transform);
            }
        }

        // Check if a character is standing on top of this.
        public bool IsPressedDown() {
            bool pressedDown = false;
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    // Check the the characters is in contact and above the obstacle.
                    Vector3 offset = (Vector3)character.Collider.offset; 
                    Vector3 height = Vector3.down * character.Collider.radius;
                    Vector3 position = m_CollisionContainer[i].position + offset + height;
                    // Can't be accessing physics settings from outside physics
                    // bool movingVertically = Mathf.Abs(controller.Body.velocity.y) < PhysicsSettings.MovementPrecision;
                    bool movingVertically = Game.Physics.IsMovingVertically(character.Body);
                    if (position.y - transform.position.y > 0f && movingVertically) {
                        pressedDown = true;
                        return;
                    }
                }
            }
            return pressedDown;
        }

        // Edits the spline of an platform.
        public static void EditSpline(float length) {
            m_Spline.Clear();
            m_Spline.InsertPointAt(0, new Vector3(-0.5f, 0f, 0f));
            m_Spline.InsertPointAt(1, length * Vector3.right + new Vector3(-0.5f, 0f, 0f));
            m_Spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            m_Spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        // Edits the hitbox of the platform.
        public void EditHitbox(float length, float height) {
            m_Hitbox.size = new Vector2(length, height);
            m_Hitbox.offset = new Vector2(length - 1f, 1f - height) / 2f;
        }

        #endregion

    }
}