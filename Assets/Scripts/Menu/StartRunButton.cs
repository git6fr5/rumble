/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class StartRunButton : MonoBehaviour {

        // The box collider attached to this button.
        [SerializeField]
        public BoxCollider2D m_Collider => GetComponent<BoxCollider2D>(); 

        // The rect transform attached to this object.
        [SerializeField]
        public RectTransform m_RectTransform => GetComponent<RectTransform>();


        #region Methods.

        private void Start() {
            EditHitbox();
        }

        private void Update() {
            bool select = UnityEngine.Input.GetMouseButton(0);
            if (select && m_Hover) {
                StartRun();
            }
        }


        private void StartRun() {
            GameManager.SetLDtkData(m_LDtkData);
            Unity.SceneManagement.LoadScene(m_GameScene);
        }

        private void EditHitbox() {
            print(m_RectTransform.sizeDelta);
            m_Collider.size = new Vector2(m_RectTransform.rect.width, m_RectTransform.rect.height);
        }

        #endregion

    }

}