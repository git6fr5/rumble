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
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels {

    /// <summary>
    ///
    /// <summary>
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

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Game.Level.Load(this);
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider == Game.MainPlayer.Collider) {
                Game.Level.Unload(this);
            }
        }

    }

}
