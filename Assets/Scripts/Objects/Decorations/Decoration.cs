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

        private const float m_ForegroundScalePer100 = 0.7f;
        
        private const float m_ForegroundShadePer100 = 0.4f;

        // Initializes this entity.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin
            transform.localPosition = worldPosition;
            m_Origin = worldPosition;

            // Set the layer. Note the sprite renderers for this will have to be set manually.
            gameObject.layer = Game.Physics.CollisionLayers.DecorLayer;

        }

        public void SetRendering(string renderingLayer) {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            string[] data = renderingLayer.Split("_");

            int order;
            bool parsed = int.TryParse(data[1], out order);
            order = parsed ? order : 0;
            
            if (data[0].Contains("FOREGROUND")) {
                
                float shadeVal = m_ForegroundShadePer100 * (1f + (float)order / 100f);
                Color shade = new Color(1f, 1f, 1f, 1f) - new Color(shadeVal, shadeVal, shadeVal, 0f);
                transform.localScale *= (1f + m_ForegroundScalePer100 * (1f + (float)order / 100f));
                for (int i = 0; i < renderers.Length; i++) {
                    renderers[i].sortingLayerName = Game.Visuals.RenderingLayers.ForegroundDecor;
                    if (renderers[i].GetComponent<SpriteRenderer>() != null) {
                        renderers[i].GetComponent<SpriteRenderer>().color = shade;
                    }
                    if (renderers[i].GetComponent<SpriteShapeRenderer>() != null) {
                        renderers[i].GetComponent<SpriteShapeRenderer>().color = shade;
                    }
                }
            }
            else {
                for (int i = 0; i < renderers.Length; i++) {
                    renderers[i].sortingLayerName = Game.Visuals.RenderingLayers.BackgroundDecor;
                }
            }

            for (int i = 0; i < renderers.Length; i++) {
                renderers[i].sortingOrder = order;
            }

        }

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

    }

}
