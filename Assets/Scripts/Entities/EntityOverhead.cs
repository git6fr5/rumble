// System.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Unity.
using UnityEngine;

namespace Platformer.LevelEditing {

    [ExecuteInEditMode]
    public class EntityOverhead : MonoBehaviour {

        // The cached list of all objects under the current layer.
        private List<GameObject> m_AllObjects = new List<GameObject>();

        [SerializeField]
        private EntityLayer m_CurrentLayer = null;

        void Start() {
            if (!Application.isPlaying) {
                m_AllObjects = GameObject.FindGameObjectsWithTag(tag).ToList();
            }
        }

        void Update() {
            if (!Application.isPlaying) {
                if (m_CurrentLayer == null) { return; }
                ParentAllNewObjects(m_CurrentLayer.transform, "Entity");
            }
        }

        void ParentAllNewObjects(Transform parent, string tag) {
            // GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);
            List<GameObject> allObjects = GameObject.FindGameObjectsWithTag(tag).ToList();

            for (int i = 0; i < allObjects.Count; i++) {
                if (allObjects[i].transform.parent == null && !m_AllObjects.Contains(allObjects[i])) {
                    allObjects[i].transform.SetParent(parent);
                }
            }
        }

    }

}