/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Spitters {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour {

        protected Rigidbody2D m_Body => GetComponent<Rigidbody2D>();

        // The effect that plays when this spike shatters.
        [SerializeField] 
        private VisualEffect m_ShatterEffect;
        
        // The effect that plays when the spike shatters.
        [SerializeField] 
        private AudioClip m_ShatterSound;

        public Projectile CreateInstance() {
            Projectile projectile = Instantiate(gameObject).GetComponent<Projectile>();
            projectile.transform.position = transform.position;
            return projectile;
        }

        public virtual void Fire(float speed, Vector2 direction) {
            gameObject.SetActive(true);
            m_Body.Move(direction.normalized * 0.5f);
            m_Body.SetVelocity(speed * direction);
        }

        // Runs when another collider enters the trigger area.
        protected void OnTriggerEnter2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                character.Reset();
                Shatter();
            }
            else {
                bool hitGround = collider.gameObject.layer == LayerMask.NameToLayer("Ground");
                bool hitPlatform = collider.gameObject.layer == LayerMask.NameToLayer("Platform");
                if (hitGround || hitPlatform) {
                    Shatter();
                }
            }
        }

        protected void Shatter() {
            Game.Visuals.Effects.PlayEffect(m_ShatterEffect);
            Game.Audio.Sounds.PlaySound(m_ShatterSound, 0.15f);
            Destroy(gameObject);
        }

        public static void DeleteAll() {
            Projectile[] projectiles = (Projectile[])GameObject.FindObjectsOfType(typeof(Projectile));
            for (int i = 0; i < projectiles.Length; i++) {
                Destroy(projectiles[i].gameObject);
            }
        }

    }

}
