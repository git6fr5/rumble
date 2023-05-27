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

namespace Platformer.Objects.Orbs {

    ///<summary>
    ///
    ///<summary>
    public class StatueAnimator : MonoBehaviour {

        [SerializeField]
        private bool m_Construct;

        [SerializeField]
        private bool m_Active;

        [SerializeField]
        private float m_Ticks = 0f;

        [SerializeField]
        private float m_Duration = 3f;

        [SerializeField]
        private float m_EmissionTicks = 0f;

        [SerializeField]
        private float m_EmissionDuration = 1f;

        [SerializeField]
        private TransformAnimator[] m_Transforms;

        [SerializeField]
        private GameObject m_OrbLight;

        [SerializeField]
        private GameObject m_BottomLight;

        [SerializeField]
        private SpriteRenderer[] m_Emissions;

        public void Initialize() {
            for (int i = 0; i < m_Transforms.Length; i++) {
                m_Transforms[i].enabled = false;
                m_Transforms[i].Animation.duration = m_Duration;
            }
            for (int i = 0; i < m_Emissions.Length; i++) {
                m_Emissions[i].material.SetFloat("_Threshold", 1f);
            }
        }

        public void Activate(bool active) {
            if (active) {
                m_Construct = true;
            }
            m_Active = active;
        }

        void FixedUpdate() {
            Animate(Time.fixedDeltaTime);
        }

        public void Animate(float dt) {
            if (m_Construct) {
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

            if (m_Ticks / m_Duration >= 1f) {


                bool changedVal = true;
                if (m_Active) {
                    if (!m_OrbLight.activeSelf) {
                        m_OrbLight.SetActive(true);
                    }
                    // if (!m_BottomLight.activeSelf) {
                    //     m_BottomLight.SetActive(true);
                    // }
                    m_EmissionTicks += Time.fixedDeltaTime;
                    if (m_EmissionTicks > m_EmissionDuration) {
                        m_EmissionTicks = m_EmissionDuration;
                        changedVal = false;
                    }
                }
                else {
                    if (m_OrbLight.activeSelf) {
                        m_OrbLight.SetActive(false);
                    }
                    // if (m_BottomLight.activeSelf) {
                    //     m_BottomLight.SetActive(false);
                    // }
                    m_EmissionTicks -= Time.fixedDeltaTime;
                    if (m_EmissionTicks < 0f) {
                        m_EmissionTicks = 0f;
                        changedVal = false;
                    }
                }

                if (changedVal) {
                    for (int i = 0; i < m_Emissions.Length; i++) {
                        m_Emissions[i].material.SetFloat("_Threshold", 1f - m_EmissionTicks / m_EmissionDuration);
                    }
                }                

            }

        }
    }

}
