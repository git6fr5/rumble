/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Rendering;

namespace Platformer.Rendering {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class PostProcessor : MonoBehaviour {

        // Renderer
        public Material[] postEffectMaterials;
        public int index;

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            print("Hello");
            Graphics.Blit(source, destination, postEffectMaterials[index]);
        }

    }
}