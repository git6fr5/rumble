/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class PlayButton : Button {
            
        public WorldButtonsLayout a;
        public BoxCollider2D hitbox => GetComponent<BoxCollider2D>();
        public SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

        void Update() {
            if (a.open && a.ticks > 0.125f) {

                hitbox.enabled = false;
                spriteRenderer.enabled = false;

                bool click = UnityEngine.Input.GetMouseButtonDown(0);
                if (click) {
                    a.open = !a.open;
                    hitbox.enabled = !a.open;
                    spriteRenderer.enabled = !a.open;
                }

            }
        }

        public override void Activate() {
            a.open = !a.open;        
        }

    }
}