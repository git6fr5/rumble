using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Levels.LDtk;

namespace ColorSwap {

    using CharacterController = Platformer.Character.CharacterController;

    public class SwapColor : MonoBehaviour {

        public int index = 0;

        public void Set(LDtkTileData tile) {
            index = 0;
            if (tile != null) {
                index = tile.vectorID.x;
            }
            ColorSwap.ColorSwapManager.Instance.SwapColor(transform, index, true, false);
        }

        public void SwapColorToThisLayer(CharacterController character) {
            ColorSwapManager.Instance.SwapColorInGame(character.transform, index);
        }

    }

}