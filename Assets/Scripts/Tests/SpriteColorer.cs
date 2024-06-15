/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gobblefish.Audio;
using Platformer.Character;

namespace Platformer.Tests {

    public class SpriteColorer : MonoBehaviour {

        public List<Color> colors;

        public bool debugColorer = false;

        void Update() {
            if (Application.isPlaying && debugColorer) {
                Color(0);
            }
        }

        public void Color(int index) {
            if (index > colors.Count || index < 0) {
                return;
            }
            recursive_Color(transform, colors[index]);
        }

        private void recursive_Color(Transform transform, Color color) {
            SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer) {
                spriteRenderer.color = color;
            }
            foreach (Transform child in transform) {
                recursive_Color(child, color);
            }
        }

    }

}