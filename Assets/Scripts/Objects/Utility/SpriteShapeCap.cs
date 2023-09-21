/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

namespace Platformer.Objects {

    ///<summary>
    ///
    ///<summary>
    public class SpriteShapeCap : MonoBehaviour {

        [SerializeField]
        private Obstacle m_Obstacle;
        public Obstacle obstacle {
            get { return m_Obstacle; }
            set { m_Obstacle = value; }
        }

        // Runs when something collides with this platform.
        private void OnCollisionEnter2D(Collision2D collision) {
            m_Obstacle.OnCollisionEnter2D(collision);
        }

        // Runs when something exit this platform.
        private void OnCollisionExit2D(Collision2D collision) {
            m_Obstacle.OnCollisionExit2D(collision);            
        }

    }

}