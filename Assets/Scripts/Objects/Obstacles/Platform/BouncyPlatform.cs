/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Character;
using Platformer.Utilities;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class BouncyPlatform : Platform {

        #region Variables.

        // Tracks whether this platform is currently bouncing or not.
        [SerializeField, ReadOnly] 
        private bool m_Bouncing = false;
        
        // The amount of additional jump speed given by the platform.
        [SerializeField] 
        private float m_JumpSpeed = 20f;
        
        // The speed at which the platform sinks.
        [SerializeField] 
        private float m_SinkSpeed = 5f;
        
        // The distance the bouncy platform sinks
        [SerializeField] 
        private float m_SinkDistance = 0.4f;

        // The sound played to indicate when to bounce.
        [SerializeField] 
        private AudioClip m_BounceSound;

        #endregion

        #region Methods.

        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
            base.Update();
            m_Bouncing = m_PressedDown ? true : m_Bouncing;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            Vector3 apex = m_Origin + Vector3.down * m_SinkDistance;
            
            if (m_Bouncing) {
                transform.Move(apex, m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);
                float distance = (transform.position - apex).magnitude;
                if (distance == 0f) {
                    Game.Audio.Sounds.PlaySound(m_BounceSound, 0.15f);
                    BounceBodies();
                    m_Bouncing = false;
                }
            }
            else {
                transform.Move(m_Origin, m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);
            }

        }

        // Bounces all the characters in the container.
        private void BounceBodies() {
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                Rigidbody2D body = m_CollisionContainer[i].GetComponent<Rigidbody>();
                if (body != null) {
                    body.AddVelocity(new Vector2(0f, m_JumpSpeed));
                }
            }
        }

        #endregion

    }
}