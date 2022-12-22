/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Decor;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class Rain : Sparkle {

        [SerializeField] protected float m_Speed = 0.75f;   
        
        protected override bool IsActive() {
            return Game.MainPlayer.Ghost.Enabled;
        }

        protected override void ResetTimer() {
            m_Ticks = m_Interval * Random.Range(0.5f, 1f);
        }

        protected override void AdjustPosition(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            Vector3 deltaPosition = Vector3.down * m_Speed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                m_Sparkles[i].transform.position += deltaPosition * 5f * (1f-m_Sparkles[i].GetComponent<SpriteRenderer>().color.a) + deltaTime * 0.4f * Vector3.right;
            }
        }

    }

}