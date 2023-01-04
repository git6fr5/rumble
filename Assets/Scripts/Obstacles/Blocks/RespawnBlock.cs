/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Blocks;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Blocks {

    ///<summary>
    ///
    ///<summary>
    public class RespawnBlock : BlockObject {

        #region Variables.

        /* --- Constants --- */

        // The amount above the respawn block that this respawns people at.
        public const float RESPAWN_HEIGHT = 1.5f;

        // The amount of time before something is respawned.
        public const float RESPAWN_DELAY = 0.12f;

        /* --- Members --- */

        // The position at which things are respawned for this block.
        public Vector3 RespawnPosition => transform.position + Vector3.up * RESPAWN_HEIGHT;

        [SerializeField] 
        private float m_FirstActivationTime = Mathf.Infinity;
        public float FirstActivationTime => m_FirstActivationTime;
        
        [SerializeField] 
        private float m_TotalActivatedTime = 0f;
        public float TotalTimeActive => m_TotalActivatedTime;

        #endregion

        
        protected override bool CheckActivationCondition() {
            return Game.MainPlayer.RespawnBlock == this;
        }

        protected override void OnTouched(CharacterController character, bool touched) {
            base.OnTouched(character, touched);
            if (touched) {
                character.SetResetBlock(this);
            }
        }

        protected override void OnActivation() {
            base.OnActivation();
            m_FirstActivationTime = Game.Physics.Time.Ticks < m_FirstActivationTime ? Game.Physics.Time.Ticks : m_FirstActivationTime;
            
        }

        private void FixedUpdate() {
            if (m_Active) {
                m_TotalActivatedTime += Time.fixedDeltaTime;
            }
        }
        
    }
}