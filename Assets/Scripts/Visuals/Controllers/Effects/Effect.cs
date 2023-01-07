/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

namespace Platformer.Visuals.Effects {

    ///<summary>
    ///
    ///<summary>
    public class Effect : MonoBehaviour {

        #region Variables.

        // Whether this effect is paused.
        private bool m_Paused = false;

        #endregion

        #region Methods.

        // Play the effect.
        public virtual void Play() {
            gameObject.SetActive(true);
            m_Paused = false;
        }

        // Pause the effect.
        public virtual void Pause() {
            m_Paused = true;
        }

        // Stops the effect.
        public virtual void Stop() {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        #endregion

    }

}