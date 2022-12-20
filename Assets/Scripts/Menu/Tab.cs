/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class Tab : MonoBehaviour {

        #region Fields.
        
        // Has to be parented to a menu.
        private Menu m_Menu => transform.parent.GetComponent<Menu>();

        // The submenu that this opens up.
        [SerializeField]
        private GameObject m_Submenu = null;

        // Whether the mouse is hovering over this tab.

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

        #endregion

    }

}