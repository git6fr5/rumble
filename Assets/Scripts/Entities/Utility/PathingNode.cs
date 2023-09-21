/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    public class PathingNode : MonoBehaviour {

        private Vector3 m_Position;
        public Vector3 Position => m_Position;

        void Awake() {
            m_Position = transform.position;
        }

    }

}
