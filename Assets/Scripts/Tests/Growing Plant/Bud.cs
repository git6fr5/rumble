// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Tests {

    public class Bud : MonoBehaviour {

        [SerializeField]
        private AnimationCurve m_GrowthCurve;

        public void Grow(float percent) {
            transform.localScale = m_GrowthCurve.Evaluate(percent) * new Vector3(1f, 1f, 1f);
        }

    }

}
