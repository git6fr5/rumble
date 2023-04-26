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
    public class Leaf : MonoBehaviour {

        #region Variables.

        /* --- Components --- */
        
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer = null;

        /* --- Parameters --- */

        // Whether this leaf has been grown or not.
        public bool Grown => gameObject.activeSelf;

        // The idle transform movement.
        [SerializeField]
        private TransformAnimation m_IdleTransformMovement = null;

        // The percentage value that this is grown at.
        [SerializeField]
        private float m_PercentStartGrowingAt;
        
        // The percentage value that this is grown at.
        [SerializeField]
        private float m_PercentFinishGrowingAt;
        
        #endregion

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_SpriteRenderer.transform.Animate(m_IdleTransformMovement, Time.fixedDeltaTime);
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
                
                transform.localScale = new Vector3(ratio, ratio, 1f);
            }

        }

    }

}
