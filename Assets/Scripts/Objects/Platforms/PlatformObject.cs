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
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteShapeController))]
    public class PlatformObject : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before the platform registers that it has been pressed. 
        private const float PRESS_BUFFER = 0.06f;

        // The height of a platform.
        private const float PLATFORM_HEIGHT = 5f/16f;

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Hitbox = null;
        
        // The sprite shape renderer attached to this platform.
        protected SpriteShapeRenderer m_SpriteShapeRenderer = null;
        
        // The sprite shape controller. attached to this platform.
        protected SpriteShapeController m_SpriteShapeController = null;

        // The spline attached to the sprite shape.
        protected Spline m_Spline = null;

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
        private Timer m_PressedTimer = new Timer(0f, PRESS_BUFFER);

        // Whether this platform is being pressed down.
        [SerializeField, ReadOnly]
        protected bool m_PressedDown = false;

        // Whether this platform was just pressed down (being a little lax).
        [SerializeField, ReadOnly] 
        private bool m_OnPressedDown = false;

        // The sound to be played when this platform is pressed.
        [SerializeField] 
        private AudioClip m_OnPressedSound;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            // Set the position.
            m_Origin = transform.position;
            // Set the layer.
            gameObject.layer = LayerMask.NameToLayer("Objects");
            // Cache the components.
            m_SpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
            m_SpriteShapeController = GetComponent<SpriteShapeController>();
            m_Hitbox = GetComponent<BoxCollider2D>();
            // Set the rendering settings.
            m_SpriteShapeRenderer.sortingLayerName = Game.Visuals.RenderingLayers.PLATFORM_RENDERING_LAYER;
            m_SpriteShapeRenderer.sortingOrder = Game.Visuals.RenderingLayers.PLATFORM_RENDERING_ORDER;
            // Reset the pressed timer.
            m_PressedTimer.Stop();
        }

        // Initalizes from the LDtk files.
        public virtual void Init(int length, Vector3[] path) {
            m_Path = path;
            // Having to do this annoys me a little but oh well.
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_SpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
            m_SpriteShapeController = GetComponent<SpriteShapeController>();
            m_Spline = m_SpriteShapeController.spline;
            EditSpline(length);
            EditHitbox(length, PLATFORM_HEIGHT);
        }

        // Runs once every frame.
        protected virtual void Update() {
            bool wasPressedDown = m_PressedDown;
            m_PressedDown = IsPressedDown();
            
            m_OnPressedDown = !wasPressedDown && m_PressedDown && m_PressedTimer.Value == 0f;
            
            if (m_OnPressedDown) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
                m_PressedTimer.Start(PRESS_BUFFER);
            }

            m_PressedTimer.TickDown(Time.deltaTime);

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
            // Check if there is a character.
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Remove(character.transform);
            }
        }

        // Check if a character is standing on top of this.
        public bool IsPressedDown() {
            if (m_CollisionContainer.Count == 0) { return false; }

            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                CharacterController character = m_CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    // Check the the characters is in contact and above the obstacle.
                    Vector3 offset = (Vector3)character.Collider.offset; 
                    Vector3 height = Vector3.down * character.Collider.radius;
                    Vector3 position = m_CollisionContainer[i].position + offset + height;
                    // Can't be accessing physics settings from outside physics
                    // bool movingVertically = Mathf.Abs(controller.Body.velocity.y) < PhysicsSettings.MovementPrecision;
                    bool movingVertically = character.Falling || character.Rising;
                    if (position.y - transform.position.y > 0f && !movingVertically) {
                        return true;
                    }
                }
            }
            return false;
        }

        // Edits the spline of an platform.
        protected void EditSpline(float length) {
            m_Spline.Clear();
            m_Spline.InsertPointAt(0, Vector3.zero);
            m_Spline.InsertPointAt(1, length * Vector3.right);
            m_Spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            m_Spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        // Edits the spline of an platform.
        protected static void EditSpline(Spline spline, float length) {
            spline.Clear();
            spline.InsertPointAt(0, Vector3.zero);
            spline.InsertPointAt(1, length * Vector3.right);
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        // Edits the hitbox of the platform.
        protected void EditHitbox(float length, float height) {
            m_Hitbox.size = new Vector2(length, height);
            m_Hitbox.offset = new Vector2(length, 1f - height) / 2f;
        }

        #endregion

    }
}
