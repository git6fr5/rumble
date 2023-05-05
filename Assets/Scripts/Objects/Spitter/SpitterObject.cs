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
using IInitializable = Platformer.Levels.Entities.IInitializable;
using IRotatable = Platformer.Levels.Entities.IRotatable;
using IOffsetable = Platformer.Levels.Entities.IOffsetable;

namespace Platformer.Objects.Spitters {

    ///<summary>
    ///
    ///<summary>
    public class SpitterObject : MonoBehaviour, IInitializable, IRotatable, IOffsetable {

        #region Variables

        /* --- Components --- */
        
        [SerializeField]
        protected SpriteRenderer m_SpriteRenderer = null;

        /* --- Member --- */

        // The euler angles rotation of this spike.
        public Vector3 Rotation => new Vector3(0f, 0f, m_Rotation);
        
        // The z value rotation of the object.
        [SerializeField] 
        protected float m_Rotation = 0f;
        
        // The base position of the object.
        [SerializeField, ReadOnly] 
        protected Vector3 m_Origin = new Vector3(0f, 0f, 0f);

        // The direction that this object spits in.
        protected Vector2 m_SpitDirection => Quaternion.Euler(0f, 0f, m_Rotation) * Vector2.down;

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
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin
            transform.position = worldPosition;
            m_Origin = worldPosition;

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

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

        public void SetOffset(int offset) {
            m_SpitTimer.Start((float)offset / 4f * m_SpitInterval);
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
            projectile.Fire(m_SpitSpeed, m_SpitDirection);
            // Game.Visuals.Effects.PlayEffect(m_SpitEffect);
            Game.Audio.Sounds.PlaySound(m_SpitSound, 0.15f);
        }

        #endregion

    }

}
