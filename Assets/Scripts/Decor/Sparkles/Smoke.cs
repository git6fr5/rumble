/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class Smoke : Sparkle {

        [SerializeField] protected float m_Speed = 0.75f;   
        
        protected override bool IsActive() {
            return Game.MainPlayer.Hop.Enabled;
        }

        protected override void AdjustPosition(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            Vector3 deltaPosition = Vector3.up * m_Speed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                m_Sparkles[i].transform.position += deltaPosition;
            }
        }

    }

}