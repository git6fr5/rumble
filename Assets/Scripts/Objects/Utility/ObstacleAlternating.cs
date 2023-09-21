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
    public class ObstacleAlternating : MonoBehaviour {

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

        [SerializeField]
        private Obstacle m_Obstacle;

        [SerializeField]
        private Renderer m_Renderer;

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

        // The sound that plays to indicate the prechange.
        [SerializeField] 
        private AudioClip m_PreChangeSound;
        
        // The sound that plays when changing.
        [SerializeField] 
        private AudioClip m_ChangeSound;

        #endregion

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
            // transform.position = m_Obstacle.Origin - Vector3.down * PRE_CHANGE_OFFSET;
            if (m_Obstacle.AllCollidersEnabled(true)) {
                Game.Audio.Sounds.PlaySound(m_PreChangeSound, 0.05f);
            }
        }

        private void OnChange(bool enable) {
            float val = 5f;
            if (enable && m_Obstacle.AllCollidersEnabled(false)) {
                Game.Audio.Sounds.PlaySound(m_ChangeSound, 0.03f);
                m_Obstacle.EnableColliders(true);
            }
            else if (!enable && m_Obstacle.AllCollidersEnabled(true)) {
                val = -5f;
                m_Obstacle.EnableColliders(false);
            }

            m_Renderer.material.SetFloat("_DissolveAmount", val);

            SpriteShapeRenderer renderer = m_Renderer.GetComponent<SpriteShapeRenderer>();
            if (renderer != null) {
                for (int i = 0; i < renderer.materials.Length; i++) {
                    if (renderer.materials[i] != null) {
                        renderer.materials[i].SetFloat("_DissolveAmount", val);
                    }
                }
            }
            
            // transform.position = m_Origin;
            // m_SpriteShapeRenderer.enabled = enable;
            // m_Hitbox.enabled = enable;
            // m_DisabledObject.SetActive(!enable);
            

        }
    }

}
