/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Input;
using Platformer.Utilities;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An abstract class defining the functionality of a
    /// character's physics. 
    ///<summary>
    [System.Serializable]
    public abstract class PhysicsAction {

        // Process the physics of this action.
        public abstract void Process(Rigidbody2D body, InputSystem input, CharacterController state, float dt);

    }
}