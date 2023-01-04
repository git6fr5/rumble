/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Objects.Spinners {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Collider2D))]
    public class SpinnerObject : MonoBehaviour {

        #region Variables

        /* --- Components --- */
        
        // The sprite renderer attached to this game object.
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();

        // The sprite renderer attached to this game object.
        protected Collider2D m_Hitbox => GetComponent<Collider2D>();

        /* --- Member --- */

        // The base position of the object.
        [SerializeField, ReadOnly] 
        protected Vector3 m_Origin = new Vector3(0f, 0f, 0f);

        // The z value rotation of the object.
        [SerializeField] 
        protected float m_Spin = 0f;

        // The effect that plays when the spike shatters.
        [SerializeField] 
        private float m_SpinSpeed = 90f;
        
        #endregion

        #region Methods.

        // Runs once before the first frame.
        private void Start() {
            m_Origin = transform.position;
        }

        // Initalizes from the LDtk files.
        public virtual void Init(float spin) {
            m_Spin = spin;
            foreach (Transform child in transform) {
                SpinnerObject spinObject = child.GetComponent<SpinnerObject>() ;
                if (spinObject != null) {
                    spinObject.Init(spin);
                }
            }
        }

        private void FixedUpdate() {
            transform.Rotate(m_Spin * m_SpinSpeed, Time.fixedDeltaTime);
        }

        #endregion

    }

}
