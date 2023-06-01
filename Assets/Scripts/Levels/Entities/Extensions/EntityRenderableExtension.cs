/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Entities {

    public interface IRenderable {
        void SetRendering(string layerName);
    }

    public static class EntityRenderableExtension {

        // Take the control data and turn it into a period offset.
        public static void SetRendering(this Entity entity, string layerName) {

            IRenderable renderable = entity.GetComponent<IRenderable>();
            if (renderable == null) {
                return;
            }
            
            renderable.SetRendering(layerName);

        }

    }
}