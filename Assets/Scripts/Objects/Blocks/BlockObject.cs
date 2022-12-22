/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Blocks {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BlockObject : MonoBehaviour {

        #region Variables.

        /* --- Components --- */
        
        // The box collider attached to this component.
        protected BoxCollider2D m_Hitbox => GetComponent<BoxCollider2D>();

        /* --- Members --- */

        // Whether the functionality of this block is currently active.
        [SerializeField]
        protected bool m_Active = false;
        public bool Active => m_Active;

        // Whether this block is currently being touched.
        [SerializeField] 
        protected bool m_Touched = false;
        
        // The origin around which this is centered.
        [SerializeField] 
        protected Vector3 m_Origin = Vector3.zero;

        // The sound that plays when this block is activated.
        [SerializeField] 
        private AudioClip m_ActivationSound;

        // The sound that plays when this block is deactivated.
        [SerializeField] 
        private AudioClip m_DeactivationSound;

        // The sound that plays when this block is touched.
        [SerializeField] 
        private AudioClip m_TouchedSound;

        // The effect that plays when this block is activated.
        [SerializeField] 
        private VisualEffect m_ActivationEffect;

        // The effect that plays when this block is deactivated.
        [SerializeField] 
        private VisualEffect m_DeactivationEffect;

        // The effect that plays when this block is touched.
        [SerializeField] 
        private VisualEffect m_TouchedEffect;

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

        // The functionality for when a block is touched.
        protected virtual void OnTouched(CharacterController character, bool touched) {
            if (touched) { 
                Game.Audio.Sounds.PlaySound(m_TouchedSound);
                Game.Visuals.Particles.PlayEffect(m_TouchedEffect);
            }
        }

        protected virtual bool CheckActivationCondition() {
            return false;
        }

        // The functionality for when a block is activated.
        protected virtual void OnActivation() {
            Game.Audio.Sounds.PlaySound(m_ActivationSound, 0.15f);
            Game.Visuals.Particles.PlayEffect(m_ActivationEffect);
        }

        // The functionality for when a block is deactivated.
        protected virtual void OnDeactivation() {
            Game.Audio.Sounds.PlaySound(m_DeactivationSound, 0.15f);
            Game.Visuals.Particles.PlayEffect(m_DeactivationEffect);
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
            BlockObject[] blocks = (BlockObject[])GameObject.FindObjectsOfType(typeof(BlockObject));
            for (int i = 0; i < blocks.Length; i++) {
                blocks[i].Reset();
            }
        }

    }

}