/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilities;
using Platformer.Decor;
using Platformer.Obstacles;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class Hearts : Sparkle {

        [SerializeField] protected float m_Speed = 0.75f;   
        [SerializeField] private RespawnBlock m_RespawnBlock;
        
        protected override bool IsActive() {
            return Game.MainPlayer.Respawn == m_RespawnBlock;
        }

        protected override void AdjustPosition(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            Vector3 deltaPosition = Vector3.up * m_Speed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                // Vector3 direction = (m_Sparkles[i].transform.position - transform.position).normalized;
                m_Sparkles[i].transform.position += deltaPosition;
                // m_Sparkles[i].transform.localScale *= 0.975f;
            }
        }

    }

}