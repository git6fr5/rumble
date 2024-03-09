// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Gobblefish.
using Gobblefish.Graphics;

namespace Platformer.Levels {

    /// <summary>
    ///
    /// <summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraNode : MonoBehaviour {

        [HideInInspector]
        private BoxCollider2D m_Box;
        public BoxCollider2D Box => m_Box;

        [HideInInspector]
        private Vector2 m_Position;
        public Vector2 Position => m_Position;

        void Awake() {
            m_Position = (Vector2)transform.position;
            m_Box = GetComponent<BoxCollider2D>();
            m_Box.isTrigger = true;
        }

        public void Snap() {
            Vector3 targetPosition = (Vector3)transform.position + Vector3.forward * GraphicsManager.CamMovement.CameraPlaneDistance;
            Camera.main.transform.position = targetPosition;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider == PlayerManager.Character.Collider) {
                Debug.Log(gameObject.name);
                GraphicsManager.CamMovement.AddTarget(transform);
                // cameraMovement.SetDefaultTarget(transform);
                // cameraMovement.RemoveAll()
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider == PlayerManager.Character.Collider) {
                GraphicsManager.CamMovement.RemoveTarget(transform);
            }
        }

        public bool debug = true;
        void OnDrawGizmos() {
            if (!debug) { return; }
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            Camera camera = Camera.main;
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(transform.position, new Vector3(halfWidth * 2f, halfHeight * 2f, 0f));
        }

    }

}
