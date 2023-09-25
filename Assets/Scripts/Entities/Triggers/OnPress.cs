/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Entity = Platformer.Entities.Entity;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Triggers {

    [DefaultExecutionOrder(1000), RequireComponent(typeof(Entity))]
    public class OnPress : MonoBehaviour {

        [HideInInspector]
        private Entity m_Entity;

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

        // The sound to be played when this platform is released.
        [SerializeField] 
        private AudioClip m_OnReleasedSound;

        [SerializeField]
        private UnityEvent m_PressEvent;

        [SerializeField]
        private UnityEvent m_ReleaseEvent;

        // Runs once on instantiation.
        void Awake() {
            m_Entity = GetComponent<Entity>();
            m_Entity.SetAsTrigger(false);
        }

        // Runs once every frame.
        void Update() {
            m_Pressed = CheckPressed(transform.position.y, m_Entity.CollisionContainer);

            if (m_Pressed && !m_CachePressed) {
                Game.Audio.Sounds.PlaySound(m_OnPressedSound, 0.15f);
                m_PressEvent.Invoke();
            }
            else if (m_CachePressed && !m_Pressed) {
                Game.Audio.Sounds.PlaySound(m_OnReleasedSound, 0.15f);
                m_ReleaseEvent.Invoke();
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

    }


}
