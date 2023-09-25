/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Triggers {

    [DefaultExecutionOrder(1000), RequireComponent(typeof(Entity))]
    public class OnTouch : MonoBehaviour {

        [HideInInspector]
        private Entity m_Entity;

        // The number of hit stop frames when touched
        [SerializeField]
        private int m_HitStopFrames = 8;

        // The sound that plays when this orb is collected.
        [SerializeField] 
        private AudioClip m_OnTouchedSound;

        [SerializeField]
        private VisualEffect m_OnTouchEffect;

        [SerializeField]
        private VisualEffect m_OnTouchGroundEffect;

        [SerializeField]
        private UnityEvent m_TouchEvent;

        [SerializeField]
        private UnityEvent m_TouchGroundEvent;

        [SerializeField]
        private bool m_DisableGroundTouching = true;

        // Runs once before the first frame.
        void Awake() {
            m_Entity = GetComponent<Entity>();
            m_Entity.SetAsTrigger(true);
        }

        // Runs everytime something enters this trigger area.
        void OnTriggerEnter2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                Game.Audio.Sounds.PlaySound(m_OnTouchedSound, 0.05f);
                Game.Physics.Time.RunHitStop(m_HitStopFrames);
                
                if (m_OnTouchEffect != null) {
                    m_OnTouchEffect.Play();
                }
                
                m_TouchEvent.Invoke();
                
            }
            else if (!m_DisableGroundTouching) {
                bool hitGround = collider.gameObject.layer == Game.Physics.CollisionLayers.PlatformLayer;
                if (hitGround) {

                    if (m_OnTouchGroundEffect != null) {
                        Game.Audio.Sounds.PlaySound(m_OnTouchedSound, 0.05f);
                        m_OnTouchGroundEffect.Play();
                    }

                    m_TouchGroundEvent.Invoke();
                }
            }
        }

    }

}