/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityExtensions;
// Platformer.
using Platformer.Visuals;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Visuals {

    ///<summary>
    /// Controls the position and quality of the camera.
    ///<summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour {

        #region Fields.
        
        /* --- Constants --- */

        // The distance of the plane that the camera sits on.
        public const float CAMERA_PLANE_DISTANCE = -10f;

        /* --- Static --- */
        
        // The plane that this camera sits on.
        public static Vector3 CameraPlane => Vector3.forward * CAMERA_PLANE_DISTANCE;

        /* --- Components --- */

        private Camera m_Camera = null;
        public Camera camera => m_Camera;

        /* --- Parameters --- */

        // The position that this camera is meant to be at.
        [SerializeField, ReadOnly]
        private Vector2 m_TargetPosition = new Vector2(0f, 0f);

        // The speed with which the camera moves.
        [SerializeField, ReadOnly]
        private float m_MoveSpeed = 0f;

        // The timer for the camera shake.
        [HideInInspector]
        private Timer m_ShakeTimer = new Timer(0f, 0f);

        // The animation curve for shaking the screen.
        [SerializeField]
        private AnimationCurve m_ShakeCurve;

        // The amount of strength the current screen shake has.
        [SerializeField]
        private float m_ShakeStrength = 1f;

        #endregion

        #region Methods.

        void Awake() {
            m_Camera = GetComponent<Camera>();
        }

        // Runs once per frame.
        private void Update() {
            MoveToTarget();
            if (m_ShakeTimer.Active) {
                WhileShakingCamera();
                m_ShakeTimer.TickDown(Time.deltaTime);
            }
        }

        // Sets the target position of the camera.
        public void SetTarget(Vector2 targetPosition) {
            m_TargetPosition = targetPosition;

            float distance = (targetPosition - (Vector2)transform.position).magnitude;
            m_MoveSpeed = distance / VisualSettings.CameraSnapTime;
        }
        
        // Moves the camera to the target position.
        public void MoveToTarget() {
            Vector3 targetPosition = (Vector3)m_TargetPosition + CameraPlane;
            transform.Move(targetPosition, m_MoveSpeed, Time.deltaTime);
        }

        // Starts the camera shaking.
        public void ShakeCamera(float strength, float duration) {
            m_ShakeStrength = Mathf.Max(m_ShakeStrength, strength);
            m_ShakeTimer.Start(duration);
        }

        // The way the camera moves while it is shaking.
        public void WhileShakingCamera() {
            float strength = VisualSettings.CameraShakeStrength * m_ShakeStrength * m_ShakeCurve.Evaluate(m_ShakeTimer.InverseRatio);
            transform.Shake(transform.position, strength);
        }

        // Gets a random position within the screen bounds.
        public Vector2 RandomPositionWithinBounds() {
            return Vector2.zero;
        }

        public bool IsWithinBounds(Vector2 position) {
            return true;
        }

        #endregion

    }

}