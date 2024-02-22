/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using Platformer;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    public class AttachOriginToSpriteShapePoint : MonoBehaviour {

        [SerializeField]
        private SpriteShapeController m_SpriteShapeController;

        [SerializeField]
        private int m_Index;

        [SerializeField]
        private Transform m_Scale;

        void Update() {

            if (m_Index >= m_SpriteShapeController.spline.GetPointCount()) {
                return;
            }

            Vector3 v =  m_SpriteShapeController.spline.GetPosition(m_Index);
            v.x *= m_Scale.localScale.x;
            v.y *= m_Scale.localScale.y;

            transform.position = m_SpriteShapeController.transform.position + (Quaternion.Euler(0f, 0f, m_SpriteShapeController.transform.eulerAngles.z) * v);

        }

    }

}