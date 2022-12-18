// TODO: Clean

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.LevelLoader;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class Menu : MonoBehaviour {

        #region Variables

        // Singleton.
        public static Menu Instance;

        // Sound manager.
        [SerializeField] private SoundManager m_SoundManager;
        public static SoundManager SoundManager => Instance.m_SoundManager;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        // Runs once before the first frame.
        void Start() {
            Screen.Instance.Init();
            m_SoundManager.OnStart();
        }

        // Validate an array.
        public static bool Validate<T>(T[] array) {
            return array != null && array.Length > 0;
        }

        // Validate a list.
        public static bool Validate<T>(List<T> list) {
            return list != null && list.Count > 0;
        }

    }

}

