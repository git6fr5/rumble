/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
using Platformer.Management;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Management {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour {

        #region Variables

        // Singleton.
        public static GameManager Instance;

        // Cached ldtk component.
        public static LDtkComponentProject m_LDtkData;

        // Player.
        [SerializeField] private CharacterController m_Player;
        public static CharacterController MainPlayer => Instance.m_Player;

        // Exposes functionality for the levels in the game.
        [SerializeField] 
        private LevelManager m_LevelManager;
        public static LevelManager Level => Instance.m_LevelManager;

        // Exposes functionality for the physics in the game.
        [SerializeField] 
        private PhysicsManager m_PhysicsManager;
        public static PhysicsManager Physics => Instance.m_PhysicsManager;

        // Exposes functionality for the audio in the game.
        [SerializeField] 
        private AudioManager m_AudioManager;
        public static AudioManager Audio => Instance.m_AudioManager;

        // Exposes functionality for the visuals in the game.
        [SerializeField] 
        private VisualManager m_VisualManager;
        public static VisualManager Visuals => Instance.m_VisualManager;

        // Score.
        // [SerializeField] private ScoreTracker m_Score;
        // public static ScoreTracker Score => Instance.m_Score;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            Instance = this;
            m_Player.gameObject.SetActive(false);
            m_PhysicsManager.OnGameLoad();
            m_AudioManager.OnGameLoad();
            m_VisualManager.OnGameLoad();
            m_LevelManager.OnGameLoad();
            Pause();
        }

        // Runs once before the first frame.
        void Start() {
            m_Player.gameObject.SetActive(true);
            Play();
        }

        public void SetLDtkData(LDtkComponentProject ldtkData) {
            if (ldtkData == null) { return; }
            m_LDtkData = ldtkData;
        }

        // Pause the game.
        public void Play() {
            Instance.m_PhysicsManager.Time.Play();
        }

        // Pause the game.
        public void Pause() {
            Instance.m_PhysicsManager.Time.Pause();
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

