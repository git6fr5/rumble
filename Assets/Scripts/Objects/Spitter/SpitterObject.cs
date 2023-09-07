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
using Projectile = Platformer.Objects.Spitters.Projectile;

namespace Platformer.Objects.Spitters {

    ///<summary>
    ///
    ///<summary>
    public class SpitterObject : MonoBehaviour {

        #region Variables

        /* --- Components --- */
        
        [SerializeField]
        protected SpriteRenderer m_SpriteRenderer = null;

        /* --- Member --- */

        // The base position of the object.
        [SerializeField, ReadOnly] 
        protected Vector3 m_Origin = new Vector3(0f, 0f, 0f);

        // The direction that this object spits in.
        protected Vector2 SpitDirection => Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * Vector2.right;

        // The timer with which this spits things.
        [SerializeField]
        private Timer m_SpitTimer = new Timer(0f, 0f);

        // The duration this spits things with.
        [SerializeField]
        private float m_SpitInterval = 2f;

        // The speed with which this spits the projectile.
        [SerializeField] 
        private float m_SpitSpeed = 10f;

        // The projectile that this thing fires.
        [SerializeField]
        private Projectile m_SpitProjectile = null;

        // The effect that plays when this spike shatters.
        [SerializeField] 
        private VisualEffect m_SpitEffect;
        
        // The effect that plays when the spike shatters.
        [SerializeField] 
        private AudioClip m_SpitSound;
        
        #endregion

        #region Methods.

         // Runs once before the first frame.
        void Awake() {
            // Cache the origin
            m_Origin = transform.position;

            // Collision settings.
            // m_Hitbox = GetComponent<CircleCollider2D>();
            // m_Hitbox.isTrigger = true;
            gameObject.layer = Game.Physics.CollisionLayers.SpikeLayer;

            // Temporarily
            m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.SpikeLayer;
            m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.SpikeOrder;

            // Start the spit timer.
            m_SpitTimer.Start(m_SpitInterval);

        }

        private void FixedUpdate() {
            bool finished = m_SpitTimer.TickDown(Time.fixedDeltaTime);
            if (finished) {
                Spit();
                m_SpitTimer.Start(m_SpitInterval);
            }
        }

        protected virtual void Spit() {
            Projectile projectile = m_SpitProjectile.CreateInstance();
            projectile.Fire(m_SpitSpeed, SpitDirection);
            // Game.Visuals.Effects.PlayEffect(m_SpitEffect);
            Game.Audio.Sounds.PlaySound(m_SpitSound, 0.15f);
        }

        #endregion

    }

}
