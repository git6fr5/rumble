/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Button : MonoBehaviour {

        // The box collider attached to this button.
        [SerializeField]
        public BoxCollider2D m_Collider => GetComponent<BoxCollider2D>(); 

        // The rect transform attached to this object.
        [SerializeField]
        public RectTransform m_RectTransform => GetComponent<RectTransform>();

        // Whether the button is being hovered over.
        [SerializeField, ReadOnly]
        private bool m_Hover = false;

        #region Methods.

        private void Start() {
            EditHitbox();
        }

        private void Update() {
            bool select = UnityEngine.Input.GetMouseButtonDown(0);
            if (select && m_Hover) {
                OnPress();
            }
        }

        private void OnMouseEnter() {
            m_Hover = true;
        }

        private void OnMouseExit() {
            m_Hover = false;
        }

        protected abstract void OnPress();

        private void EditHitbox() {
            Vector2 anchor = new Vector2(0.5f, 0.5f) - m_RectTransform.pivot;
            m_Collider.size = new Vector2(m_RectTransform.rect.width, m_RectTransform.rect.height);
            m_Collider.offset = new Vector2(anchor.x * m_Collider.size.x, anchor.y * m_Collider.size.y);
        }

        #endregion

    }

}