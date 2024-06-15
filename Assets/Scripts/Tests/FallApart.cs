/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gobblefish.Audio;
using Platformer.Character;

namespace Platformer.Tests {

    // [DefaultExecutionOrder(1000)]
    public class FallApart : MonoBehaviour {

        Vector3[] origins;
        Rigidbody2D[] bodies;

        void Start() {
            
            if (Application.isPlaying) {

                int count = transform.childCount;
                bodies = new Rigidbody2D[count];
                origins = new Vector3[count];
                
                int i = 0;
                foreach (Transform child in transform) {
                    origins[i] = child.localPosition;
                    bodies[i] = child.gameObject.GetComponent<Rigidbody2D>();
                    if (bodies[i] == null) {
                        bodies[i] = child.gameObject.AddComponent<Rigidbody2D>();
                        bodies[i].simulated = false;
                    }
                    i+= 1;
                }

            }

        }

        public void Activate() {

            foreach (Rigidbody2D body in bodies) {
                if (body != null) {
                    body.simulated = true;
                    body.isKinematic = false;
                    body.gravityScale = 3f;
                    body.velocity = Random.insideUnitCircle * 3f;
                }
            }

        }

        public void PutBackTogether(float duration) {
            foreach (Rigidbody2D body in bodies) {
                if (body != null) {
                    body.simulated = false;
                }
            }
            StartCoroutine(IEPutBackTogether(duration));
        }

        private IEnumerator IEPutBackTogether(float duration) {

            int i = 0;
            foreach (Transform child in transform) {
                child.localPosition = origins[i];
                i+= 1;
                yield return new WaitForSeconds(duration/(float)origins.Length);
            }


        }

    }

}