/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
using Platformer;
using Platformer.LevelEditing;

using TransformAnimator = Gobblefish.Animation.TransformAnimator;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    public class MantaRay : MonoBehaviour {

        [SerializeField]
        private List<Renderer> m_Renderers;

        [SerializeField]
        private Gradient m_Gradient;

        [SerializeField]
        private AnimationCurve m_ScaleCurve;

        [SerializeField]
        private Vector3 m_Velocity = new Vector3(-1f, 0f, 0f);

        [SerializeField]
        private AnimationCurve m_VelocityCurve;

        [SerializeField]
        private TransformAnimator[] m_TransformAnimators;

        [SerializeField]
        private AnimatedRope m_AnimatedRope;

        void Start() {
            Randomize();
        }

        void FixedUpdate() {
            transform.localPosition += m_Velocity * Time.fixedDeltaTime;
        }

        public void Randomize() {
            float depth = Random.Range(-100f, 0f);

            int sortingOrder = (int)(depth * m_Renderers.Count);
            for (int i = 0; i < m_Renderers.Count; i++) {
                m_Renderers[i].sortingOrder = sortingOrder + i;
                
                if (m_Renderers[i].GetComponent<SpriteRenderer>() != null) {
                    m_Renderers[i].GetComponent<SpriteRenderer>().color =  m_Gradient.Evaluate(depth * -0.01f);
                }
                else {
                    m_Renderers[i].GetComponent<SpriteShapeRenderer>().color =  m_Gradient.Evaluate(depth * -0.01f);
                }

            }

            // m_Velocity *= 1f / (((-1f * depth) + 1f) / 5f);
            m_Velocity *= m_VelocityCurve.Evaluate(depth * -0.01f);
            transform.localScale *= m_ScaleCurve.Evaluate(depth * -0.01f);

            float val = m_AnimatedRope.GetDuration() / m_VelocityCurve.Evaluate(depth * -0.01f);
            m_AnimatedRope.SetDuration(val);
            for (int i = 0; i < m_TransformAnimators.Length; i++) {
                m_TransformAnimators[i].Animation.duration = val;
            }

        }

    }

}