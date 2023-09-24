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
using Room = Platformer.Levels.Room;

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
        public Camera mainCam => m_Camera;

        /* --- Parameters --- */

        [SerializeField]
        private Transform m_PlayerTransform;

        // The position that this camera is meant to be at.
        [SerializeField]
        private List<Room> m_Targets = new List<Room>();

        // The speed with which the camera moves.
        [SerializeField]
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
        void Update() {
            if (m_ShakeTimer.Active) {
                WhileShakingCamera();
                m_ShakeTimer.TickDown(Time.deltaTime);
            }
        }

        void FixedUpdate() {
            MoveToTarget(Time.fixedDeltaTime);
        }

        // Sets the target position of the camera.
        public void AddTarget(Room target) {
            if (!m_Targets.Contains(target)) {
                m_Targets.Add(target);
            }
        }

        // Sets the target position of the camera.
        public void RemoveTarget(Room target) {
            if (m_Targets.Contains(target)) {
                m_Targets.Remove(target);
            }
        }
        
        // Moves the camera to the target position.
        public void MoveToTarget(float dt) {
            Vector2 aggregatedTargets = new Vector2(0f, 0f);
            if (m_Targets.Count == 0) {
                aggregatedTargets = m_PlayerTransform.position;
            }
            else {
                GetTarget(out aggregatedTargets);
            }
            
            Vector3 targetPosition = (Vector3)aggregatedTargets + CameraPlane;
            transform.Move(targetPosition, m_MoveSpeed, dt);
        
        }

        void GetTarget(out Vector2 target) {
            target = new Vector2(0f, 0f);
            for (int i = 0; i < m_Targets.Count; i++) {
                target += (Vector2)m_Targets[i].Position;
            }
            target /= m_Targets.Count;
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

        void DrawGizmos() {
            // if (m_Targets.Count == 0) {
            //     return;
            // }

            // Vector2 aggregatedTargets = new Vector2(0f, 0f); 
            // for (int i = 0; i < m_Targets.Count; i++) {

            //     Vector2 v = (Vector2)(m_PlayerTransform.position - m_Targets[i].Position);
            //     Vector2 vMax = m_Targets[i].GetComponent<BoxCollider2D>().size;

            //     float xRatio = Mathf.Abs(v.x / vMax.x); float yRatio = Mathf.Abs(v.y / vMax.y);
                
            //     Vector2 vRatio = new Vector2(m_Targets[i].Position.x * xRatio, m_Targets[i].Position.y * yRatio);
            //     aggregatedTargets += vRatio;

            // }

            // Vector3 targetPosition = (Vector3)aggregatedTargets;
            // Gizmos.DrawWireSphere(targetPosition, 3f);
        
        }

        #endregion

    }

}