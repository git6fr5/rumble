/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Gobblefish.Input;
using Platformer.Character;

namespace Platformer.Character {

    ///<summary>
    /// An abstract class defining the functionality of a
    /// character's action. 
    ///<summary>
    [System.Serializable]
    public abstract class CharacterAction : ScriptableObject {

        // The increment with which to notify charge.
        public const float CHARGE_INCREMENT = 0.1f;

        #region Enumerations

        public enum ActionPhase {
            None,
            PreAction,
            MidAction,
            PostAction
        }

        #endregion

        #region Fields
        
        /* --- Member Variables --- */

        // Checks whether the activation conditions have been fulfilled.
        [SerializeField, ReadOnly] 
        protected ActionPhase m_ActionPhase = ActionPhase.None;
        
        // Checks whether the activation conditions have been fulfilled.
        [SerializeField, ReadOnly] 
        protected bool m_Refreshed = false;

        // Checks whether this ability is enabled.
        [SerializeField] 
        protected bool m_Enabled = false;
        public bool Enabled => m_Enabled;

        #endregion

        #region Methods
    
        // When this ability is activated.
        public abstract void InputUpdate(CharacterController character);
        
        // Refreshes the settings for this ability every interval.
        public abstract void PhysicsUpdate(CharacterController character, float dt);

        // Enables the action.
        public virtual void Enable(CharacterController character, bool enable = true) {
            m_Enabled = enable;
            m_Refreshed = enable;
        }

        public void EnablePower(CharacterController character, bool enable, Sprite sprite) {
            Enable(character, enable);
            if (sprite != null && enable) {
                character.Animator.SetBody(sprite);
            }
        }

        #endregion

    }

}

