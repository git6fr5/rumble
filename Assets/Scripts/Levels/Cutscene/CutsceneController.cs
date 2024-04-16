// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//
using Gobblefish.Input;
//
using Platformer.Physics;
using AnimationPriority = Platformer.Character.CharacterAnimator.AnimationPriority;

namespace Platformer.Levels {

    [System.Serializable]
    public class CutsceneCharacter {
        public NPCInputSystem characterInputSystem;
        public NPCInputChain characterInputChain;
        public bool dissappearOnEnd;
    }

    [System.Serializable]
    public class PlayerAnimationOverride {
        public string animationName = "Idle";
        public float duration = 1f;
        private float ticks = 0f;
    }

    public class CutsceneController : MonoBehaviour {

        public AnimationPriority priority;

        [SerializeField]
        private Volume m_Volume;

        [SerializeField]
        private float m_FadeInDuration = 1f;
        private bool m_FadeIn = false;

        [SerializeField]
        private float m_FadeOutDuration = 1f;
        private bool m_FadeOut = false;

        [SerializeField]
        private PlayerAnimationOverride[] m_PlayerAnimationOverrides;
        
        [SerializeField]
        public CutsceneCharacter[] m_NPCs;

        [SerializeField]
        private bool m_Playing = false;
        
        [SerializeField] 
        private float m_Duration = 3f;
        private float m_Ticks = 0f;

        void Play() {
            FreezePlayer();

            foreach (CutsceneCharacter npc in m_NPCs) {
                npc.characterInputSystem.SetChain(npc.characterInputChain);
                npc.characterInputSystem.gameObject.SetActive(true);
            }

            m_Playing = true;
            m_Ticks = 0f;
            m_FadeOut = false;
            m_FadeIn = true;

        }

        public void OnPlayerEntered(Platformer.Character.CharacterController character) {
            if (character == Platformer.PlayerManager.Character) {
                Play();
            }
        }

        public void FreezePlayer() {
            Platformer.PlayerManager.Character.Body.Stop();
            Platformer.PlayerManager.Character.Body.SetWeight(3f);
            Platformer.PlayerManager.Character.Disable(m_Duration);
            Platformer.PlayerManager.Character.Default.Enable(Platformer.PlayerManager.Character, false);
            Platformer.PlayerManager.Character.Animator.PlayAnimation(m_PlayerAnimationOverride);
        }

        public void UnfreezePlayer() {
            Platformer.PlayerManager.Character.Default.Enable(Platformer.PlayerManager.Character, true);
            Platformer.PlayerManager.Character.Animator.StopAnimation(m_PlayerAnimationOverride);
        }

        public void FixedUpdate() {
            if (m_Playing) {
                WhilePlaying(Time.fixedDeltaTime);
            }
        }

        private void Stop() {
            UnfreezePlayer();

            foreach (CutsceneCharacter npc in m_NPCs) {
                npc.characterInputSystem.gameObject.SetActive(!npc.dissappearOnEnd);
            }
            
            m_Playing = false;
            m_Ticks = 0f;
            m_FadeOut = false;
            m_FadeIn = false;

        }

        private void WhilePlaying(float dt) {
            m_Ticks += dt;
            if (m_Ticks >= m_Duration) {
                Stop();
                return;
            }

            m_PlayerAnimationOverrides[m_AnimationOverrideIndex].ticks += dt;
            if (m_PlayerAnimationOverrides[m_AnimationOverrideIndex].ticks > m_PlayerAnimationOverrides[m_AnimationOverrideIndex].duration) {
                Platformer.PlayerManager.Character.StopAnimation(m_PlayerAnimationOverrides[m_Index].priority);
                
                m_AnimationOverrideIndex += 1;
                if (m_AnimationOverrideIndex < m_PlayerAnimationOverrides.Length) {
                    Platformer.PlayerManager.Character.PlayAnimation(m_PlayerAnimationOverrides[m_Index].animationName, m_PlayerAnimationOverrides[m_Index].priority);
                }

            }

            if (m_Volume != null && m_Ticks < m_FadeInDuration) {
                m_Volume.weight += m_FadeInDuration * dt;
            }
            else if (m_Volume != null && m_Ticks > m_Duration - m_FadeOutDuration) {
                m_Volume.weight -= m_FadeOutDuration * dt;
            }

        }

    }

}
