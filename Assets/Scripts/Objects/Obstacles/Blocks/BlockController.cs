/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Timer = Platformer.Utilities.Timer;
using CharacterController = Platformer.Character.CharacterController;
using ColorPalette = Platformer.Visuals.Rendering.ColorPalette;

namespace Platformer.Obstacles {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class GhostBlock : BlockController {

        #region Variables.

        /* --- Components --- */
        
        // The box collider attached to this component.
        private BoxCollider2D m_Hitbox => GetComponent<BoxCollider2D>();

        /* --- Members --- */

        // Whether this block is currently being touched.
        [SerializeField] 
        private bool m_Touched;
        
        // The origin around which this is centered.
        [SerializeField] 
        private Vector3 m_Origin;

        #endregion

        // Runs once before the first frame.
        void Start() {
            m_Origin = transform.position;
            Reset();
        }

        // Runs once every frame.
        void Update() {
            bool wasActive = m_Active;
            m_Active = CheckActivationCondition();
            if (m_Active && !wasActive) {
                OnActivation();
            }
            else if (!m_Active && wasActive) {
                OnDeactivation();
            }

            if (Active) {
                WhileActive();
            }
            else {
                WhileInactive();
            }
        }

        // Runs once when something enters this area.
        protected virtual void OnTriggerEnter2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                m_Touched = true;
                OnTouched(character, true);
            }
        }

        // Runs once when something leaves this area.
        protected virtual void OnTriggerExit2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                m_Touched = false;
                OnTouched(character, false);
            }
        }

        public virtual void OnTouched(CharacterController character, bool touched) {

        }

        protected virtual bool CheckActivationCondition() {
            return false;
        }

        protected virtual void OnActivation() {
            
        }

        protected virtual void OnDeactivation() {

        }

        // Runs while the block is released.
        protected virtual void WhileActive() {

        }

        protected virtual void WhileInactive() {
            
        }

        // Resets the block.
        public virtual void Reset() {
            transform.position = m_Origin;
        }

        // Resets all the ghost blocks in the scene.
        public static void ResetAll() {
            BlockController[] blocks = (BlockController[])GameObject.FindObjectsOfType(typeof(BlockController));
            for (int i = 0; i < blocks.Length; i++) {
                blocks[i].Reset();
            }
        }

    }

}