/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character;

namespace Platformer.Tests {

    ///<summary>
    ///
    ///<summary>
    public class SpawnObjectTest : MonoBehaviour {

        public GameObject m_Object;
        public int i = 0;

        public int n = 50;
        bool sixty = false;
        int twenty = 0;

        void Start() {
            twenty = 0;
            i = n;
            for (int j = 0; j < n; j++) {
                Instantiate(m_Object);
                m_Object.transform.position =  5f * UnityEngine.Random.insideUnitCircle;
            }
        }

        void Update() {
            if (twenty > 10) {
                return;
            }
            if (Time.deltaTime > 1f/20f) {
                twenty += 1;
                if (twenty > 10) {
                    print(i);
                }
                return;
            }
            

            for (int j = 0; j < n; j++) {
                Instantiate(m_Object);
                m_Object.transform.position =  5f * UnityEngine.Random.insideUnitCircle;
            }
            i += n;

            if (Time.deltaTime > 1f/60f && !sixty) {
                print(i);
                sixty = true;
            }
 
            
        }


    }

}