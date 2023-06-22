/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Visuals {

    ///<summary>
    /// Stores a set of rendering layers for easy reference.
    ///<summary>
    [System.Serializable]
    public class RenderingLayers {

        [Header("Platforms"), Space(2)]
        public string PlatformLayer = "Midground";
        public int PlatformOrder = 1;

        [Header("Characters"), Space(2)]
        public string CharacterLayer = "Midground";
        public int CharacterOrder = 0;

        [Header("Blocks"), Space(2)]
        public string BlockLayer = "Midground";
        public int BlockOrder = 2;

        [Header("Spikes"), Space(2)]
        public string SpikeLayer = "Midground";
        public int SpikeOrder = 3;

        [Header("Orbs"), Space(2)]
        public string OrbLayer = "Midground";
        public int OrbOrder = 4;


        [Header("Sky"), Space(2)]
        public string SkyLayer = "Background";
        public int SkyOrder = -100;
        
        [Header("Tiles"), Space(2)]
        public string TileLayer = "Foreground";
        [HideInInspector]
        public int TileOrder = 3;

        [Header("Decor"), Space(2)]
        public string ForegroundDecor = "Foreground";

        [SerializeField]
        public float ForegroundScalePer100 = 0.35f;

        [SerializeField]
        public float BackgroundScalePer100 = 0.35f;
        
        [SerializeField]
        public float ForegroundShadePer100 = 0.8f;
        
        [Header("Decor"), Space(2)]
        public string BackgroundDecor = "Background";

        [System.Serializable]
        public class DecorationData {
            public string id;
            public string renderingLayer;
            public Material material;
            public float scale = 1f;
            public int order = 0; 
        }

        public List<DecorationData> DecorData;


    }

}
    