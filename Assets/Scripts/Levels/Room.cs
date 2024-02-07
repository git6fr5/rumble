// TODO: Clean

/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using LDtkUnity;
// Platformer.
using Platformer.Character;
using Platformer.Levels;

/* --- Definitions --- */
using Game = Platformer.GameManager;

namespace Platformer.Levels {

    /// <summary>
    ///
    /// <summary>
    [ExecuteInEditMode, RequireComponent(typeof(BoxCollider2D))]
    public class Room : MonoBehaviour {

        #region Enumerations.

        public enum State {
            Loaded,
            Unloaded
        }

        #endregion

        /* --- Members --- */

        // Whether this level is currently loaded.
        [SerializeField, ReadOnly]
        private State m_State = State.Unloaded;  
        public State state => m_State;

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

        public bool snapToThis = false;
        void Update() {
            if (!Application.isPlaying && snapToThis) {
                Vector3 targetPosition = (Vector3)transform.position + Vector3.forward * Gobblefish.Graphics.CameraMovement.CAMERA_PLANE_DISTANCE;
                Camera.main.transform.position = targetPosition;
                snapToThis = false;
            }
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Camera.main.transform.parent.GetComponent<Gobblefish.Graphics.CameraMovement>().AddTarget(transform);
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Camera.main.transform.parent.GetComponent<Gobblefish.Graphics.CameraMovement>().RemoveTarget(transform);
            }
        }

        public bool debug = true;
        void OnDrawGizmos() {
            if (!debug) {
                return;
            }
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            BoxCollider2D box = GetComponent<BoxCollider2D>();

            Camera camera = Camera.main;
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;

            // Camera.main
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
            
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(transform.position, new Vector3(halfWidth * 2f, halfHeight * 2f, 0f));


        }

    }

}
