// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.Tests {

    public class Plant : MonoBehaviour {

        // The sprite shape controller. attached to this platform.
        [SerializeField]
        private Stem m_Stem = null;
        public Stem stem => m_Stem;
        public Vector3[] stemPositions => m_Stem.positions;
        public Vector3 stemOrigin => m_Stem.transform.position;
        
        //
        [SerializeField]
        private Leaf[] m_Leaves;

        //
        [SerializeField]
        private Bud[] m_Buds;

        // The amount that this plant has grown.
        [SerializeField, Range(0f, 1f)]
        private float m_PercentGrown = 0f;
        private float m_CachePercentGrown = 0f; 

        [SerializeField]
        private bool m_Grow = false;

        [SerializeField]
        private float m_PercentPerSecond = 1f;

        void Start() {
            UpdateGrowthState();
        }

        public void Grow(bool grow) {
            m_Grow = grow;
        }

        void FixedUpdate() {
            if (m_Grow && m_PercentGrown < 1f) {
                m_PercentGrown += Time.fixedDeltaTime * m_PercentPerSecond;
            }
            else if (!m_Grow && m_PercentGrown > 0f) {
                m_PercentGrown -= Time.fixedDeltaTime * m_PercentPerSecond;
            }

            if (m_PercentGrown != m_CachePercentGrown) {
                UpdateGrowthState();
            }
        }

        private void UpdateGrowthState() {
            m_Stem.Grow(m_PercentGrown);
            foreach (Leaf leaf in m_Leaves) {
                leaf.Grow(m_PercentGrown);
            }
            foreach (Bud bud in m_Buds) {
                bud.Grow(m_PercentGrown);
            }
        }
    }

}
