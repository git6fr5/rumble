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
    public class Menu : MonoBehaviour {

        #region Fields.

        // The accessible tabs in this menu.
        [SerializeField, ReadOnly] 
        private List<Tab> m_Tabs = new List<Tab>();

        // The tab that is currently being hovered over.
        [SerializeField, ReadOnly] 
        private Tab m_HoverTab = null;

        // The tab that is currently selected.
        [SerializeField, ReadOnly] 
        private Tab m_SelectedTab = null;
        public Tab SelectedTab => m_SelectedTab;

        // The tab that is currently being indicated.
        [SerializeField] 
        private Transform m_HoverTabIndicator = null;

        // The tab that is currently selected.
        [SerializeField] 
        private Transform m_SelectedTabIndicator = null;

        #endregion

        #region Methods.

        // Runs once before the first frame,
        private void Start() {
            GetAllTabs();
            OpenTab(0);
        }

        // Runs once per frame.
        private void Update() {
            if (m_HoverTab == null || m_HoverTab == m_SelectedTab) {
                m_HoverTabIndicator.gameObject.SetActive(false);
            }
            else {
                m_HoverTabIndicator.gameObject.SetActive(true);
            }

            bool select = UnityEngine.Input.GetMouseButton(0);
            if (select && m_HoverTab != null) {
                m_HoverTab.Open();
            }

        }

        // Gets all the children that are tabs.
        private void GetAllTabs() {
            m_Tabs = new List<Tab>();
            foreach (Transform child in transform) {
                Tab tab = child.GetComponent<Tab>();
                if (tab != null) {
                    m_Tabs.Add(tab);
                }
            }
        }

        // Opens the tab by index.
        private void OpenTab(int index) {
            if (m_Tabs == null || m_Tabs.Count == 0) { return; }
            index = index % m_Tabs.Count;
            for (int i = 0; i < m_Tabs.Count; i++) {
                m_Tabs[i].Close();
            }
            m_Tabs[index].Open();
        }

        // Closes all the tabs.
        public void CloseAllTabs() {
            if (m_Tabs == null || m_Tabs.Count == 0) { return; }
            for (int i = 0; i < m_Tabs.Count; i++) {
                m_Tabs[i].Close();
            }
        }

        // Sets the selected tab
        public void SetSelectedTab(Tab tab) {
            if (m_SelectedTab != null) {
                m_SelectedTab.Close();
            }
            m_SelectedTabIndicator.position = tab.transform.position;
            m_SelectedTab = tab;
        }

        // Sets the selected tab
        public void SetHoverTab(Tab tab) {
            m_HoverTabIndicator.position = tab.transform.position;
            m_HoverTab = tab;
        }

        // Ends a tab being hovered over.
        public void EndHoverTab(Tab tab) {
            if (m_HoverTab == tab) {
                m_HoverTab = null;
            }
        }

        #endregion

    }

}