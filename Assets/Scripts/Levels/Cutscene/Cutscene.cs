// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Platformer.Levels {

    public class Cutscene : MonoBehaviour {

        public Volume m_Volume;

        public float m_FadeOutDuration = 1f;

        public bool end = false;

        public void Update() {
            float dt = Time.deltaTime;


            if (end) {
                m_Volume.weight -= m_FadeOutDuration * dt;
            }

        }

    }

}
