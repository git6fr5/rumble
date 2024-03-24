
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Platformer.LevelEditing;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class DecorationOverhead : MonoBehaviour {

        // The singleton.
        [SerializeField]
        private static DecorationOverhead INSTANCE = null;

        [SerializeField]
        private DecorationLayer m_CurrentLayer = null;
        public static DecorationLayer CurrentLayer => INSTANCE != null ? INSTANCE.m_CurrentLayer : null;

        void Update() {
            if (!Application.isPlaying) {
                if (INSTANCE == null) {
                    INSTANCE = this;
                }
            }
        }

    }

}
