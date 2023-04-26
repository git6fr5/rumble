/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    ///<summary>
    /// Decorates a level with grass.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Arrow : MonoBehaviour {

        #region Variables.

        /* --- Components --- */
        
        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        
        /* --- Members --- */
        
        [SerializeField]
        private float m_Rotation;

        // The period by which this animates.
        public static float Period = 1f;

        // The amplitude that this oscillates with.
        public static float Amplitude = 0.1f;

        #endregion

        // Initalizes from the LDtk files.
        public virtual void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

        void Update() {
            transform.localScale = (Mathf.Sin(Period * Game.Physics.Time.Ticks) * Amplitude + 1f - Amplitude) * new Vector3(1f, 1f, 1f);
        }

    }

}
