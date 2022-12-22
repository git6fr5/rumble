/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Decor; // For the particles.

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Timer = Platformer.Utilities.Timer;
using CharacterController = Platformer.Character.CharacterController;
using ColorPalette = Platformer.Visuals.Rendering.ColorPalette;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class RespawnBlock : BlockController {

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
        private AudioClip m_ActivateSound;

        [SerializeField] 
        private float m_FirstActivationTime = Mathf.Infinity;
        public float FirstActivationTime => m_FirstActivationTime;
        
        [SerializeField] 
        private float m_TotalActivatedTime = 0f;
        public float TotalTimeActive => m_TotalActivatedTime;

        [SerializeField] 
        private VisualEffect m_ActivationEffect;

        #endregion

        
        protected override bool CheckActivationCondition() {
            return Game.MainPlayer.Respawn == this;
        }

        protected override void OnTouched(CharacterController character, bool touched) {
            if (!touched) { return; }
            character.SetResetBlock(this);
        }

        protected override void OnActivation() {
            m_FirstActivationTime = Game.Ticks < m_FirstActivationTime ? Game.Ticks : m_FirstActivationTime;
            Game.Visuals.Particles.PlayEffect(m_ActivateEffect);
            Game.Audio.Sounds.PlaySound(m_ActivateSound, 0.15f);
        }

        private void FixedUpdate() {
            if (m_Active) {
                m_TotalActivatedTime += Time.fixedDeltaTime;
            }
        }
        
    }
}