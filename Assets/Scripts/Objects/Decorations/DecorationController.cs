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
using IRotatable = Platformer.Levels.Entities.IRotatable;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class DecorationController : MonoBehaviour, IRotatable {

        #region Variables.

        //
        public DecorationPiece[] m_Pieces;

        public DecorationPiece[] disconAnims;
        public Timer[] timers;
        public float[] approximateIntervalBetweenPlaying;
        public bool[] disconState;


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
            for (int i = 0; i < m_Pieces.Length; i++) {
                // m_Pieces[i].Animation.Loop = false;
                // m_Pieces[i].Animation.AnimationTimer.Set(Random.Range(0.02f, m_Interval));
                // m_Piece
                m_Pieces[i].Animation.AnimationTimer.Set(Random.Range(0f, 3f));
            }

            timers = new Timer[disconAnims.Length];
            disconState = new bool[disconAnims.Length];
            for (int i = 0; i < timers.Length; i++) {

                disconAnims[i].Animation.Loop = false;
                
                disconState[i] = false;

                timers[i] = new Timer(Random.Range(0f, approximateIntervalBetweenPlaying[i]), approximateIntervalBetweenPlaying[i]);
            }

        }

        // Runs once every fixed interval.
        void FixedUpdate() {

            for (int i = 0; i < timers.Length; i++) {
                if (disconState[i]) {
                    
                    disconAnims[i].spriteRenderer.transform.Animate(disconAnims[i].Animation, Time.fixedDeltaTime);

                    if (disconAnims[i].Animation.AnimationTimer.Ratio == 1f) {
                        disconAnims[i].Animation.AnimationTimer.Stop();
                        disconState[i] = !disconState[i];
                    }

                }
                else {

                    bool finished = timers[i].TickDown(Time.fixedDeltaTime);
                    if (finished) {
                        timers[i].Start((approximateIntervalBetweenPlaying[i] * Random.Range(0.8f, 1.2f)));
                        disconState[i] = !disconState[i];
                    }

                }
            }

            

            for (int i = 0; i < m_Pieces.Length; i++) {

                m_Pieces[i].spriteRenderer.transform.Animate(m_Pieces[i].Animation, Time.fixedDeltaTime);

                // if (m_Pieces[i].Animation.AnimationTimer.Ratio == 1f) {
                //     // m_Pieces[i].Animation.AnimationTimer.Start(Random.Range(m_Interval * 0.8f, m_Interval * 1.2f));
                //     // Because for some reason, it counts down, not up.
                //     m_Pieces[i].Animation.AnimationTimer.Stop();
                // }

            }

        }


    }

}
