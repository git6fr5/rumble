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
        private SpriteRenderer[] m_StaticDecorations;

        [SerializeField]
        private Renderer[] m_StaticRenderers;

        // [SerializeField]
        // private SpriteRenderer[] m_FoliageDecorations;

        // Initializes this entity.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin
            transform.localPosition = worldPosition;
            m_Origin = worldPosition;

            // Set the layer. Note the sprite renderers for this will have to be set manually.
            gameObject.layer = Game.Physics.CollisionLayers.DecorLayer;

        }

        public void SetRendering(string layerID) {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Platformer.Visuals.RenderingLayers.DecorationData data = Game.Visuals.RenderingLayers.DecorData.Find(data => data.id == layerID);

            if (data == null) {
                return;
            }

            if (m_Rescale) {
                transform.localScale *= data.scale;
            }

            for (int i = 0; i < renderers.Length; i++) {
                renderers[i].sortingLayerName = data.renderingLayer;
                renderers[i].sortingOrder += data.order;
            }

            for (int i = 0; i < m_StaticDecorations.Length; i++) {
                if (data.material != null) {
                    m_StaticDecorations[i].sharedMaterial = data.material;
                }
            }

            for (int i = 0; i < m_StaticRenderers.Length; i++) {
                if (data.material != null) {
                    m_StaticRenderers[i].sharedMaterial = data.material;
                }
            }
            
        }

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

    }

}
