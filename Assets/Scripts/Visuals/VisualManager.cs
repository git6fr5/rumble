/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Visuals;

namespace Platformer.Management {

    ///<summary>
    /// Ties the visual functionality to the rest of the game.
    ///<summary>
    public class VisualManager : MonoBehaviour {

        #region Fields.

        // Controls the camera in the game.
        [SerializeField]
        private CameraController m_CameraController;
        public CameraController Camera => m_CameraController;

        // Controls the particles in the game.
        [SerializeField]
        private ParticleController m_ParticleController;
        public ParticleController Particles => m_ParticleController;

        // The default color palette.
        [SerializeField]
        private ColorPalette m_DefaultPalette;
        public ColorPalette DefaultPalette => m_DefaultPalette;

        // The rendering layers in the game.
        [HideInInspector]
        private RenderingLayers m_RenderingLayers = new RenderingLayers();
        public RenderingLayers RenderingLayers => m_RenderingLayers;

        #endregion

        #region Methods.

        public void OnGameLoad() {
            // Application.targetFrameRate = VisualSettings.FrameRate;
            m_CameraController.RecolorScreen(m_DefaultPalette);
        }

        #endregion

    }

}