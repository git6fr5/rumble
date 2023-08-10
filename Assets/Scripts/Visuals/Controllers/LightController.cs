/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;

namespace Platformer.Visuals {

    ///<summary>
    /// Controls the particle effects in the game.
    ///<summary>
    public class LightController : MonoBehaviour {

        [SerializeField]
        private Light2D m_ForegroundLight;
        public Light2D Foreground => m_ForegroundLight;

        [SerializeField]
        private Light2D m_BackgroundLight;
        public Light2D Background => m_BackgroundLight;


    }

}