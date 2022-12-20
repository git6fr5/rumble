/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Utilities;
using Platformer.Character;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ShadowBlock : MonoBehaviour {

        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        [SerializeField] private SpriteRenderer m_SpriteRenderer;

        [SerializeField] private BoxCollider2D m_CollisionBox;
        [SerializeField] private BoxCollider2D m_TriggerBox;

        [SerializeField] private bool m_Locked;
        [SerializeField] private CharacterState m_Touched;

        [SerializeField] private Orb m_ShadowOrb;

        [SerializeField] private AudioClip m_ActivateSound;

        void Start() {
            m_ShadowOrb.Palette.SetSimple(m_SpriteRenderer.material);
            Freeze();
        }

        void Update() {
            if (Game.MainPlayer.Shadow.Enabled) {
                m_SpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f) * 1.25f;
            } 
            else {
                m_SpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (m_Locked) {
                //
                m_SpriteRenderer.transform.eulerAngles += Vector3.forward * Time.deltaTime * 720f;
            }
            else {
                m_SpriteRenderer.transform.eulerAngles = Vector3.zero;
            }
            
            if (!m_Locked && m_Touched != null && m_Touched.Shadow.Dashing) {
                m_Touched.Shadow.Lock(m_Touched, this);
                m_Touched = null;
            }
        }

        protected virtual void Freeze() {
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_Body.gravityScale = 0f;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            if (character != null && character.IsPlayer) {
                m_Touched = character;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            if (character != null && character.IsPlayer) {
                m_Touched = null;
            }
        }

        public void Lock() {
            m_Locked = true;
            m_CollisionBox.enabled = false;
            m_TriggerBox.enabled = false;
            SoundManager.PlaySound(m_ActivateSound, 0.15f);
        }

        public void Unlock() {
            m_Locked = false;
            m_CollisionBox.enabled = true;
            m_TriggerBox.enabled = true;

        }

    }

}