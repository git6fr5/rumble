/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer
using Platformer.Objects.Spinners;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Spinners {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Collider2D))]
    public class SpinningColumn : SpinnerObject {

        #region Methods.

        // Initalizes from the LDtk files.
        public override void Init(float spin) {
            m_Hitbox.isTrigger = false;
            m_Spin = spin;
            foreach (Transform child in transform) {
                SpinnerObject spinObject = child.GetComponent<SpinnerObject>() ;
                if (spinObject != null) {
                    spinObject.Init(0f);
                }
            }        
        }

        #endregion

    }

}
