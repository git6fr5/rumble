/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Visuals {

    ///<summary>
    ///
    ///<summary>
    public class Environment : MonoBehaviour {

        [System.Serializable]
        public class EnvironmentLayer {
            
            [SerializeField]
            private string m_ID;
            public string ID => m_ID;

            [SerializeField]
            private string m_RenderingLayer;
            public string RenderingLayer => m_RenderingLayer;

            [SerializeField]
            private Material m_StaticMaterial; 
            public Material StaticMaterial => m_StaticMaterial;

            [SerializeField]
            private Material m_DynamicMaterial;
            public Material DynamicMaterial => m_DynamicMaterial;

            [SerializeField]
            private float m_Scale = 1f;
            public float Scale => m_Scale;

            [SerializeField]
            private int m_Order = 0;
            public int Order => m_Order; 
        }

        [SerializeField]
        private Sprite m_BackgroundSky;

        [SerializeField]
        private List<EnvironmentLayer> m_EnvironmentLayers;

        public EnvironmentLayer GetLayer(string id) {
            return m_EnvironmentLayers.Find(layer => layer.ID == id);
        }

    }
}