/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.UI;
using Platformer.Obstacles;

namespace Platformer.UI {

    ///<summary>
    ///
    ///<summary>
    public class WorldButtonsLayout : MonoBehaviour {

        public GameObject unlockedButton;
        public GameObject lockedButton;

        public Vector3 origin;
        
        // public Platformer.Button lockedButton;
        // public Platformer.Button lockedButton;

        List<GameObject> buttons = new List<GameObject>();
        List<Vector3> targets = new List<Vector3>();

        public int worlds;

        void Start() {

            int width = 4;
            float w = (float)width;

            int height = 3;
            float h = (float)height;

            for (int i = 0; i <= 4; i++) {
                for (int j = 0; j <= 3; j++) {
                    Vector3 position = new Vector3(-10f + i * 20f / w, 4 - j * 12f / h, 0f);
                    targets.Add(position);
                    if (h * i + j < worlds) {
                        GameObject obj = Instantiate(unlockedButton.gameObject);
                        obj.transform.position = origin;
                        obj.transform.parent = transform;

                        obj.GetComponent<WorldButton>().index = height * i + j + 1;

                        buttons.Add(obj);
                    }
                    else {
                        GameObject obj = Instantiate(lockedButton.gameObject);
                        obj.transform.position = origin;
                        obj.transform.parent = transform;
                        buttons.Add(obj);
                    }
                }
            }

        }

        public bool open = true;
        public float ticks = 0f;

        public AnimationCurve scaleCurve;

        public void Open() { 
            open = true;
        }

        public void Close() {
            open = false;
        }

        void FixedUpdate() {


            float dt = Time.fixedDeltaTime;
            
            float flip = open ? 1f : -1f;
            ticks += flip * dt * 2f;
            if (ticks > 2f) {
                ticks = 2f;
            }
            else if (ticks < 0f) {
                ticks = 0f;
            }

            transform.localScale = scaleCurve.Evaluate(ticks) * new Vector3(1f, 1f, 1f);

            for (int i = 0; i < buttons.Count; i++) {
                Vector3 target = open ? targets[i] : origin;

                Obstacle.Move(buttons[i].transform, target, 6f, dt);
                
                float distance = (buttons[i].transform.position - origin).magnitude;

                if (distance < 0.05f) {
                    buttons[i].SetActive(false);
                }
                else {
                    buttons[i].SetActive(true);
                }


            }

        }

    }


}