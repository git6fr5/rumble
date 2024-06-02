// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using Gobblefish.Animation;

namespace Blobbers.Graphics {

    public class Outliner : MonoBehaviour {

        // public const int SORTING_ORDER_OFFSET = -1;

        // The base renderer.
        public SpriteRenderer[] m_ToOutline;

        // The sprite renderer attached to this object.
        public List<SpriteRenderer> m_OutlineRenderers;
        public List<SpriteMask> m_Masks;

        public float outlineWidth;
        public Color outlineColor;

        public int minSortingLayerValue;
        public string minSortingLayerName;
        public int minSortingOrder = 0;
        public int offset = -1;

        // Runs once before the first frame.
        void Start() {

            transform.SetParent(null);
            transform.localPosition = Vector3.zero;

            m_OutlineRenderers = new List<SpriteRenderer>();
            m_Masks = new List<SpriteMask>();

            for (int i = 0; i < m_ToOutline.Length; i++) {
                SpriteRenderer spriteRenderer = new GameObject(m_ToOutline[i].name + " outline", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                spriteRenderer.transform.SetParent(transform);
                spriteRenderer.transform.FromMatrix(m_ToOutline[i].localToWorldMatrix);
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                SpriteMask spriteMask = new GameObject(m_ToOutline[i].name + " mask", typeof(SpriteMask)).GetComponent<SpriteMask>();
                spriteMask.transform.SetParent(transform);
                spriteMask.transform.FromMatrix(m_ToOutline[i].localToWorldMatrix);

                m_OutlineRenderers.Add(spriteRenderer);
                m_Masks.Add(spriteMask);

                int sortingLayerValue = SortingLayer.GetLayerValueFromID(m_ToOutline[i].sortingLayerID);
                if (sortingLayerValue < minSortingLayerValue) {
                    minSortingLayerValue = sortingLayerValue;
                    minSortingLayerName = m_ToOutline[i].sortingLayerName;
                    minSortingOrder = m_ToOutline[i].sortingOrder;
                }
                else if (sortingLayerValue == minSortingLayerValue) {
                    minSortingOrder = m_ToOutline[i].sortingOrder < minSortingOrder ? m_ToOutline[i].sortingOrder : minSortingOrder;
                }
                
            }

        }
            
        // Runs once every frame.
        void Update() {


            for (int i = 0; i < m_ToOutline.Length; i++) {

                m_OutlineRenderers[i].sortingOrder = minSortingOrder+offset;
                m_OutlineRenderers[i].sortingLayerName = minSortingLayerName;
                
                m_OutlineRenderers[i].color = outlineColor;

                m_OutlineRenderers[i].transform.FromMatrix(m_ToOutline[i].transform.localToWorldMatrix);
                m_OutlineRenderers[i].transform.position = m_ToOutline[i].transform.position;
                m_OutlineRenderers[i].transform.localScale = m_ToOutline[i].transform.lossyScale + new Vector3(outlineWidth, outlineWidth, 0f);

                m_Masks[i].transform.FromMatrix(m_ToOutline[i].transform.localToWorldMatrix);
                m_Masks[i].transform.position = m_ToOutline[i].transform.position;
                m_Masks[i].transform.localScale = m_ToOutline[i].transform.lossyScale;

                if (m_OutlineRenderers[i].sprite != m_ToOutline[i].sprite) {
                    m_OutlineRenderers[i].sprite = m_ToOutline[i].sprite;
                } 
                if (m_Masks[i].sprite != m_ToOutline[i].sprite) {
                    m_Masks[i].sprite = m_ToOutline[i].sprite;
                } 

            }
            
            
        }

    }

}