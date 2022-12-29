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

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    public class AlternatingPlatform : PlatformObject {

        #region Enumerations.

        public enum AlternatingType {
            A, B
        }

        public enum AlternatingState {
            Stable, Changing
        }

        #endregion

        /* --- Constants --- */

        // The period with which these alternate.
        private const float PERIOD = 4f;

        // The duration before changing that we indicate a change is happening.
        private const float PRE_CHANGE_DURATION = 0.5f;

        // The offset the alternating platform experiences while changing.
        private const float PRE_CHANGE_OFFSET = 2f/16f;

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

        public override void Init(int length, Vector3[] path) {
            base.Init(length, path);
            EditSpline(m_DisabledObject.GetComponent<SpriteShapeController>().spline, length);
        }

        void FixedUpdate() {
            
            float t = Game.Physics.Time.Ticks % PERIOD;
            bool enableA = t < PERIOD / 2f;
            bool change = (Game.Physics.Time.Ticks % (PERIOD / 2f)) > PERIOD / 2f - PRE_CHANGE_DURATION;

            if (m_AlternatingState != AlternatingState.Changing && change) {
                m_AlternatingState = AlternatingState.Changing;
                OnPreChange();
            }
            else if (m_AlternatingState == AlternatingState.Changing && !change) {
                m_AlternatingState = AlternatingState.Stable;

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

        }

        private void OnPreChange() {
            transform.position = m_Origin - Vector3.down * PRE_CHANGE_OFFSET;
            if (m_Hitbox.enabled) {
                Game.Audio.Sounds.PlaySound(m_PreChangeSound, 0.05f);
            }
        }

        private void OnChange(bool enable) {
            if (enable && !m_Hitbox.enabled) {
                Game.Audio.Sounds.PlaySound(m_ChangeSound, 0.03f);
            }
            
            transform.position = m_Origin;
            m_SpriteShapeRenderer.enabled = enable;
            m_Hitbox.enabled = enable;
            m_DisabledObject.SetActive(!enable);
        }
    }

}
