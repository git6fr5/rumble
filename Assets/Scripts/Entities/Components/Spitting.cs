/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using Projectile = Platformer.Entities.Utility.Projectile;
using FiringPattern = Platformer.Entities.Utility.FiringPattern;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Spitting: MonoBehaviour {

        public enum SpitState {
            None,
            Tensing,
            Releasing,
        }

        // Whether this is bouncing or releasing.
        [SerializeField] 
        private SpitState m_SpitState = SpitState.None;        

        // The projectile that this thing fires.
        [SerializeField]
        private Projectile m_SpitProjectile = null;

        [SerializeField]
        private FiringPattern m_SpitPattern = null;

        [SerializeField]
        private Transform m_SpitBarrel = null;

        [SerializeField]
        private float m_SpitDelay = 0.5f;
        private Timer m_SpitTimer = new Timer(0f, 0f);
        
        void FixedUpdate() {
            bool finished = m_SpitTimer.TickDown(Time.fixedDeltaTime);

            // Whenever the crumble timer hits 0.
            if (finished) {

                switch (m_SpitState) {
                    case SpitState.Tensing:
                        OnSpit();
                        break;
                    default:
                        break;
                }
            }

        }

        public void OnStartSpitting() {
            if (m_SpitState != SpitState.None) { return; }

            m_SpitTimer.Start(m_SpitDelay);
            m_SpitState = SpitState.Tensing;

        }

        private void OnSpit() {
            m_SpitPattern.Fire(m_SpitProjectile, m_SpitBarrel);
            m_SpitState = SpitState.None;
        }

    }

}
