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
using Entity = Platformer.Entities.Entity;

namespace Platformer.Entities.Effects {

    ///<summary>
    ///
    ///<summary>
    public class Alternator : MonoBehaviour {

        #region Enumerations.

        public enum AlternatingType {
            A, B
        }

        public enum AlternatingState {
            Stable, Changing
        }

        #endregion

        #region Variables.

        /* --- Constants --- */

        // The period with which these alternate.
        private const float PERIOD = 4f;

        // The duration before changing that we indicate a change is happening.
        private const float PRE_CHANGE_DURATION = 0.5f;

        // The offset the alternating platform experiences while changing.
        private const float PRE_CHANGE_OFFSET = 2f/16f;

        /* --- Members --- */

        // The type of alternating platform.
        [SerializeField] 
        private AlternatingType m_AlternatingType;

        // The type of alternating platform.
        [SerializeField] 
        private AlternatingState m_AlternatingState;

        // The object that is presented when the platform is disabled.
        [SerializeField] 
        private GameObject m_DisabledObject;

        // The sound that plays to indicate the prechange.
        [SerializeField] 
        private AudioClip m_PreChangeSound;
        
        // The sound that plays when changing.
        [SerializeField] 
        private AudioClip m_ChangeSound;

        [SerializeField]
        private TransformAnimation m_ChangingAnimation;

        Entity m_Entity;

        #endregion

        void Awake() {
            m_Entity = GetComponent<Entity>();
        }

        public float t = 0;

        void FixedUpdate() {

            t += Time.fixedDeltaTime;
            
            // Get the modulated time.
            float modulatedTime = t % PERIOD;
            float halfModulatedTime = t % (PERIOD / 2f);
            // Get the half period that we're currently in. 
            bool enableA = modulatedTime < PERIOD / 2f;
            // Check whether the time is currently within the pre-change range.
            bool changing = halfModulatedTime > PERIOD / 2f - PRE_CHANGE_DURATION;
            float changingPercent = (PERIOD / 2f - halfModulatedTime) / PRE_CHANGE_DURATION;

            // Toggle the pre-change.
            if (m_AlternatingState != AlternatingState.Changing && changing) {
                m_AlternatingState = AlternatingState.Changing;
                OnPreChange();
            }
            // Toggle the change.
            else if (m_AlternatingState == AlternatingState.Changing && !changing) {
                m_AlternatingState = AlternatingState.Stable;

                // Change the state depending on the type.
                switch (m_AlternatingType) {
                    case AlternatingType.A:
                        OnChange(enableA);
                        break;
                    case AlternatingType.B:
                        OnChange(!enableA);
                        break;
                    default:
                        break;
                }

            }
            else if (m_AlternatingState == AlternatingState.Changing) {
                WhileChanging(changingPercent);
            }

        }

        private void WhileChanging(float ticks) {
            m_Entity.transform.Animate(m_ChangingAnimation, Time.fixedDeltaTime);
        }

        // The effect to occur on pre change.
        private void OnPreChange() {
            m_ChangingAnimation.AnimationTimer.Stop();
            // if (m_Hitbox.enabled) {
            //     Game.Audio.Sounds.PlaySound(m_PreChangeSound, 0.05f);
            // }
        }

        // The effect to occur when changing.
        private void OnChange(bool enable) {
            // Play a sound when enabling this entity.
            // if (enable && !m_Hitbox.enabled) {
            //     Game.Audio.Sounds.PlaySound(m_ChangeSound, 0.03f);
            // }
            
            m_Entity.Enable(enable);
            m_Entity.Reset();

            m_DisabledObject.SetActive(!enable);
        }
    }

}
