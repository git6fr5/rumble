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
        private EffectController m_EffectController;
        public EffectController Effects => m_EffectController;

        // The default color palette.
        [SerializeField]
        private Texture2D m_DefaultPalette;
        public Texture2D DefaultPalette => m_DefaultPalette;

        // The rendering layers in the game.
        [SerializeField]
        private RenderingLayers m_RenderingLayers = new RenderingLayers();
        public RenderingLayers RenderingLayers => m_RenderingLayers;

        #endregion

        #region Methods.

        public void OnGameLoad() {
            // Application.targetFrameRate = VisualSettings.FrameRate;
            // m_CameraController.ReshapeWindow();
            // m_CameraController.RecolorScreen(m_DefaultPalette);
        }

        #endregion

    }

}