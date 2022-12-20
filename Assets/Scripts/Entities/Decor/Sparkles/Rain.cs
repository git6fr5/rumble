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
    public class Rain : Sparkle {

        [SerializeField] protected float m_Speed = 0.75f;   
        
        protected override bool IsActive() {
            return Game.MainPlayer.Ghost.Enabled;
        }

        protected override void ResetTimer() {
            m_Ticks = m_Interval * Random.Range(0.5f, 1f);
        }

        // protected override void AdjustRotation(float deltaTime) {
        //     m_Sparkles.RemoveAll(thing => thing == null);
        //     Vector3 deltaPosition = Vector3.down * m_Speed * deltaTime;
        //     for (int i = 0; i < m_Sparkles.Count; i++) {
        //         Vector3 v = deltaPosition * 5f * (1f-m_Sparkles[i].GetComponent<SpriteRenderer>().color.a) + deltaTime * 10f * Vector3.right;
        //         float angle = Vector2.SignedAngle(Vector2.down, (Vector2)deltaPosition);
        //         m_Sparkles[i].transform.eulerAngles = Vector3.forward * angle;
        //     }
        // }

        protected override void AdjustPosition(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            Vector3 deltaPosition = Vector3.down * m_Speed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                m_Sparkles[i].transform.position += deltaPosition * 5f * (1f-m_Sparkles[i].GetComponent<SpriteRenderer>().color.a) + deltaTime * 0.4f * Vector3.right;
            }
        }

    }

}