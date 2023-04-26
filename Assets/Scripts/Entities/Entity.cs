/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities {

    ///<summary>
    ///
    ///<summary>
    public class Entity : MonoBehaviour {

        
        // The amount of time before the platform registers that it has been pressed. 
        private const float PRESS_BUFFER = 0.06f;

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Hitbox = null;
        
        // The sprite shape renderer attached to this platform.
        protected SpriteRenderer m_SpriteRenderer = null;
        

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


        Vector3 m_Origin;
        Vector3 m_Size;
        Quaternion m_Orientation;



        void Awake() {
            m_Origin = transform.localPosition;
            m_Size = transform.localScale;
            m_Orientation = transform.localRotation;
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_PressedTimer.Stop();
        }

        //
        void FixedUpdate() {
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

        // 
        private void CheckPressedDown() {
            bool wasPressedDown = m_PressedDown;
            m_PressedDown = IsPressedDown();

            if (m_PressedDown) {
                // PressedDownEvent.Invoke();
            }
            
            m_OnPressedDown = !wasPressedDown && m_PressedDown && m_PressedTimer.Value == 0f;
            
            if (m_OnPressedDown) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
                m_PressedTimer.Start(PRESS_BUFFER);
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

        public void Disable() {
            Enable(false);
        }

        public void Enable(bool enable = true) {
            m_SpriteRenderer.enabled = enable;
            m_Hitbox.enabled = enable;
        }

        public void Reset() {
            transform.localPosition = m_Origin;
            transform.localScale = m_Size;
            transform.localRotation = m_Orientation;
        }

    }
}