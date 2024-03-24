/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Platformer.LevelEditing;
using Gobblefish;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class DecorationLayer : MonoBehaviour {

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
        private Decoration[] m_Decorations;

        [SerializeField]
        private SpriteRenderer[] m_SpriteRenderers;

        [SerializeField]
        private Transform m_LastAddedTransform;

        [Header("Flip")]
        public bool m_FlipHorizontally = false;
        public bool m_FlipVertically = false;
        [Header("Rotate")]
        // public bool m_Rotate90 = false;
        // public bool m_RotateBack90 = false;
        public bool m_Rotate45 = false;
        public bool m_RotateBack45 = false;
        public bool m_Rotate5 = false;
        public bool m_RotateBack5 = false;

        void Update() {
            if (!Application.isPlaying) {
                SetName();
                if (DecorationOverhead.CurrentLayer == this) { 
                    // ParentAllNewObjects<Decoration>(transform);
                    ParentAllSpriteRenderers(transform);
                }

                // m_Decorations = transform.GetComponentsInChildren<Decoration>();
                m_SpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

                int i = m_SortingOrderOffset;
                foreach (SpriteRenderer spriteRenderer in m_SpriteRenderers) {
                    if (spriteRenderer != null) {
                        i += ORDER_OFFSET_PER_CHILD;
                        EditSpriteRenderer(spriteRenderer, m_Material, m_SortingLayer, i, m_Color);
                    }
                }

                if (UnityEditor.Selection.activeTransform != null) {
                    ControlLastAddedTransform(UnityEditor.Selection.activeTransform);
                }

                print(UnityEditor.Selection.activeTransform);

            }
            
        }

        void SetName() {
            string matName = m_Material != null ? m_Material.name : "default";
            string roomName = transform.parent != null ? transform.parent.gameObject.name : "N/A";
            gameObject.name = roomName + " " + m_SortingLayer + " " + m_SortingOrderOffset.ToString(); // " (" + matName + ")";
        }

        void ParentAllSpriteRenderers(Transform parent) {
            SpriteRenderer[] allObjects = (SpriteRenderer[])GameObject.FindObjectsOfType<SpriteRenderer>();
            for (int i = 0; i < allObjects.Length; i++) {
                if (allObjects[i].transform.parent == null) {
                    allObjects[i].transform.SetParent(transform);
                    m_LastAddedTransform = allObjects[i].transform;
                }
            }

        }

        void EditSpriteRenderer(SpriteRenderer spriteRenderer, Material mat, string sortingLayer, int sortingOrder, Color color) {
            if (spriteRenderer != null) {
                // spriteRenderer.sharedMaterial = DecorationOverhead.GetMaterial(spriteRenderer, mat);
                spriteRenderer.sortingLayerName = sortingLayer;
                spriteRenderer.sortingOrder = sortingOrder;
                spriteRenderer.color = color;
            }
        }

        void OnDrawGizmos() {

            for (int i = 0; i < m_SpriteRenderers.Length; i++) {
                
                Bounds bounds = m_SpriteRenderers[i].bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.extents * 2f);
            }

        }

        void ControlLastAddedTransform(Transform transform) {
            if (m_FlipHorizontally) {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
                m_FlipHorizontally = false;
            }

            if (m_FlipVertically) {
                Vector3 localScale = transform.localScale;
                localScale.y *= -1f;
                transform.localScale = localScale;
                m_FlipVertically = false;
            }

            // if (m_Rotate90) { Rotate(-90f, ref m_Rotate90); }
            // if (m_RotateBack90) { Rotate(90f, ref m_RotateBack90); }
            if (m_Rotate45) { Rotate(transform, -45f, ref m_Rotate45); }
            if (m_RotateBack45) { Rotate(transform, 45f, ref m_RotateBack45); }
            if (m_Rotate5) { Rotate(transform, -5f, ref m_Rotate5); }
            if (m_RotateBack5) { Rotate(transform, 5f, ref m_RotateBack5); }
        }

        public void Rotate(Transform transform, float angle, ref bool p) {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z += angle;
            transform.eulerAngles = eulerAngles;
            p = false;
        }

    }

}