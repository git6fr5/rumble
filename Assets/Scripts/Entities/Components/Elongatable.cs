/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;
using Platformer.Entities.Utility;

namespace Platformer.Entities.Components {

    public class Elongatable : MonoBehaviour {

        public enum SearchDirection {
            Horizontal,
            Vertical
        }

        // The box collider.
        private BoxCollider2D m_BoxCollider;
        
        // The sprite shape.
        [SerializeField]
        private SpriteShapeController m_SpriteShapeController;
        public Spline spline => m_SpriteShapeController.spline;

        // Height.
        [SerializeField]
        private float m_ColliderHeight;

        // Offset.
        [SerializeField]
        private float m_ColliderVerticalOffset;

        // Offset.
        [SerializeField]
        private float m_ColliderHorizontalInset;

        [SerializeField]
        private int m_LengthUnits = 1;
        public int LengthUnits => m_LengthUnits;

        //
        [SerializeField]
        private SearchDirection m_SearchDirection;
        public SearchDirection searchDirection => m_SearchDirection; 

        void Start() {
            m_BoxCollider = GetComponent<BoxCollider2D>();
        }

        public void SetLength(int length) {
            m_LengthUnits = length;

            if (length < 0) {
                if (!Application.isPlaying) { DestroyImmediate(gameObject); }
                else { Destroy(gameObject); }
                return;
            }

            SetSpriteshapePoints(length);
            SetHitbox((float)length);


            ElongatableExtras extras = GetComponent<ElongatableExtras>();
            if (extras) {
                extras.SetLength(length, m_ColliderHeight);
            }

        }

        public void SetSpriteshapePoints(int length) {
            Spline spline = m_SpriteShapeController.spline;

            length -= 1;
            if (length <= 0) {
                return;
            }

            spline.Clear();

            Quaternion q = transform.localRotation;
            spline.InsertPointAt(0, Vector2.zero);
            spline.InsertPointAt(1, q * (length * Vector3.right));
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
            m_SpriteShapeController.gameObject.SetActive(true);

        }

        // Edits the hitbox of an obstacle
        public void SetHitbox(float length) {
            m_BoxCollider = GetComponent<BoxCollider2D>();
            if (m_BoxCollider == null) { return; }

            Quaternion q = transform.localRotation;

            Vector2 size = new Vector2(length - m_ColliderHorizontalInset, m_ColliderHeight);
            Vector2 offset = new Vector2((length - 1f) / 2f, m_ColliderVerticalOffset);

            if (transform.eulerAngles.z == 180f) {
                offset.x *= -1f;
            }

            if (m_SearchDirection == SearchDirection.Vertical) {
                size = new Vector2(size.y, size.x);
                offset = new Vector2(offset.y, offset.x);
            }

            m_BoxCollider.size = size;
            m_BoxCollider.offset = offset;
            
        }

    }

}