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
    public class Creepy : Sparkle {

        [SerializeField] protected float m_Speed = 5f;   
        
        protected override bool IsActive() {
            return Game.MainPlayer.Shadow.Enabled;
        }

        protected override void AdjustPosition(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            for (int i = 0; i < m_Sparkles.Count; i++) {
                float speed = m_Sparkles[i].GetComponent<SpriteRenderer>().color.a * m_Speed / 2f + m_Speed / 2f;
                Vector3 deltaPosition = ((Vector3)(Vector2)Camera.main.transform.position-m_Sparkles[i].transform.position).normalized * speed * deltaTime;
                m_Sparkles[i].transform.position += deltaPosition;
                float distance = ((Vector2)(Camera.main.transform.position-m_Sparkles[i].transform.position)).magnitude;
                if (distance < 0.25f) {
                    Destroy(m_Sparkles[i].gameObject);
                }
            }
        }

    }

}

