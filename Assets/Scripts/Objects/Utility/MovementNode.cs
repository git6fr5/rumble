/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects {

    ///<summary>
    ///
    ///<summary>
    public class MovementNode : MonoBehaviour {

        [HideInInspector]
        public Vector3 position;
        void Awake() {
            position = transform.position;
        }

    }

}
