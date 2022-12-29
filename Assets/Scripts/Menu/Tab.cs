/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class Tab : MonoBehaviour {

        #region Fields.
        
        // Has to be parented to a menu.
        private Menu m_Menu => transform.parent.GetComponent<Menu>();

        // The text attached to the tab.
        [SerializeField]
        private Text m_Text;

        // The submenu that this opens up.
        [SerializeField]
        private GameObject m_Submenu = null;

        #endregion

        #region Methods

        private void Start() {
            // BoxCollider2D box = AddComponent<BoxCollider2D>();
        }

        private void OnMouseEnter() {
            m_Menu.SetHoverTab(this);
        }

        private void OnMouseExit() {
            m_Menu.EndHoverTab(this);
        }

        public void Open() {
            m_Menu.SetSelectedTab(this);
            if (m_Submenu != null) {
                m_Submenu.SetActive(true);
            }
        }

        public void Close() {
            if (m_Submenu != null) {
                m_Submenu.SetActive(false);
            }
        }

        public void SetText(string text) {
            if (m_Text == null) { return; }
            m_Text.text = text.ToUpper();
            m_Text.ReduceUntilFitsHorizontally(m_Text.fontSize);
        }

        #endregion

    }

}