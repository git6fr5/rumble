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

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public class Obstacle : MonoBehaviour {

        // The objects that are attached to the platform.
        [SerializeField, ReadOnly] 
        protected List<Transform> m_CollisionContainer = new List<Transform>();
        public List<Transform> CollisionContainer => m_CollisionContainer;

        [SerializeField, ReadOnly]
        private List<Collider2D> m_Colliders = new List<Collider2D>();
        public void AddCollider(Collider2D collider2D) { 
            if (!m_Colliders.Contains(collider2D)) { 
                m_Colliders.Add(collider2D);
            } 
        }
        public void RemoveCollider(Collider2D collider2D) { 
            if (m_Colliders.Contains(collider2D)) { 
                m_Colliders.Remove(collider2D); 
            }
        }
        public void EnableColliders(bool enable) {
            for (int i = 0; i < m_Colliders.Count; i++) {
                m_Colliders[i].enabled = enable;
            }
        }
        public bool AllCollidersEnabled(bool enabled) {
            for (int i = 0; i < m_Colliders.Count; i++) {
                if (m_Colliders[i].enabled != enabled) {
                    return false;
                }
            }
            return true;
        }

        // Runs when something collides with this platform.
        public void OnCollisionEnter2D(Collision2D collision) {
            print("on collision enter");

            // Check if there is a character.
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (!m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Add(character.transform);
            }
            
        }

        // Runs when something exit this platform.
        public void OnCollisionExit2D(Collision2D collision) {
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Remove(character.transform);
            }
        }

    }

}