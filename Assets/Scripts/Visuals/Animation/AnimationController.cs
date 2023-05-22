/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Visuals.Animation;

namespace Platformer.Visuals.Animation {

    ///<summary>
    ///
    ///<summary>
    public class AnimationController : MonoBehaviour {

        [SerializeField]
        private bool m_Play;

        [SerializeField]
        private float m_Ticks = 0;

        [SerializeField]
        private float m_Duration = 3;

        [SerializeField]
        private TransformAnimator[] m_Transforms;

        void Start() {
            for (int i = 0; i < m_Transforms.Length; i++) {
                m_Transforms[i].enabled = false;
                m_Transforms[i].Animation.duration = m_Duration;
            }
        }

        public void Play(bool play) {
            m_Play = play;
        }

        void FixedUpdate() {
            if (m_Play) {
                m_Ticks += Time.fixedDeltaTime;
                if (m_Ticks > m_Duration) {
                    m_Ticks = m_Duration;
                }
            }
            else {
                m_Ticks -= Time.fixedDeltaTime;
                if (m_Ticks < 0f) {
                    m_Ticks = 0f;
                } 
            }

            for (int i = 0; i < m_Transforms.Length; i++) {
                m_Transforms[i].Animation.Set(m_Ticks);
                m_Transforms[i].transform.localPosition = m_Transforms[i].Animation.GetPosition();
                m_Transforms[i].transform.localRotation = m_Transforms[i].Animation.GetRotation();
                m_Transforms[i].transform.localScale = m_Transforms[i].Animation.GetStretch(); 
            }
        }

    }

}
