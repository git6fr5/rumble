/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer;
using Platformer.Utilites;
using Platformer.Obstacles;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class AlternatingPlatform : Platform {

        public enum Beat {
            A, B
        }

        [SerializeField] private Beat m_Beat;
        private static float Period = 6f;

        void FixedUpdate() {
            
            float t = Game.Ticks % Period;
            bool enableA = t < Period / 2f;
            bool change = (Game.Ticks % (Period * 0.5f)) > 0.5f * Period - 0.5f;

            Color color = Screen.ForegroundColorShift;
            if (m_Beat == Beat.A) {
                color *= enableA ? 1f : 0.25f;
                m_SpriteShapeRenderer.color = color;
                m_Hitbox.enabled = enableA;
            }

            if (m_Beat == Beat.B) {
                color *= !enableA ? 1f : 0.25f;
                m_SpriteShapeRenderer.color = color;
                m_Hitbox.enabled = !enableA;
            }

            if (change) {
                transform.position = m_Origin - Vector3.down * 2f/16f;
            }
            else {
                transform.position = m_Origin;
            }
            
        }

    }

}