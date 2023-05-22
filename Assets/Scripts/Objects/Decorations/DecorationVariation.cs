/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class DecorationVariation : MonoBehaviour {

        [System.Serializable]
        private struct BoundingBox {
            public float MaxX;
            public float MinX;
            public float MaxY;
            public float MinY;
        }

        private SpriteRenderer m_SpriteRenderer = null;
        
        // The different types of grass available.
        [SerializeField] 
        private Sprite[] m_Variations;

        [SerializeField]
        private BoundingBox m_PositionVariation;

        // [SerializeField, Range(0f, 1f)]
        // private float m_ActivePercent = 1f;

        [SerializeField, ReadOnly]
        private Vector3 m_Origin;

        // Runs once on instantiation.
        void Start() {
            m_Origin = transform.localPosition;
            Vary();
        }

        public void Vary() {
            // gameObject.SetActive(Random.Range(0f, 1f) < m_ActivePercent);
            // if (!gameObject.activeSelf) { return; }

            transform.localPosition = m_Origin + new Vector3(Random.Range(-m_PositionVariation.MinX, m_PositionVariation.MaxX), Random.Range(-m_PositionVariation.MinY, m_PositionVariation.MaxY),0f);
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            if (m_SpriteRenderer != null && m_Variations.Length > 0) { Pick(); }
        }

        public void Pick() {
            int index = Random.Range(0, m_Variations.Length);
            m_SpriteRenderer.sprite = m_Variations[index];
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.yellow;

            if (!Application.isPlaying) {
                m_Origin = transform.localPosition;
            }

            Vector3 pointA = transform.parent.position + m_Origin + Vector3.up * m_PositionVariation.MaxY + Vector3.left * m_PositionVariation.MinX;
            Vector3 pointB = transform.parent.position + m_Origin + Vector3.up * m_PositionVariation.MaxY + Vector3.right * m_PositionVariation.MaxX; 
            Gizmos.DrawLine(pointA, pointB);
        
            pointA = transform.parent.position + m_Origin + Vector3.down * m_PositionVariation.MinY + Vector3.left * m_PositionVariation.MinX;
            pointB = transform.parent.position + m_Origin + Vector3.down * m_PositionVariation.MinY + Vector3.right * m_PositionVariation.MaxX; 
            Gizmos.DrawLine(pointA, pointB);

            pointA = transform.parent.position + m_Origin + Vector3.up * m_PositionVariation.MaxY + Vector3.left * m_PositionVariation.MinX;
            pointB = transform.parent.position + m_Origin + Vector3.down * m_PositionVariation.MinY + Vector3.left * m_PositionVariation.MinX; 
            Gizmos.DrawLine(pointA, pointB);

            pointA = transform.parent.position + m_Origin + Vector3.up * m_PositionVariation.MaxY + Vector3.right * m_PositionVariation.MaxX;
            pointB = transform.parent.position + m_Origin + Vector3.down * m_PositionVariation.MinY + Vector3.right * m_PositionVariation.MaxX; 
            Gizmos.DrawLine(pointA, pointB);
        
        }

    }

}
