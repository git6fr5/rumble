/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Spikes {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class Spikeball : SpikeObject {

        // The speed with which this rotates.
        private const float ROTATION_SPEED = 90f;

        protected virtual void FixedUpdate() {
            transform.Rotate(ROTATION_SPEED, Time.fixedDeltaTime);
        }

    }

}
