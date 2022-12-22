/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Timer = Platformer.Utilities.Timer;
using CharacterController = Platformer.Character.CharacterController;
using ColorPalette = Platformer.Visuals.Rendering.ColorPalette;

namespace Platformer.Obstacles {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class GhostBlock : BlockController {

        #region Variables.

        /* --- Constants --- */

        // The amount of friction the ghost block experiences while active.
        public const float FRICTION = 0.01f;

        // The amount of drag the ghost block experiences while active.
        public const float DRAG = 0.05f;

        // The amount of mass the block has (for collisions).
        public const float MASS = 0.7f;

        /* --- Components --- */
        
        // The rigidbody attached to this component.
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        
        #endregion

        #region Methods.

        protected override bool CheckActivationCondition() {
            return Game.MainPlayer.Ghost.Enabled;
        }

        protected override void OnDeactivation() {
            m_Body.Freeze();
        }

        // Runs while the block is released.
        protected override void WhileActive() {
            if (m_Touched) {
                m_Body.ReleaseXY();
            }
            else {
                m_Body.ReleaseAll();
            }
            m_Body.Slowdown(1f - FRICTION);
        }

        // Resets the block.
        public override void Reset() {
            transform.eulerAngles = Vector3.zero;
            m_Body.SetAngularDrag(DRAG);
            m_Body.SetWeight(0f, MASS);
            m_Body.Freeze();
            base.Reset();
        }

        #endregion

    }

}