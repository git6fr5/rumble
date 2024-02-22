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
    public class DecorationOverhead : MonoBehaviour {

        // The singleton.
        [SerializeField]
        private static DecorationOverhead INSTANCE = null;

        [SerializeField]
        private Tilemap m_CollisionGrid;
        public bool hideGrid;

        [SerializeField]
        private Tilemap m_ForegroundMask;
        public bool hideForeground;

        [SerializeField]
        private DecorationLayer m_CurrentLayer = null;
        public static DecorationLayer CurrentLayer => INSTANCE != null ? INSTANCE.m_CurrentLayer : null;

        [System.Serializable]
        public class MaterialRemap {
            public Material originalMaterial;
            public Material targetMaterial;
            public Material outputMaterial;
        }

        public MaterialRemap[] remaps;

        [SerializeField]
        public static Dictionary<SpriteRenderer, Material> OriginalMaterials = new Dictionary<SpriteRenderer, Material>();

        void Update() {
            if (!Application.isPlaying) {

                if (INSTANCE == null) {
                    INSTANCE = this;
                }

                if (hideForeground) {
                    m_ForegroundMask.color = new Color(1f, 1f, 1f, 0.2f);
                }
                else {
                    m_ForegroundMask.color = new Color(1f, 1f, 1f, 1f);
                }

                if (hideGrid) {
                    m_CollisionGrid.color = new Color(1f, 1f, 1f, 0.2f);
                }
                else {
                    m_CollisionGrid.color = new Color(1f, 1f, 1f, 1f);
                }

            }
        }

        public static Material GetMaterial(SpriteRenderer spriteRenderer, Material targetMaterial) {
            if (spriteRenderer.GetComponent<DecorationMaterialRemap>() == null) {
                return targetMaterial;
            }
            return spriteRenderer.sharedMaterial; 
            // return INSTANCE._GetMaterial(originalMaterial, targetMaterial);
        }

        private Material _GetMaterial(Material originalMaterial, Material targetMaterial) {
            // for (int i = 0; i < remaps.Length; i++) {
            //     if (remaps[i].originalMaterial == originalMaterial && remaps[i].targetMaterial == targetMaterial) {
            //         return remaps[i].outputMaterial;
            //     }
            // }
            return targetMaterial;
        }

    }

}