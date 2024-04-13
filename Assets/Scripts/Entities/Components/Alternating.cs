/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityEngine.U2D;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

namespace Platformer.Entities {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class Alternating : MonoBehaviour {

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

        [HideInInspector]
        private Entity m_Entity;

        // The period with which these alternate.
        private const float PERIOD = 4f;

        // The duration before changing that we indicate a change is happening.
        private const float PRE_CHANGE_DURATION = 0.3f;

        // The offset the alternating platform experiences while changing.
        private const float PRE_CHANGE_OFFSET = 1f/16f;

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

        [SerializeField]
        private AnimationCurve m_AlternatingCurve;

        [SerializeField]
        private UnityEvent m_PreChangeEvent = new UnityEvent();

        [SerializeField]
        private UnityEvent m_PreChangeBackEvent = new UnityEvent();

        [SerializeField]
        private UnityEvent m_ChangeOnEvent = new UnityEvent();
        
        [SerializeField]
        private UnityEvent m_ChangeOffEvent = new UnityEvent();

        #endregion

        void Awake() {
            m_Entity = GetComponent<Entity>();
        }
        
        void Start() {
            // m_Entity.SetMaterial(Game.Visuals.RenderingLayers.AltMat);
            
            float t = PhysicsManager.Time.Ticks % PERIOD;
            bool enableA = t < PERIOD / 2f;
            switch (m_AlternatingType) {
                case AlternatingType.A:
                    OnChange(enableA);
                    WhileChanging(enableA);
                    break;
                case AlternatingType.B:
                    OnChange(!enableA);
                    WhileChanging(!enableA);
                    break;
                default:
                    break;
            }
        }


        void FixedUpdate() {
            
            float t = PhysicsManager.Time.Ticks % PERIOD;
            bool enableA = t < PERIOD / 2f;
            bool change = (PhysicsManager.Time.Ticks % (PERIOD / 2f)) > PERIOD / 2f - PRE_CHANGE_DURATION;

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

            if (m_AlternatingState == AlternatingState.Changing) {
                
                switch (m_AlternatingType) {
                    case AlternatingType.A:
                        WhileChanging(enableA);
                        break;
                    case AlternatingType.B:
                        WhileChanging(!enableA);
                        break;
                    default:
                        break;
                }

            }

        }

        private void WhileChanging(bool enable) {

            // float t = (PhysicsManager.Time.Ticks % (PERIOD / 2f)) - (PERIOD / 2f - PRE_CHANGE_DURATION);
            // float x = t / PRE_CHANGE_DURATION;
            
            // // If we're enabled, then the change is towards disabled
            // if (enable) {
            //     x *= -1f;
            // }

            // float val = m_AlternatingCurve.Evaluate(x);
            // m_Entity.SetMaterialValue("_DissolveAmount", val);

        }

        private void OnPreChange() {
            // m_Entity.Renderer.transform.localPosition = -Vector3.down * PRE_CHANGE_OFFSET;
            if (m_Entity.CollisionEnabled) {
                // m_PreChangeSound.Play(0.05f);
                m_PreChangeEvent.Invoke();
            }
            else {
                m_PreChangeBackEvent.Invoke();
            }
        }

        private void OnChange(bool enable) {
            if (enable) {
                // m_ChangeSound.Play(0.03f);
                m_ChangeOnEvent.Invoke();
            }
            else {
                m_ChangeOffEvent.Invoke();
            }

            m_Entity.EnableColliders(enable);
            // m_Entity.SetMaterialValue("_DissolveAmount", val);
            
            // m_Entity.Renderer.transform.localPosition = Vector3.zero;
            // m_SpriteShapeRenderer.enabled = enable;
            // m_Hitbox.enabled = enable;
            // m_DisabledObject.SetActive(!enable);
            

        }

        public void SetType(AlternatingType altType) {
            m_AlternatingType = altType;
        }
        
    }

}
