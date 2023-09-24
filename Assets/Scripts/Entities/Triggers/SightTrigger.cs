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
using SpriteAnimator = Platformer.Visuals.Animation.SpriteAnimator;
using SpriteAnimation = Platformer.Visuals.Animation.SpriteAnimation;
using TrailAnimator = Platformer.Visuals.Animation.TrailAnimator;

namespace Platformer.Entities.Utility {

    [System.Serializable]
    public class SightEvent : UnityEvent<CharacterController> { }

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class SightTrigger : MonoBehaviour {

        [SerializeField]
        private SightEvent m_OnSightEvent;

        [SerializeField]
        private Vector3 m_Direction = new Vector3(0f, -1f, 0f);
        
        // Runs once every frame.
        private void Update() {
            CharacterController character = Game.Physics.Collisions.LineOfSight<CharacterController>(transform.position + m_Direction, m_Direction, Game.Physics.CollisionLayers.Solid);
            if (character != null) {
                m_OnSightEvent.Invoke(character);
            }
        }

    }

}
