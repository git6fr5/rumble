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
    public class DecorationPiece : MonoBehaviour {

        // The idle transform movement.
        [SerializeField]
        private TransformAnimation m_IdleTransformMovement = null;
        public TransformAnimation Animation => m_IdleTransformMovement;

        // The sprite renderer component attached to this.        
        private SpriteRenderer m_SpriteRenderer = null;
        public SpriteRenderer spriteRenderer => m_SpriteRenderer;

        // Runs once on instantiation.
        void Awake() {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }


    }

}
