/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game = Platformer.Management.GameManager;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public class CollisionBallTester : MonoBehaviour {

        public LayerMask m_Mask;

        public GameObject m_Object;

        public Vector2 m_Direction = Vector2.right;

        /* --- Unity --- */
        #region Unity
        
        // Runs once before the first frame.
        void Start() {
            
        }
        
        // Runs once every frame.
        void FixedUpdate() {
            m_Object = Game.Physics.Collisions.ILineOfSight<MonoBehaviour>(transform.position, m_Direction, m_Mask, 50f);
        }

        void OnDrawGizmosSelected() {
            // Gizmos.color = Gizmos.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)m_Direction * 50f);
        }
        
        #endregion

    }
}