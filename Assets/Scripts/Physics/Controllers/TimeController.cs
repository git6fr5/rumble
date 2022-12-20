/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// Controls the time of the game.
    ///<summary>
    public class TimeController : MonoBehaviour {

        #region Fields.

        /* --- Constants --- */

        // The threshold above which a ramp stop starts ramping up.
        public const float RAMP_THRESHOLD = 0.5f;

        // The default time scale.
        public const float DEFAULT_TIMESCALE = 1f;

        // The paused time scale.
        public const float PAUSED_TIMESCALE = 0f;

        /* --- Members --- */

        // Whether a ramp stop is currently being run.
        [HideInInspector] 
        private Timer m_RampStopTimer = new Timer(0f, 0f);

        // Whether a hit stop is currently being run.
        [HideInInspector] 
        private Timer m_HitStopTimer = new Timer(0f, 0f);
        
        #endregion

        // Runs once per frame.
        void Update() {
            if (m_RampStopTimer.Active) {
                UpdateRampStop();
            }
            else if (m_HitStopTimer.Active) {
                UpdateHitStop();
            }
            else {
                m_TimeScale = DEFAULT_TIMESCALE;
            }
        }

        // Run a hit stop.
        public static void RunHitStop(int frames = 16) {
            m_TimeScale = PAUSED_TIMESCALE;
            m_HitStopTimer.Start(frames);
        }

        private void UpdateHitStop() {
            m_HitStopTimer.TickDown(1f);
        }

        // Run a ramp stop.
        public static void RunRampStop(int frames = 128) {
            m_TimeScale = PAUSED_TIMESCALE;
            m_RampIncrement.Start(frames);
        }

        private void UpdateRampStop() {
            m_RampStopTimer.TickDown(1f);
            if (m_RampStopTimer.InverseRatio > RAMP_THRESHOLD) {
                m_TimeScale = m_RampStopTimer.InverseRatio;
            }
        }

    }

}