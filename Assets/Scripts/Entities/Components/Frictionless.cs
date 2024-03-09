/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

/* --- Definitions --- */
using IReset = Platformer.Entities.Utility.IReset;
using CharacterController = Platformer.Character.CharacterController;
// using TrailAnimator = Gobblefish.Animation.TrailAnimator;

namespace Platformer.Entities.Components {

    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
    public class Frictionless : MonoBehaviour {

        //
        private BoxCollider2D m_BoxCollider;
        
        //
        private Rigidbody2D m_Body;

        void Start() {
            m_BoxCollider = GetComponent<BoxCollider2D>();
            m_Body = GetComponent<Rigidbody2D>();
            m_Body.gravityScale = 0f;
            m_Body.angularDrag = 0.05f;
            Freeze();
        }

        void Update() {
            Release();
            m_Body.velocity *= 0.99f;
        }

        protected virtual void Release() {
            m_Body.constraints = RigidbodyConstraints2D.None;
            m_Body.gravityScale = 0f;
        }

        protected virtual void Freeze() {
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_Body.gravityScale = 0f;
        }
        
    }

}