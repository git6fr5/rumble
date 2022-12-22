/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Input;
using Platformer.Character;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An abstract class defining the functionality of a
    /// character's action. 
    ///<summary>
    [System.Serializable]
    public abstract class CharacterAction {

        #region Fields
        
        /* --- Member Variables --- */
        
        // Checks whether the activation conditions have been fulfilled.
        [SerializeField, ReadOnly] 
        protected bool m_Refreshed;

        // Checks whether this ability is enabled.
        [SerializeField, ReadOnly] 
        protected bool m_Enabled;
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
        }

        #endregion

    }

}

