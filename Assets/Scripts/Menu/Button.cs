/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.UI;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public abstract class Button : MonoBehaviour {

        bool hover = false;
        float ticks = 0f;

        void FixedUpdate() {
            ticks += Time.fixedDeltaTime * 5f;

            if (hover) {
                transform.localScale = (1.25f + 0.125f * Mathf.Sin(ticks)) * new Vector3(1f, 1f, 1f);
            }else {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        
        void OnMouseDown() {
            print("hi");
            Activate();
        }

        void OnMouseOver() {
            hover = true;
        }

        void OnMouseExit() {
            hover = false;
        }

        public abstract void Activate();

    }
}