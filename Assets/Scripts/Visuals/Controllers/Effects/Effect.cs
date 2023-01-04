/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

namespace Platformer.Visuals.Effects {

    public class Effect : MonoBehaviour {

        private bool m_Paused = false;

        public virtual void Play() {
            gameObject.SetActive(true);
            m_Paused = false;
        }

        public virtual void Pause() {
            m_Paused = true;
        }

        public virtual void Stop() {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

    }

}