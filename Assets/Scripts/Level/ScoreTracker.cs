/* --- Libraries --- */
// System.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Platformer.
using Platformer;
using Platformer.LevelLoader;
using Level = Platformer.LevelLoader.Level;
using Checkpoint = Platformer.Obstacles.RespawnBlock;
using Star = Platformer.Obstacles.Star;
// LDtk
using LDtkUnity;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public class ScoreTracker : MonoBehaviour {

        public static Vector2Int CheckpointID = new Vector2Int(6, 0);
        public static Vector2Int StarID = new Vector2Int(5, 3);

        [SerializeField, ReadOnly] private float m_Time = 0f;
        [SerializeField] private Text m_ScoreText;
        public string ClockTime {
            get {
                return (TimeSpan.FromSeconds(m_Time)).ToString();
            }
        }

        [SerializeField, ReadOnly] private int m_TotalDeaths = 0;

        [SerializeField, ReadOnly] private int m_TotalStars = 0;
        [SerializeField, ReadOnly] private int m_MaxStars = 0;

        [System.Serializable]
        public class StarTracker {
            public Star TrackedStar;
            public float TimeCollected = -1f;

            public bool Collected => TimeCollected != -1f;

            public StarTracker(Star star) {
                TrackedStar = star;
                TimeCollected = -1f;
            }

            public void OnUpdate() {
                if (TrackedStar == null && TimeCollected == -1f) {
                    TimeCollected = Game.Ticks;
                }
            }

        }


        [System.Serializable]
        public class CheckpointTracker {

            public Checkpoint TrackedCheckpoint;
            public float TimeFirstReached;
            public float TotalTimeActive;
            public int Deaths;
            // Death Heat Map
            // Movement Heat Map

            public CheckpointTracker(Checkpoint checkpoint) {
                TrackedCheckpoint = checkpoint;
            }

            public void OnUpdate() {
                if (TrackedCheckpoint == null) { return; }
                TimeFirstReached = TrackedCheckpoint.FirstActivationTime;
                TotalTimeActive = TrackedCheckpoint.TotalTimeActive;
            }

        }

        [SerializeField]
        private List<StarTracker> m_Stars = new List<StarTracker>();

        [SerializeField]
        private List<CheckpointTracker> m_Checkpoints = new List<CheckpointTracker>();

        public void Init(List<Level> levels) {

            for (int i = 0; i < levels.Count; i++) {

                for (int j = 0; j < levels[i].Stars.Count; j++) {
                    Star star = levels[i].Stars[j].GetComponent<Star>();
                    m_Stars.Add(new StarTracker(star));
                }

                for (int j = 0; j < levels[i].Checkpoints.Count; j++) {
                    Checkpoint checkpoint = levels[i].Checkpoints[j].GetComponent<Checkpoint>();
                    m_Checkpoints.Add(new CheckpointTracker(checkpoint));
                }

            }

            m_MaxStars = m_Stars.Count;

        }

        void Update() {
            for (int i = 0; i < m_Checkpoints.Count; i++) {
                m_Checkpoints[i].OnUpdate();
            }
            for (int i = 0; i < m_Stars.Count; i++) {
                m_Stars[i].OnUpdate();
            }
            m_Time = Game.Ticks;
            m_TotalStars = m_Stars.FindAll(star => star.Collected).Count;

            m_ScoreText.text = ClockTime;
        }

        public void AddDeath() {
            m_TotalDeaths += 1;

            Checkpoint block = Game.MainPlayer.Respawn;
            for (int i = 0; i < m_Checkpoints.Count; i++) {
                if (block == m_Checkpoints[i].TrackedCheckpoint) {
                    m_Checkpoints[i].Deaths += 1;
                    return;
                }
            }

        }

        public void AddStar(Star star) {
            for (int i = 0; i < m_Stars.Count; i++) {
                if (star == m_Stars[i].TrackedStar) {
                    m_Stars[i].TimeCollected = Game.Ticks;
                    return;
                }
            }
        }

    //     public void TempCollectStar(Star star) {

    //     }

    //     public void ClearTempStarCollection() {
            

    //     }

    //     public void FinalizeStarCollection() {
    //         for (int i = 0; i < m_TempCollectedStars.Count; i++) {
    //             if (star == m_Stars[i].TrackedStar) {
    //                 m_Stars[i].TimeCollected = Game.Ticks;
    //                 return;
    //             }
    //         }
    //     }

    }

}