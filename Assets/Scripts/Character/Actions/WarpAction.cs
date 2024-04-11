// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
using Gobblefish.Animation;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [CreateAssetMenu(fileName="WarpAction", menuName ="Actions/Warp")]
    public class WarpAction : CharacterAction {

        #region Variables.
        
        // The speed of the actual warp.
        [SerializeField] 
        private float m_WarpDistance = 2f;

        // The speed of the actual warp.
        [SerializeField] 
        private float m_ChargeDuration = 0.5f;

        [SerializeField]
        private Timer m_ChargeTimer = new Timer(0f, 0f);

        [SerializeField]
        private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);

        // The direction the player was facing before the warp started.
        [HideInInspector]
        protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_PrewarpAnimation = null;

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_WarpAnimation = null;

        // The sprites this is currently animating through.
        [SerializeField]
        protected SpriteAnimation m_PostwarpAnimation = null;

        // The visual effect that plays at the start of the warp.
        [SerializeField]
        protected VisualEffect m_StartWarpEffect;

        // The visual effect that plays at the start of the warp.
        [SerializeField]
        protected AudioSnippet m_StartWarpSound;

        // The visual effect that plays at the start of the warp.
        [SerializeField]
        protected VisualEffect m_EndWarpEffect;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private VisualEffect m_ChargeEffect = null;

        // The sounds that plays when charging the hop.
        [SerializeField]
        private AudioSnippet m_ChargeSound = null;

        [SerializeField]
        private Sprite m_WarpIndicatorSprite = null;

        private Transform m_WarpIndicator = null;

        public float CYCLE_DURATION = 2f;

        public float m_PostwarpWeight;

        public float m_PostwarpDuration;

        public float m_PostwarpSpeed;

        [SerializeField]
        private Timer m_WarpTimer = new Timer(0f, 0f);

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            m_ActionPhase = ActionPhase.None;

            if (!enable) {
                character.Animator.Remove(m_WarpAnimation);
            }

            if (enable && m_WarpIndicator == null) {
                m_WarpIndicator = new GameObject("warp indicator", typeof(SpriteRenderer)).transform;
                m_WarpIndicator.GetComponent<SpriteRenderer>().sprite = m_WarpIndicatorSprite;
                m_WarpIndicator.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
                m_WarpIndicator.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }

            if (enable) {
                moving = m_WarpIndicator;
                stationary = character.transform;
                
                if (option1) {
                    character.Default.Enable(character, false);
                    character.Body.SetWeight(0f);
                    character.Body.SetVelocity(Vector2.zero);
                }
            }

        }

        public bool option0;
        public bool option1;
        public bool option2;

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // if (character.Input.Actions[1].Pressed && m_Refreshed && m_ActionPhase == ActionPhase.None && !character.OnGround) {
                
            //     m_WarpTimer.Start(1f);
            //     m_ActionPhase = ActionPhase.PreAction;
            //     character.Default.Enable(character, false);
            //     character.Body.SetWeight(CHARGE_WEIGHT);
                
            //     character.Input.Actions[1].ClearPressBuffer();
            //     m_Refreshed = false;
            // }

            if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None) {
                if (option0) { OnWarp(character); }

                if (moving == m_WarpIndicator) { Swap(); }
                else {
                    if (option1) { Swap(); }
                    else if (option2) { Throw(character); }
                }


                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }

        }

        Transform moving;
        Transform stationary;
        float offset = 0f;
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && m_ActionPhase == ActionPhase.None ? true : m_Refreshed;

            //
            bool finished = m_WarpTimer.TickDown(dt) || !m_WarpTimer.Active;

            if (finished) {
                switch (m_ActionPhase) {
                    case ActionPhase.PostAction:
                        
                        OnEndWarp(character);

                        break;
                    default:
                        break;
                }
            }

            // Charge the hop.
            bool cycleFin = m_ChargeTimer.TickDown(dt) || !m_ChargeTimer.Active;
            
            // For constant cycling.
            if (cycleFin) {
                m_ChargeTimer.Start(CYCLE_DURATION);
            }

            if (option0) {
                m_WarpIndicator.position = character.transform.position + Quaternion.Euler(0f, 0f, -m_ChargeTimer.Ratio * 360f) * (m_WarpDistance / 2f * Vector3.right);
            }
            else if (option1) {
                moving.position = stationary.transform.position + Quaternion.Euler(0f, 0f, -m_ChargeTimer.Ratio * 360f + offset) * (m_WarpDistance * Vector3.right);
            }
            else if (option2 && m_ActionPhase == ActionPhase.None) {
                moving.position = stationary.transform.position + Quaternion.Euler(0f, 0f, -m_ChargeTimer.Ratio * 360f + offset) * (m_WarpDistance * Vector3.right);
            }

        }

        public void OnWarp(CharacterController character) {
            character.transform.position += Quaternion.Euler(0f, 0f, -m_ChargeTimer.Ratio * 360f) * (m_WarpDistance * Vector3.right);
            character.Body.velocity = Quaternion.Euler(0f, 0f, -m_ChargeTimer.Ratio * 360f) * (m_PostwarpSpeed * Vector3.right);
            character.Body.SetWeight(m_PostwarpWeight);
            character.Default.Enable(character, false);

            m_WarpTimer.Start(m_PostwarpDuration);
            m_ActionPhase = ActionPhase.PostAction;
        }

        public void OnEndWarp(CharacterController character) {
            character.Default.Enable(character, true);
            m_ActionPhase = ActionPhase.None;
        }

        public void Swap() {

            Transform tmp = moving;
            moving = stationary;
            stationary = tmp;

            offset = Vector2.SignedAngle(Vector2.right, moving.position - stationary.position);

        }

        public void Throw(CharacterController character) {

            character.Body.velocity = Quaternion.Euler(0f, 0f, -m_ChargeTimer.Ratio * 360f - 90f) * (m_PostwarpSpeed * Vector3.right);
            character.Body.SetWeight(0f);
            character.Default.Enable(character, false);

            m_WarpTimer.Start(m_PostwarpDuration);
            m_ActionPhase = ActionPhase.PostAction;
            Swap();

        }

        

    }
}