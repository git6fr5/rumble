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
using Spring = Platformer.Objects.Decorations.Spring;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformObject : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before the platform registers that it has been pressed. 
        private const float PRESS_BUFFER = 0.06f;

        // The height of a platform.
        private const float PLATFORM_HEIGHT = 5f/16f;

        // The outcropping width for the border of the platform.
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

        //
        [SerializeField] 
        private Spring[] m_Springs = null;

        //
        [SerializeField] 
        SpriteShape m_PressedShape = null;
        SpriteShape m_DefaultShape = null;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            // Collision settings.
            m_Hitbox = GetComponent<BoxCollider2D>();
            gameObject.layer = LayerMask.NameToLayer("Objects");

            // Rendering settings.
            m_SpriteShapeRenderer.sortingLayerName = Game.Visuals.RenderingLayers.PLATFORM_RENDERING_LAYER;
            m_SpriteShapeRenderer.sortingOrder = Game.Visuals.RenderingLayers.PLATFORM_RENDERING_ORDER;
            // transform.position.z += Game.Visuals.Rendering.Saturation;

            // Other.
            m_Origin = transform.position;
            m_DefaultShape = m_SpriteShapeController.spriteShape;
            m_Spline = m_SpriteShapeController.spline;
            m_PressedTimer.Stop();
        }

        // Initalizes from the LDtk files.
        public virtual void Init(int length, Vector3[] path) {
            m_Path = path;
            // Having to do this annoys me a little but oh well.
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_Spline = m_SpriteShapeController.spline;

            // Cut two pixels off the right.
            float _length = (float)length - 2f * (0.5f - OFFSET); 
            EditSpline(_length);
            EditHitbox(_length, PLATFORM_HEIGHT);

            Invoke("ActivateSprings", 0.04f);

        }

        private void ActivateSprings() {

            float distanceToGround = Game.Physics.Collisions.DistanceToFirst(transform.position + Vector3.down * 0.25f, Vector3.down, Game.Physics.CollisionLayers.Ground);
            distanceToGround = Mathf.Min(distanceToGround, 20f);

            Vector2 offset = Vector2.zero;
            for (int i = 0; i < m_Springs.Length; i++) {
                offset.x = m_Hitbox.offset.x + Random.Range(-0.5f, 0.5f);
                m_Springs[i].Activate(offset, distanceToGround + 0.5f, Vector3.down);
            }

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

            if (m_PressedDown && m_PressedShape != null) {
                m_SpriteShapeController.spriteShape = m_PressedShape;
            }
            else if (!m_PressedDown && m_SpriteShapeController.spriteShape != m_DefaultShape) {
                m_SpriteShapeController.spriteShape = m_DefaultShape;
            }

            AdjustPosition();

        }

        protected void AdjustPosition() {
            if (!m_PressedDown) {
                m_SpriteShapeController.transform.localPosition = new Vector3(0f, 1f/16f, 0f); 
            }
            else {
                m_SpriteShapeController.transform.localPosition = new Vector3(0f, 0f, 0f); 
            }
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
            EditSpline(m_Spline, length);
        }

        // Edits the spline of an platform.
        protected static void EditSpline(Spline spline, float length) {
            spline.Clear();
            spline.InsertPointAt(0, -OFFSET * Vector3.right);
            spline.InsertPointAt(1, (-OFFSET + length) * Vector3.right);
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        // Edits the hitbox of the platform.
        protected void EditHitbox(float length, float height) {
            m_Hitbox.size = new Vector2(length, height);
            m_Hitbox.offset = new Vector2(length - 2f * OFFSET, 1f - height) / 2f;
        }

        #endregion

    }
}
