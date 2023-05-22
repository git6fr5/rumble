/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Orbs;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;
using Plant = Platformer.Objects.Decorations.Plant;
using AnimationController = Platformer.Visuals.Animation.AnimationController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    ///
    ///<summary>
    public class RespawnOrb : OrbObject {

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

        // [SerializeField]
        // private Plant m_Plant;

        [SerializeField]
        private AnimationController m_Animation;

        [SerializeField]
        private Light2D m_Light;

        private bool m_Active = false;

        #endregion

        void Start() {
            if (Game.MainPlayer.RespawnOrb == this) {
                Activate();
            }
            else {
                Deactivate();
            }
        }

        protected override void OnTouch(CharacterController character) {
            base.OnTouch(character);
            character.SetRespawn(this);
        }

        public void Activate() {
            m_Active = true;
            
            if (m_Light != null) {
                m_Light.enabled = true;
            }


            if (m_TotalActivatedTime == 0f) {
                m_FirstActivationTime = Game.Physics.Time.Ticks; // Game.Physics.Time.Ticks < m_FirstActivationTime ? Game.Physics.Time.Ticks : m_FirstActivationTime;
                m_Animation.Play(true);
            }
            
        }

        public void Deactivate() {
            if (m_Light != null) {
                m_Light.enabled = false;
            }

            m_Active = false;
        }

        private void FixedUpdate() {
            if (m_Active) {
                m_TotalActivatedTime += Time.fixedDeltaTime;
            }
        }
        
    }
}