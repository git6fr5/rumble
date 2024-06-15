/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    using Gobblefish;

    ///<summary>
    ///
    ///<summary>
    // TODO: Add the Jelly.
    // TODO: Add the Springs.
    [RequireComponent(typeof(Entity))]
    public class Bouncing : MonoBehaviour {

        #region Enumerations.

        public enum BounceState {
            None,
            Tensing,
            Releasing,
        }

        #endregion

        #region Variables

        /* --- Constants --- */

        // The default bounce speed.
        private const float BOUNCE_SPEED = 20f; // 26f;

        // The jump speed for missed bounces.
        private const float MISSED_BOUNCE_SPEED = 8f; // 18f;

        /* --- Members --- */

        // The threshold between which
        private Entity m_Entity;

        // Whether this is bouncing or releasing.
        [SerializeField]
        private BounceState m_BounceState = BounceState.None;

        // The pause timer.
        [SerializeField]
        protected Timer m_BounceTimer = new Timer(0f, 0f);

        // The curve that controls the motion towards the nadir.
        [SerializeField]
        private AnimationCurve m_SinkCurve;

        // The scale of the sinking.
        [SerializeField]
        private float m_SinkDuration = 0.7f;

        // The scale of the sinking.
        [SerializeField]
        private float m_SinkScale = 0.7f;

        // The point during sinking after which the character's jump is "clamped".
        [SerializeField, Range(0f, 1f)]
        private float m_ClampJumpThreshold;

        // The speed with which this moves away from its zenith.
        [SerializeField]
        private float m_RiseDuration = 0.6f;

        // The curve that controls the motion towards the zenith.
        [SerializeField]
        private AnimationCurve m_RiseCurve;

        // The point during rising after which a bounce is considered "missed".
        [SerializeField, Range(0f, 1f)]
        private float m_MissedBounceThreshold;

        private MovementAnimator m_SinkAnimator;
        private MovementAnimator m_RisingAnimator;

        #endregion

        void Awake() {
            m_Entity = GetComponent<Entity>();
            // m_Origin = transform.localPosition;

            if (Application.isPlaying) {

                GameObject animatorObject = new GameObject("animator object");
                animatorObject.transform.SetParent(transform.parent);
                m_SinkAnimator = MovementAnimator.New(animatorObject, m_SinkCurve, m_SinkScale);
                m_RisingAnimator = MovementAnimator.New(animatorObject, m_RiseCurve, m_SinkScale);

            }

        }

        void FixedUpdate() {
            bool finished = m_BounceTimer.TickDown(Time.fixedDeltaTime);

            // Whenever the path timer hits 0.
            if (finished) {

                switch (m_BounceState) {
                    case BounceState.Tensing:
                        CheckPreemptiveBounce();
                        m_BounceState = BounceState.Releasing;
                        transform.localPosition = m_Entity.Origin + Vector3.down * m_SinkScale;
                        m_BounceTimer.Start(m_RiseDuration);
                        break;
                    case BounceState.Releasing:
                        m_BounceState = BounceState.None;
                        transform.localPosition = m_Entity.Origin;
                        break;
                    default:
                        break;
                }

            }

            // What to do for each state.
            switch (m_BounceState) {
                case BounceState.Tensing:
                    WhileTensing(Time.fixedDeltaTime);
                    break;
                case BounceState.Releasing:
                    WhileReleasing(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        public void ClampMainPlayerJump() {
            Platformer.Character.CharacterController character = Platformer.PlayerManager.Character;
            character.Default.ClampJump(true);
        }

        public void OnStartTensing() {
            if (m_BounceState != BounceState.Tensing || (m_BounceState == BounceState.Releasing && m_BounceTimer.InverseRatio > m_MissedBounceThreshold)) {
                m_BounceState = BounceState.Tensing;
                m_BounceTimer.Start(m_SinkDuration);
            }
        }

        private void WhileTensing(float dt) {
            MoveTransform(dt, m_BounceTimer, m_SinkAnimator, Vector3.down);

            if (m_BounceTimer.InverseRatio > m_ClampJumpThreshold) {
                PreemptiveClamp();
            }

        }

        private void WhileReleasing(float dt) {
            MoveTransform(dt, m_BounceTimer, m_RisingAnimator, Vector3.up);

            OldCheckBounce();

            float distance = (transform.localPosition - m_Entity.Origin).magnitude;
            if (m_BounceTimer.InverseRatio > m_MissedBounceThreshold) {
                MissedBounce();
            }

        }

        private void MoveTransform(float dt, Timer timer, MovementAnimator animator, Vector3 direction) {
            float normalizedDeltaTime = dt / timer.MaxValue;
            float ds = animator.GetStepDistance(timer.InverseRatio, normalizedDeltaTime);

            transform.localPosition += ds * direction;

            if (m_Entity != null) {
                foreach (var col in m_Entity.CollisionContainer) {
                    col.localPosition += ds * direction;
                }
            }

        }

        // Clamps the characters jump after the platform has gone down a certain distance.
        private void PreemptiveClamp() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    character.Default.ClampJump(true);
                    if (character.Input.Actions[0].Released) {
                        character.Default.OnExternalJump(character, character.Default.JumpSpeed);
                        character.Default.ClampJump(false);
                    }
                }
            }
        }

        // If the character has pressed the jump key/ is holding the jump key while the
        // bouncy platform has preemptively clamped the character.
        private void CheckPreemptiveBounce() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    if (true || character.Input.Actions[0].Held) {
                        print("pre-emptive bounce");
                        character.Default.OnExternalJump(character, BOUNCE_SPEED);
                        character.Default.ClampJump(false);
                    }

                }
            }
        }

        // Bounce a character that did not pre-emptively bounce if it
        // PRESSES jump while the platform is releasing.
        private void CheckBounce() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    character.Default.OnExternalJump(character, character.Default.JumpSpeed + BOUNCE_SPEED);
                    character.Default.ClampJump(false);
                }
            }
        }

        private void OldCheckBounce() {
            print(m_Entity.CollisionContainer.Count);
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    if (true || character.Input.Actions[0].Held) {
                        print("releasing bounce");
                        character.Default.OnExternalJump(character, BOUNCE_SPEED);
                        character.Default.ClampJump(false);
                    }
                }
            }
        }

        // For characters that missed a bounce.
        private void MissedBounce() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    print("missed bounce");
                    character.Default.OnExternalJump(character, MISSED_BOUNCE_SPEED);
                    character.Default.ClampJump(false);
                }
            }
            m_Entity.ForceClearContainer();
        }

    }
}
