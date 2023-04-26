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
    public class Bud : MonoBehaviour {

        #region Variables.

        /* --- Components --- */
        
        // The idle transform movement.
        [SerializeField]
        private TransformAnimation m_OpeningAnimation = null;

        #endregion

        public void Grow(float percent) {
            transform.SetAnimation(m_OpeningAnimation, percent);
        }

    }

}
