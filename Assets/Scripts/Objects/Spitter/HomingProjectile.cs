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

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class HomingProjectile : Projectile {

        [SerializeField]
        private float m_Acceleration = 7f;

        public override void Fire(float speed, Vector2 direction) {
            gameObject.SetActive(true);
            direction = (Game.MainPlayer.transform.position - transform.position).normalized;
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
            m_Body.Move(direction.normalized * 0.5f);
            m_Body.SetVelocity(speed * direction);
        }

        void FixedUpdate() {
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, m_Body.velocity);

            Vector2 deltaVelocity = m_Acceleration * Time.fixedDeltaTime * (Game.MainPlayer.transform.position - transform.position).normalized;
            m_Body.AddVelocity(deltaVelocity);
        }

    }

}
