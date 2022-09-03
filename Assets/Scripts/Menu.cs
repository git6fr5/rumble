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

        // Ticks.
        [SerializeField] private float m_TimeScale;
        [SerializeField] private float m_Ticks;
        public static float Ticks => Instance.m_Ticks;


        // Sound manager.
        [SerializeField] private SoundManager m_SoundManager;
        public static SoundManager SoundManager => Instance.m_SoundManager;

        // Particle rid.
        [SerializeField] private GridRenderer m_ParticleGrid;
        public static GridRenderer ParticleGrid => Instance.m_ParticleGrid;

        // Position
        [SerializeField] private Vector3 m_PrevMousePosition;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        // Runs once before the first frame.
        void Start() {
            Screen.Instance.Init();
            m_ParticleGrid.BuildGrid();
            m_SoundManager.OnStart();
            m_PrevMousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        }

        // Runs once every frame.
        void Update() {
            Time.timeScale = m_TimeScale;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            float distance = (mousePosition - m_PrevMousePosition).magnitude;
            if (distance < 0.25f * Time.deltaTime) {
                distance = 0f;
            }
            m_ParticleGrid.Implode(mousePosition, 2.5e2f * distance, 3f * distance + 1f);

            m_PrevMousePosition = mousePosition;

            bool pressed = UnityEngine.Input.GetMouseButton(0);

            if (pressed) {
                m_ParticleGrid.Spin(mousePosition, 1e4f, 3f, 1f);
            }

            ticks += Time.deltaTime;
            if (ticks >= 0.25f) {
                ticks -= 0.25f;
                cachedPos = Screen.RandomPositionWithinBounds();
            }
            m_ParticleGrid.Implode(cachedPos, 1e3f, 6f);

        }
        Vector3 cachedPos = new Vector3(0f, 0f, 0f);
        float ticks = 0f;

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
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

