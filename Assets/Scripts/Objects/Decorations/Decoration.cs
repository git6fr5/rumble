/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using IInitializable = Platformer.Levels.Entities.IInitializable;
using IRotatable = Platformer.Levels.Entities.IRotatable;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class Decoration : MonoBehaviour, IInitializable, IRotatable {

        [SerializeField]
        private float m_Rotation;

        [SerializeField, ReadOnly]
        private Vector3 m_Origin;

        // Initializes this entity.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin
            transform.localPosition = worldPosition;
            m_Origin = worldPosition;

            // Set the layer. Note the sprite renderers for this will have to be set manually.
            gameObject.layer = Game.Physics.CollisionLayers.DecorLayer;

        }

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

    }

}
