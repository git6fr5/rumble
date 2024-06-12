/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.Tests {

    public class Leaf : MonoBehaviour {

        public const float DEFAULT_GROW_TIME = 0.4f;

        [SerializeField]
        private AnimationCurve m_GrowthCurve;
        
        // Whether this leaf has been grown or not.
        public bool Grown => gameObject.activeSelf;

        // The percentage value that this is grown at.
        [SerializeField]
        private float m_PercentStartGrowingAt;
        
        // The percentage value that this is grown at.
        [SerializeField]
        private float m_PercentFinishGrowingAt;

        public bool autoConfigure;

        Vector3 baseScale;

        void Start() {

            baseScale = transform.localScale;

            Plant plant = transform.parent.GetComponent<Plant>();
            if (plant && autoConfigure) {

                Vector3 stemOrigin = plant.stemOrigin;
                Vector3[] positions = plant.stemPositions;
                
                float minDist0 = Mathf.Infinity;
                float minDist1 = Mathf.Infinity;
                int index0 = -1;
                int index1 = -1;
                float dist;
                for (int i = 0; i < positions.Length; i++) {

                    dist = (positions[i] + stemOrigin - transform.position).sqrMagnitude;

                    if (dist < minDist1) {
                        index1 = i;
                        minDist1 = dist;

                        if (minDist1 < minDist0) {
                            index1 = index0;
                            index0 = i;

                            minDist1 = minDist0;
                            minDist0 = dist;
                        }

                    }

                }

                if (index0 > 0 && index1 > 0) {

                    float r1 = plant.stem.GetPercentFromIndex(index0);
                    float r2 = plant.stem.GetPercentFromIndex(index1);

                    float r = r1 * (1f - minDist0 / (minDist0 + minDist1)) + r2 * (1f - minDist1 / (minDist0 + minDist1));
                    m_PercentStartGrowingAt = r;
                    m_PercentFinishGrowingAt = Mathf.Min(r + DEFAULT_GROW_TIME, 1f);

                }
                

            }

        }
        
        public void Grow(float percent) {
            if (m_PercentStartGrowingAt == 0f) {
                return;
            }

            if (gameObject.activeSelf && percent < m_PercentStartGrowingAt) {
                gameObject.SetActive(false);
                return;
            }
            else if (!gameObject.activeSelf && percent >= m_PercentStartGrowingAt) {
                gameObject.SetActive(true);
            }

            if (m_PercentFinishGrowingAt != m_PercentStartGrowingAt) { 
                float ratio = (percent - m_PercentStartGrowingAt) / (m_PercentFinishGrowingAt - m_PercentStartGrowingAt);
                if (ratio > 1f) { ratio = 1f; }
                else if (ratio < 0f) { ratio = 0f; }
                
                transform.localScale = m_GrowthCurve.Evaluate(ratio) * baseScale;
            }

        }

    }

}
