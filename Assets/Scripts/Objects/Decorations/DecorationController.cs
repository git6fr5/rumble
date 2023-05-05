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
using CharacterController = Platformer.Character.CharacterController;
using IInitializable = Platformer.Levels.Entities.IInitializable;
using IRotatable = Platformer.Levels.Entities.IRotatable;

namespace Platformer.Objects.Decorations {

    ///<summary>
    ///
    ///<summary>
    public class DecorationController : MonoBehaviour, IInitializable, IRotatable {

        #region Variables.

        // The pieces that need to be animated.
        public DecorationPiece[] m_AnimatedPieces;

        [SerializeField]
        private float m_Rotation;

        [SerializeField, ReadOnly]
        private Vector3 m_Origin;

        #endregion

        // Initializes this entity.
        public void Initialize(Vector3 worldPosition, float depth) {
            // Cache the origin
            transform.localPosition = worldPosition;
            m_Origin = worldPosition;

            // Set the layer. Note the sprite renderers for this will have to be set manually.
            gameObject.layer = Game.Physics.CollisionLayers.DecorLayer;

            // Set the animations to the start.
            for (int i = 0; i < m_AnimatedPieces.Length; i++) {
                m_AnimatedPieces[i].Animation.AnimationTimer.Set(Random.Range(0f, 3f));
            }

            variations = GetComponentsInChildren<DecorationVariation>();
            for (int i = 0; i < variations.Length; i++) {
                variations[i].Vary();
            }
            revary = false;

        }

        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

        void Start() {
            variations = GetComponentsInChildren<DecorationVariation>();
        }

        void FixedUpdate() {
            for (int i = 0; i < m_AnimatedPieces.Length; i++) {
                m_AnimatedPieces[i].spriteRenderer.transform.Animate(m_AnimatedPieces[i].Animation, Time.fixedDeltaTime);
            }

            if (revary) {
                for (int i = 0; i < variations.Length; i++) {
                    variations[i].Vary();
                }
                revary = false;
            }

        }

        public DecorationVariation[] variations;

        public bool revary = true;

    }

}
