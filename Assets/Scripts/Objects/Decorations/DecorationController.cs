/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class DecorationController : MonoBehaviour {

        #region Variables.

        //
        public DecorationPiece[] m_Pieces;

        // public Timer[] m_Timers;

        [SerializeField]
        private float m_Interval = 1f;

        [SerializeField]
        private float m_Rotation;

        #endregion

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

        void Start() {
            // m_Timers = new Timer[m_Pieces.Length];
            for (int i = 0; i < m_Pieces.Length; i++) {
                m_Pieces[i].Animation.Loop = false;
                m_Pieces[i].Animation.AnimationTimer.Set(Random.Range(0.02f, m_Interval));
            }
        }

        // Runs once every fixed interval.
        void FixedUpdate() {

            for (int i = 0; i < m_Pieces.Length; i++) {

                m_Pieces[i].spriteRenderer.transform.Animate(m_Pieces[i].Animation, Time.fixedDeltaTime);

                if (m_Pieces[i].Animation.AnimationTimer.Ratio == 1f) {
                    m_Pieces[i].Animation.AnimationTimer.Start(Random.Range(m_Interval * 0.8f, m_Interval * 1.2f));
                    // Because for some reason, it counts down, not up.
                    m_Pieces[i].Animation.AnimationTimer.Stop();

                }

            }

        }


    }

}
