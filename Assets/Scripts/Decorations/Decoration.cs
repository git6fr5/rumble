/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using EnvironmentLayer = Platformer.Visuals.Environment.EnvironmentLayer;
using IInitializable = Platformer.Levels.Entities.IInitializable;
using IRotatable = Platformer.Levels.Entities.IRotatable;
using IRenderable = Platformer.Levels.Entities.IRenderable;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class Decoration : MonoBehaviour, IInitializable, IRotatable, IRenderable {

        [SerializeField]
        private float m_Rotation;

        [SerializeField, ReadOnly]
        private Vector3 m_Origin;

        [SerializeField]
        private bool m_Rescale = true;

        [SerializeField]
        private bool m_AttachToBottom = true;

        [SerializeField]
        private SpriteRenderer[] m_StaticPieces;

        [SerializeField]
        private SpriteRenderer[] m_DynamicPieces;

        // Initializes this entity.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin
            transform.localPosition = worldPosition;
            m_Origin = worldPosition;

            // Set the layer. Note the sprite renderers for this will have to be set manually.
            gameObject.layer = Game.Physics.CollisionLayers.DecorLayer;

        }

        public void SetRendering(string layerID) {
            EnvironmentLayer layer = Game.Visuals.RenderingLayers.CurrentEnvironmnet.GetLayer(layerID);
            if (layer == null) { return; }

            if (m_Rescale) {
                transform.localScale *= layer.Scale;
                if (m_AttachToBottom) {
                    transform.localPosition += Mathf.Max((1f - layer.Scale)) * Vector3.down;
                }
            }

            for (int i = 0; i < m_StaticPieces.Length; i++) {
                m_StaticPieces[i].sortingLayerName = layer.RenderingLayer;
                m_StaticPieces[i].sortingOrder += layer.Order;
                if (layer.StaticMaterial != null) {
                    m_StaticPieces[i].sharedMaterial = layer.StaticMaterial;
                }
            }

            for (int i = 0; i < m_DynamicPieces.Length; i++) {
                m_DynamicPieces[i].sortingLayerName = layer.RenderingLayer;
                m_DynamicPieces[i].sortingOrder += layer.Order;
                if (layer.DynamicMaterial != null) {
                    m_DynamicPieces[i].sharedMaterial = layer.DynamicMaterial;
                }
            }
            
        }

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

    }

}
