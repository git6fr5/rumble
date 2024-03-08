/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.U2D;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using Game = Platformer.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour {

        protected Rigidbody2D m_Body;

        public Projectile CreateInstance() {
            Projectile projectile = Instantiate(gameObject).GetComponent<Projectile>();
            projectile.transform.position = transform.position;
            return projectile;
        }

        void Awake() {
            m_Body = GetComponent<Rigidbody2D>();
        }

        public virtual void Fire(float speed, Vector2 direction) {
            gameObject.SetActive(true);
            m_Body.Move(direction.normalized * 0.5f);
            m_Body.SetVelocity(speed * direction);
        }

        public void Fire(float speed, Vector2 direction, float torque) {
            Fire(speed, direction);
            m_Body.AddTorque(torque);
        }

        public void DeleteSelf() {
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
