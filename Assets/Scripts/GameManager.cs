/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.LevelLoader;
using Platformer.Management;

namespace Platformer.Management {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour {

        #region Variables

        // Singleton.
        public static GameManager Instance;

        // Ticks.
        [SerializeField] private float m_TimeScale;
        [SerializeField] private float m_Ticks;
        public static float Ticks => Instance.m_Ticks;

        // Player.
        [SerializeField] private CharacterController m_Player;
        public static CharacterController MainPlayer => Instance.m_Player;

        // Level loading.
        [SerializeField] private LDtkLoader m_LevelLoader;
        public static LDtkLoader LevelLoader => Instance.m_LevelLoader;

        // Exposes functionality for the physics in the game.
        [SerializeField] private PhysicsManager m_PhysicsManager;
        public static PhysicsManager Physics => Instance.m_PhysicsManager;

        // Exposes functionality for the audio in the game.
        [SerializeField] 
        private AudioManager m_AudioManager;
        public static AudioManager Audio => Instance.m_AudioManager;

        // Exposes functionality for the visuals in the game.
        [SerializeField] 
        private VisualManager m_VisualManager;
        public static AudioManager Visuals => Instance.m_VisualManager;

        // Grid.
        [SerializeField] private UnityEngine.Grid m_Grid;
        public static UnityEngine.Grid MainGrid => Instance.m_Grid;

        // Score.
        [SerializeField] private ScoreTracker m_Score;
        public static ScoreTracker Score => Instance.m_Score;

        // Opening level.
        [SerializeField] private string m_OpeningLevel;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            Instance = this;
            Application.targetFrameRate = 120; // Settings.FrameRate
        }

        // Runs once before the first frame.
        void Start() {
            Level.InitializeGroundLayer(m_Grid.transform);
            Level.InitializeWaterLayer(m_Grid.transform);
            m_LevelLoader.Init();
            Screen.Instance.gameObject.SetActive(false);
            m_Player.gameObject.SetActive(false);
            StartCoroutine(IELoadOpeningLevel());
        }

        // Load the opening level.
        private IEnumerator IELoadOpeningLevel() {
            yield return 0;
            m_LevelLoader.SetLoadPoint(m_OpeningLevel, m_Player.transform);
            Screen.Instance.transform.position = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y + 10f, Screen.Instance.transform.position.z);
            Screen.Instance.gameObject.SetActive(true);
            yield return 0;
            m_SoundManager.OnStart();
            m_Score.Init(LevelLoader.Levels);
            m_Player.gameObject.SetActive(true);
            yield return null;
        }

        // Runs once every frame.
        void Update() {
            Time.timeScale = m_TimeScale * m_PhysicsManager.Time.TimeScale;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
        }

        // Pause the game.
        public static void Pause() {
            Instance.m_TimeScale = 0f;
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

