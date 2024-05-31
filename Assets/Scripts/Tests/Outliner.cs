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

        public float outlineWidth;
        public Color outlineColor;

        public int minToOutline = 0;
        public int offset = -1;

        // Runs once before the first frame.
        void Start() {

            transform.SetParent(null);
            transform.localPosition = Vector3.zero;

            m_OutlineRenderers = new List<SpriteRenderer>();

            for (int i = 0; i < m_ToOutline.Length; i++) {
                SpriteRenderer spriteRenderer = new GameObject(m_ToOutline[i].name + " outline", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                
                spriteRenderer.transform.SetParent(transform);
                spriteRenderer.transform.FromMatrix(m_ToOutline[i].localToWorldMatrix);

                m_OutlineRenderers.Add(spriteRenderer);

                minToOutline = m_ToOutline[i].sortingOrder < minToOutline ? m_ToOutline[i].sortingOrder : minToOutline;
                
            }

        }
            
        // Runs once every frame.
        void Update() {


            for (int i = 0; i < m_ToOutline.Length; i++) {

                m_OutlineRenderers[i].sortingOrder = minToOutline-offset;
                m_OutlineRenderers[i].sortingLayerName = m_ToOutline[i].sortingLayerName;
                
                m_OutlineRenderers[i].color = outlineColor;

                m_OutlineRenderers[i].transform.FromMatrix(m_ToOutline[i].transform.localToWorldMatrix);
                m_OutlineRenderers[i].transform.position = m_ToOutline[i].transform.position;

                // m_OutlineRenderer.transform.localPosition = Vector3.zero;
                // m_OutlineRenderer.transform.localRotation = Quaternion.identity;
                // m_OutlineRenderer.transform.localScale = Vector3.right + Vector3.up + Vector3.forward;

                m_OutlineRenderers[i].transform.localScale = m_ToOutline[i].transform.lossyScale + new Vector3(outlineWidth, outlineWidth, 0f);

                if (m_OutlineRenderers[i].sprite != m_ToOutline[i].sprite) {
                    m_OutlineRenderers[i].sprite = m_ToOutline[i].sprite;
                } 

            }
            
            
        }

    }

}