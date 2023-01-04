/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    public class CrumblingPlatform : PlatformObject {

        #region Enumerations.

        public enum CrumbleState {
            None,
            Crumbling,
            Reforming
        }

        #endregion

        #region Variables.

        // Whether this platform is crumbling.
        [SerializeField] 
        private CrumbleState m_CrumbleState = CrumbleState.None;
        
        // The duration this is crumbling for.
        [SerializeField] 
        private float m_CrumbleDuration = 0.5f;

        // The time it takes for this to reform.
        [SerializeField] 
        private float m_ReformDuration = 1f;

        // Tracks whether        
        [SerializeField] 
        private Timer m_CrumbleTimer = new Timer(0f, 0f);

        // The base strength this shakes with while crumbling.
        [SerializeField] 
        private float m_ShakeStrength = 0.12f;
        
        // The adjusted shake strength.
        private float Strength => m_ShakeStrength * m_CrumbleTimer.InverseRatio;

        // The sound this plays while crumbling
        [SerializeField] 
        private AudioClip m_WhileCrumblingSound = null;
        
        // The sound this plays on crumbling.
        [SerializeField] 
        private AudioClip m_OnCrumbleSound = null;

        // The sound this plays while reforming.
        // [SerializeField] 
        // private AudioClip m_OnReformBlinkSound = null;

        // The sound this plays on reforming.
        [SerializeField] 
        private AudioClip m_OnReformSound = null;

        #endregion

        #region Methods.

        // Runs once every frame.
        // Having to do this is a bit weird.
        protected override void Update() {
            base.Update();
            
            // What to do for each state.
            switch (m_CrumbleState) {
                case CrumbleState.None:
                    if (m_PressedDown) { OnStartCrumble(); }
                    break;
                case CrumbleState.Crumbling:
                    transform.Shake(m_Origin, Strength); // Should this be in while crumbling?
                    break;
                default:
                    break;
            }

        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_CrumbleTimer.TickDown(Time.fixedDeltaTime);

            // Whenever the crumble timer hits 0.
            if (finished) {

                switch (m_CrumbleState) {
                    case CrumbleState.Crumbling:
                        OnCrumble();
                        break;
                    case CrumbleState.Reforming:
                        OnReform();
                        break;
                    default:
                        break;
                }
            }

            // What to do for each state.
            switch (m_CrumbleState) {
                case CrumbleState.Crumbling:
                    WhileCrumbling(Time.fixedDeltaTime);
                    break;
                case CrumbleState.Reforming:
                    WhileReforming(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        private void OnStartCrumble() {
            m_CrumbleTimer.Start(m_CrumbleDuration);
            m_CrumbleState = CrumbleState.Crumbling;
        }

        private void OnCrumble() {
            m_Hitbox.enabled = false;
            m_SpriteShapeRenderer.enabled = false;
            m_CrumbleState = CrumbleState.Reforming;
            
            m_CrumbleTimer.Start(m_ReformDuration);
            Game.Audio.Sounds.PlaySound(m_OnCrumbleSound, 0.15f);
        }

        private void OnReform() {
            m_Hitbox.enabled = true;
            m_SpriteShapeRenderer.enabled = true;
            m_CrumbleState = CrumbleState.None;

            Game.Audio.Sounds.PlaySound(m_OnReformSound, 0.15f);
        }

        private void WhileCrumbling(float dt) {
            Game.Audio.Sounds.PlaySound(m_WhileCrumblingSound, Mathf.Sqrt(m_CrumbleTimer.InverseRatio) * 1f);
        }

        private void WhileReforming(float dt) {

        }

        #endregion

    }

}
