// System.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Unity.
using UnityEngine;

namespace Platformer.LevelEditing {

    [ExecuteInEditMode]
    public class LevelLayer : MonoBehaviour {

        // The cached list of all objects under the current layer.
        private List<GameObject> m_AllObjects = new List<GameObject>();

        [SerializeField]
        private Transform m_CurrentLayer = null;

        void Start() {
            if (!Application.isPlaying) {
                m_AllObjects = GameObject.FindGameObjectsWithTag(tag).ToList();
            }
        }

        void Update() {
            if (!Application.isPlaying) {
                ParentAllNewObjects(m_CurrentLayer, "Entity");
            }
        }

        void ParentAllNewObjects(Transform parent, string tag) {
            GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);
            List<GameObject> newObjects = GameObject.FindGameObjectsWithTag(tag).ToList();

            for (int i = 0; i < allObjects.Length; i++) {
                if (allObjects[i].transform.parent == null) {
                    allObjects[i].transform.SetParent(transform);
                }
            }
        }

    }

}