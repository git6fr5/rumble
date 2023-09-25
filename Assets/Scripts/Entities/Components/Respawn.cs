/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Entity = Platformer.Entities.Entity;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Respawn : MonoBehaviour {

        // The amount above the respawn block that this respawns people at.
        public const float RESPAWN_HEIGHT = 1.5f;

        // The amount of time before something is respawned.
        public const float RESPAWN_DELAY = 0.12f;

        // The position at which things are respawned for this block.
        public Vector3 RespawnPosition => transform.position + Vector3.up * RESPAWN_HEIGHT;

        [SerializeField, ReadOnly] 
        private float m_FirstActivationTime = Mathf.Infinity;
        public float FirstActivationTime => m_FirstActivationTime;

        [SerializeField] 
        private float m_TotalActivatedTime = 0f;
        public float TotalTimeActive => m_TotalActivatedTime;

        [SerializeField]
        private VisualEffect m_EmissionParticle;

        [SerializeField, ReadOnly]
        private bool m_Active = false;

        // Runs once before the first frame.
        void Start() {
            if (Game.MainPlayer.CurrentRespawn == this) {
                Activate();
            }
            else {
                Deactivate();
            }
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            if (m_Active) {
                m_TotalActivatedTime += Time.fixedDeltaTime;
            }
        }

        public void SetRespawnPoint() {
            CharacterController character = Game.MainPlayer;
            character.SetRespawn(this);
        }

        public void Activate() {
            print("activating");
            m_Active = true;
            if (m_EmissionParticle != null) {
                m_EmissionParticle.Play();
            }
            if (m_TotalActivatedTime == 0f) {
                m_FirstActivationTime = Game.Physics.Time.Ticks;
            }
            
        }

        public void Deactivate() {
            m_Active = false;
            if (m_EmissionParticle != null) {
                m_EmissionParticle.Stop();
            }        
        }
        
    }
}