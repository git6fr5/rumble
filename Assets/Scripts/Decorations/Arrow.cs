/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    /// Decorates a level with grass.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Arrow : MonoBehaviour {

        // The period by which this animates.
        public const float PERIOD = 1f;

        // The amplitude that this oscillates with.
        public const float AMPLITUDE = 0.1f;

        void Update() {
            transform.localScale = (Mathf.Sin(PERIOD * Game.Physics.Time.Ticks) * AMPLITUDE + 1f - AMPLITUDE) * new Vector3(1f, 1f, 1f);
        }

    }

}
