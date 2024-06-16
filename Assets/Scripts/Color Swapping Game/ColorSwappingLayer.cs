using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorSwap {

    [System.Serializable]
    public class ColorSwappingLayer {

        [SerializeField]
        public Color color;

        [SerializeField] 
        public string layerName;
        public int layer => LayerMask.NameToLayer(layerName);

    }

}