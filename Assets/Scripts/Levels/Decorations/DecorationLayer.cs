/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Platformer.LevelEditing;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class DecorationLayer : MonoBehaviour {

        [System.Serializable]
        public class Decoration {
            public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        }

        [SerializeField]
        private Material m_Material;

        [SerializeField]
        private string m_SortingLayer;

        [SerializeField]
        private Color m_Color = new Color(1f, 1f, 1f, 1f);
        
        [SerializeField]
        private int m_SortingOrderOffset;
        private const int ORDER_OFFSET_PER_CHILD = -10;

        [SerializeField]
        private List<Decoration> m_Decorations = new List<Decoration>();

        void Update() {
            if (!Application.isPlaying) {
                SetName();
                if (DecorationOverhead.CurrentLayer == this) { 
                    ParentAllNewObjects(transform, "Decoration");
                }

                // Update the list of decorations.
                m_Decorations = new List<Decoration>();
                SpriteRenderer spriteRenderer = null;
                Decoration decoration = null;
                foreach (Transform child in transform) {
                    if (child.tag == "Decoration") {

                        decoration = new Decoration();
                        spriteRenderer = child.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null) {
                            decoration.spriteRenderers.Add(spriteRenderer);
                        }
                        GatherSpriteRenderers(child, ref decoration, ref spriteRenderer);
                        m_Decorations.Add(decoration);

                    }
                }

                int i = m_SortingOrderOffset;
                foreach (Decoration decor in m_Decorations) {
                    i += ORDER_OFFSET_PER_CHILD;
                    for (int j = 0; j < decor.spriteRenderers.Count; j++) {
                        EditDecoration(decor.spriteRenderers[j], m_Material, m_SortingLayer, i + j, m_Color);
                    }
                }

            }
            
        }

        public void GatherSpriteRenderers(Transform parent, ref Decoration decoration, ref SpriteRenderer spriteRenderer) {
            foreach (Transform child in parent) {
                spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) {
                    decoration.spriteRenderers.Add(spriteRenderer);
                }
                if (child.childCount > 0) {
                    GatherSpriteRenderers(child, ref decoration, ref spriteRenderer);
                }
            }
        }

        void SetName() {
            string matName = m_Material != null ? m_Material.name : "default";
            string roomName = transform.parent != null ? transform.parent.gameObject.name : "N/A";
            gameObject.name = roomName + " " + m_SortingLayer + " " + m_SortingOrderOffset.ToString(); // " (" + matName + ")";
        }

        void ParentAllNewObjects(Transform parent, string tag) {
            GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);
            for (int i = 0; i < allObjects.Length; i++) {
                if (allObjects[i].transform.parent == null) {
                    allObjects[i].transform.SetParent(transform);
                }
            }

        }

        void EditDecoration(SpriteRenderer spriteRenderer, Material mat, string sortingLayer, int sortingOrder, Color color) {
            if (spriteRenderer != null) {
                // spriteRenderer.sharedMaterial = DecorationOverhead.GetMaterial(spriteRenderer, mat);
                spriteRenderer.sortingLayerName = sortingLayer;
                spriteRenderer.sortingOrder = sortingOrder;
                spriteRenderer.color = color;
            }
        }

    }

}