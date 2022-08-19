/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Character;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class GhostBlock : MonoBehaviour {

        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        [SerializeField] private bool m_Touched;
        [SerializeField] private Orb m_GhostOrb;

        void Start() {
            m_GhostOrb.Palette.SetSimple(m_SpriteRenderer.material);
            m_Body.angularDrag = 0.05f;
            Freeze();
        }

        void Update() {
            if (Game.MainPlayer.Ghost.Enabled) {
                Release();
            }
            else {
                Freeze();
            }
            m_Body.velocity *= 0.99f;
        }

        protected virtual void Release() {
            if (m_Touched) {
                m_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else {
                m_Body.constraints = RigidbodyConstraints2D.None;
            }
            m_Body.gravityScale = 0f;
        }

        protected virtual void Freeze() {
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_Body.gravityScale = 0f;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider) {
            if (collider.GetComponent<CharacterState>() != null) {
                m_Touched = true;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collider) {
            if (collider.GetComponent<CharacterState>() != null) {
                m_Touched = false;
            }
        }

    }

}