/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Platformer.Visuals;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Timer = Platformer.Utilities.Timer;

namespace Platformer.Visuals {

    ///<summary>
    /// Controls the position and quality of the camera.
    ///<summary>
    [RequireComponent(typeof(Camera)), RequireComponent(typeof(PixelPerfectCamera))]
    public class CameraController : MonoBehaviour {

        #region Fields.
        
        /* --- Constants --- */

        // The distance of the plane that the camera sits on.
        public const float CAMERA_PLANE_DISTANCE = 10f;

        // The amount of time it takes to recolor the screen.
        public const float RECOLOR_TIME = 0.24f;

        // The threshold above which a ramp stop starts ramping up.
        public const float RAMP_THRESHOLD = 0.5f;

        // The default time scale.
        public const float DEFAULT_TIMESCALE = 1f;

        // The paused time scale.
        public const float PAUSED_TIMESCALE = 0f;

        /* --- Static --- */
        
        // The plane that this camera sits on.
        public static Vector3 CameraPlane => Vector3.forward * CAMERA_PLANE_DISTANCE;

        /* --- Components --- */

        // The camera attached to this object.
        private Camera m_Camera => GetComponent<Camera>();

        // The pixel perfect camera attached to this object
        private UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera m_PixelPerfectCamera => GetComponent<UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera>();

        /* --- Members --- */

        // The position that this camera is meant to be at.
        [SerializeField, ReadOnly]
        private Vector2 m_TargetPosition = new Vector2(0f, 0f);

        // The speed with which the camera moves.
        [SerializeField, ReadOnly]
        private float m_MoveSpeed = 0f;

        // The timer for the camera shake.
        [HideInInspector]
        private Timer m_ShakeTimer = new Timer(0f, 0f);

        // The timer for the screen recoloration.
        [HideInInspector]
        private Timer m_RecolorTimer = new Timer(0f, 0f);

        // The current palette that the screen is being colored by.
        [SerializeField]
        private ColorPalette m_CurrentPalette = new ColorPalette();

        // The material being used to color the screen.
        [SerializeField]
        private Material m_ScreenMaterial = null;

        #endregion

        #region Methods.

        // Runs once per frame.
        private void Update() {
            MoveToTarget();
            if (m_ShakeTimer.Active) {
                WhileShakingCamera();
                m_ShakeTimer.Tick(Time.deltaTime);
            }
            if (m_RecolorTimer.Active) {
                WhileColoringScreen();
                m_RecolorTimer.Tick(Time.deltaTime);
            }

        }

        // Reshapes the camera window.
        public void ReshapeWindow(Vector2Int shape) {
            m_PixelPerfectCamera.refResolutionX = shape.x * VisualSettings.PixelsPerUnit;
            m_PixelPerfectCamera.refResolutionY = shape.y * VisualSettings.PixelsPerUnit;
            m_PixelPerfectCamera.assetsPPU = VisualSettings.PixelsPerUnit;
        }

        // Sets the target position of the camera.
        public void SetTarget(Vector2 targetPosition) {
            m_TargetPosition = targetPosition;

            float distance = (targetPosition - (Vector2)transform.position).magnitude;
            m_MoveSpeed = distance / VisualSettings.CameraSnapTime;
        }
        
        // Moves the camera to the target position.
        public Vector3 MoveToTarget() {
            Vector3 targetPosition = (Vector3)m_TargetPosition + CameraPlane;
            if (targetPosition == transform.position) { return; }
            
            Obstacle.Move(transform, actualTarget, snapSpeed, Time.deltaTime, null);
        }

        // Starts the camera shaking.
        public void ShakeCamera(float strength, float duration) {
            m_ShakeStrength = Mathf.Max(m_ShakeStrength, strength);
            m_ShakeTimer.Start(duration);
        }

        // The way the camera moves while it is shaking.
        public void WhileShakingCamera() {
            float strength = VisualSettings.CameraShakeStrength * m_ShakeCurve.Evaluate(m_ShakeTimer.InverseRatio);
            transform.position += (Vector3)Random.insideUnitCircle * strength;
        }

        // Recolors the screen.
        public void RecolorScreen(ColorPalette colorPalette) {
            colorPalette.SetBlend(m_ColorPaletteMaterial, "A");
            m_CurrentPalette.SetBlend(m_ColorPaletteMaterial, "B");
            m_CurrentPalette = colorPalette;
            m_RecolorTimer.Start(RECOLOR_TIME);
        }

        // Gradually blends the two color palettes into the second color palette
        public void WhileColoringScreen() {
            m_ScreenMaterial.SetFloat("_Radius", m_RecolorationTimer.InverseRatio);
        }

        // Gets a random position within the screen bounds.
        public Vector2 RandomPositionWithinBounds() {
            float x = m_PixelPerfectCamera.refResolutionX / m_PixelPerfectCamera.assetsPPU;
            float y = m_PixelPerfectCamera.refResolutionX / m_PixelPerfectCamera.assetsPPU;
            return (Vector2)transform.position + new Vector2(Random.Range(-x, x), Random.Range(-y, y));
        }

        #endregion

    }

}