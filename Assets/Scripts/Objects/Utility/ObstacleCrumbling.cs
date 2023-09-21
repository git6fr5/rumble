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

namespace Platformer.Objects {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(PlatformObject))]
    public class ObstacleCrumbling : MonoBehaviour {

        #region Enumerations.

        // A macro overview of the state.
        public enum CrumbleState {
            None,
            Crumbling,
            Reforming
        }

        #endregion

        #region Variables.

        // The threshold between which 
        [SerializeField]
        private PlatformObject m_Obstacle;

        [SerializeField]
        private Renderer m_Renderer;

        // Whether this platform is crumbling.
        [SerializeField] 
        private CrumbleState m_CrumbleState = CrumbleState.None;

        // The duration this is crumbling for.
        [SerializeField] 
        private float m_CrumbleDuration = 1f;

        // The duration this is crumbling for.
        [SerializeField] 
        private float m_CrumbleScale = 5f;

        [SerializeField]
        private AnimationCurve m_CrumbleCurve;

        // The time it takes for this to reform.
        [SerializeField] 
        private float m_ReformDuration = 2f;

        [SerializeField]
        private AnimationCurve m_ReformCurve;

        // Tracks whether        
        [SerializeField] 
        private Timer m_CrumbleTimer = new Timer(0f, 0f);

        // The sound this plays on crumbling.
        [SerializeField] 
        private AudioClip m_OnCrumbleSound = null;

        // The sound this plays on reforming.
        [SerializeField] 
        private AudioClip m_OnReformSound = null;

        #endregion

        #region Methods.

        // Runs once every frame.
        void Update() {
            
            // What to do for each state.
            switch (m_CrumbleState) {
                case CrumbleState.None:
                    if (m_Obstacle.Pressed) { OnStartCrumble(); }
                    break;
                case CrumbleState.Crumbling:
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
            m_Obstacle.EnableColliders(false);
            // m_Animator.gameObject.SetActive(false);
            m_CrumbleState = CrumbleState.Reforming;
            
            m_CrumbleTimer.Start(m_ReformDuration);
            Game.Audio.Sounds.PlaySound(m_OnCrumbleSound, 0.15f);


        }

        private void OnReform() {
            m_Obstacle.EnableColliders(true);
            // m_Animator.gameObject.SetActive(true);
            m_CrumbleState = CrumbleState.None;

            Game.Audio.Sounds.PlaySound(m_OnReformSound, 0.15f);
            // m_Animator.SetMaterialValue("_Crumbliness", 0.86f);

        }

        private void WhileCrumbling(float dt) {
            // Game.Audio.Sounds.PlaySound(m_WhileCrumblingSound, Mathf.Sqrt(m_CrumbleTimer.InverseRatio) * 1f);
            float val = m_CrumbleScale * m_CrumbleCurve.Evaluate(m_CrumbleTimer.InverseRatio);
            m_Renderer.material.SetFloat("_DissolveAmount", val);
   
            SpriteShapeRenderer renderer = m_Renderer.GetComponent<SpriteShapeRenderer>();
            if (renderer != null) {
                for (int i = 0; i < renderer.materials.Length; i++) {
                    if (renderer.materials[i] != null) {
                        renderer.materials[i].SetFloat("_DissolveAmount", val);
                    }
                }
            }

        }

        private void WhileReforming(float dt) {
            float val = m_CrumbleScale * (1f - m_ReformCurve.Evaluate(m_CrumbleTimer.InverseRatio));
            m_Renderer.material.SetFloat("_DissolveAmount", val);
   
            SpriteShapeRenderer renderer = m_Renderer.GetComponent<SpriteShapeRenderer>();
            if (renderer != null) {
                for (int i = 0; i < renderer.materials.Length; i++) {
                    if (renderer.materials[i] != null) {
                        renderer.materials[i].SetFloat("_DissolveAmount", val);
                    }
                }
            }
        }

        #endregion

    }

}
