/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.Entities.Components {

    public class Elongatable : MonoBehaviour {

        [System.Serializable]
        public class OverrideLength {
            public int length;
            public GameObject gameObject;
        }

        // Overrides.
        [SerializeField]
        private OverrideLength[] m_Overrides;

        // The box collider.
        private BoxCollider2D m_BoxCollider;
        
        // The sprite shape.
        [SerializeField]
        private SpriteShapeController m_SpriteShapeController;

        // Height.
        [SerializeField]
        private float m_ColliderHeight;

        // Offset.
        [SerializeField]
        private float m_ColliderVerticalOffset;

        void Start() {
            m_BoxCollider = GetComponent<BoxCollider2D>();
        }

        public void SetLength(int length) {
            if (length < 0) {
                if (!Application.isPlaying) { DestroyImmediate(gameObject); }
                else { Destroy(gameObject); }
                return;
            }
            SetSpriteshapePoints(length);
            SetHitbox((float)length);
        }

        public bool SetSpriteshapePoints(int length) {

            Spline spline = m_SpriteShapeController.spline;

            // In the special case that the length of this is 0 or less.
            // if (length <= 2) { return false; }

            m_SpriteShapeController.gameObject.SetActive(false);
            for (int i = 0; i < m_Overrides.Length; i++) {
                m_Overrides[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < m_Overrides.Length; i++) {
                if (length == m_Overrides[i].length) {
                    m_Overrides[i].gameObject.SetActive(true);
                    return false;
                }
            }

            length -= 2;
            spline.Clear();
            spline.InsertPointAt(0, 0.5f * Vector3.right);
            spline.InsertPointAt(1, (length + 0.5f) * Vector3.right);
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
            m_SpriteShapeController.gameObject.SetActive(true);

            return true;

        }

        // Edits the hitbox of an obstacle
        public void SetHitbox(float length) {
            m_BoxCollider = GetComponent<BoxCollider2D>();
            if (m_BoxCollider == null) { return; }

            m_BoxCollider.size = new Vector2(length - 0.3f, m_ColliderHeight);
            m_BoxCollider.offset = new Vector2((length - 1f) / 2f, m_ColliderVerticalOffset);
        }

    }

}