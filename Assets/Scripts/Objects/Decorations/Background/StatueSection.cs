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

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class StatueSection : MonoBehaviour {

        #region Variables.

        // The idle transform movement.
        [SerializeField]
        private TransformAnimation m_IdleTransformMovement = null;

        [SerializeField]
        private SpriteRenderer m_SpriteRenderer = null;

        #endregion

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_SpriteRenderer.transform.Animate(m_IdleTransformMovement, Time.fixedDeltaTime);
        }

    }

}
