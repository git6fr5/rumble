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
    [RequireComponent(typeof(BoxCollider2D))]
    public class LevelSection : MonoBehaviour {

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

        [SerializeField]
        private Vector2 m_Buffer;

        [HideInInspector]
        private Vector2 m_Position;
        public Vector2 Position => m_Position;

        private GameObject[] m_Pieces;
        public GameObject[] Pieces => m_Pieces;

        void Awake() {
            m_Box = GetComponent<BoxCollider2D>();
            m_Box.isTrigger = true;
            m_Position = (Vector2)transform.position + m_Box.offset;

            int i = 0;
            m_Pieces = new GameObject[transform.childCount];
            foreach (Transform child in transform) {
                m_Pieces[i] = child.gameObject;
                i += 1;
            }

        }

        void Start() {
            Game.Level.Unload(this);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Game.Level.Load(this);
                m_State = State.Loaded;
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Game.Level.Unload(this);
                m_State = State.Unloaded;
            }
        }

        public bool debug = true;
        void OnDrawGizmos() {
            if (!debug) {
                return;
            }
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size + m_Buffer);

        }

    }

}
