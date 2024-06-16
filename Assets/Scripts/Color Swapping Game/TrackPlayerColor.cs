using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Levels.LDtk;

namespace ColorSwap {

    using CharacterController = Platformer.Character.CharacterController;

    public class TrackPlayerColor : MonoBehaviour {

        public int offset;
        public int trackingLayer;

        void Start() {
            print("tracking layer hi" + gameObject.name);
            int layer = Platformer.PlayerManager.Character.gameObject.layer;
            ColorSwapManager.Instance.SwapColorInGameByLayer(transform, layer, offset);
            trackingLayer = layer;
        }

        void Update() {
            
            int layer = Platformer.PlayerManager.Character.gameObject.layer;
            if (layer != trackingLayer) {
                ColorSwapManager.Instance.SwapColorInGameByLayer(transform, layer, offset);
                trackingLayer = layer;
            }

        }

    }

}