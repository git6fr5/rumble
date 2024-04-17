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
        public NPCInputChain onEndChain;
    }

    public class CutsceneController : MonoBehaviour {

        [SerializeField]
        private Volume m_Volume;

        [SerializeField]
        private float m_FadeInDuration = 1f;
        private bool m_FadeIn = false;

        [SerializeField]
        private float m_FadeOutDuration = 1f;
        private bool m_FadeOut = false;

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

            if (m_Volume != null) { 
                m_Volume.weight = 0f; 
            }

        }

        public void OnPlayerEntered(Platformer.Character.CharacterController character) {
            if (character == Platformer.PlayerManager.Character) {
                Play();
            }
        }

        public void FreezePlayer() {
            // Platformer.PlayerManager.Character.Body.SetWeight(0f);
            Platformer.PlayerManager.Character.Body.velocity *= 0f;
            Platformer.PlayerManager.Character.Disable(m_Duration);
            Platformer.PlayerManager.Character.Default.Enable(Platformer.PlayerManager.Character, false, true);
        }

        public void UnfreezePlayer() {
            Platformer.PlayerManager.Character.Default.Enable(Platformer.PlayerManager.Character, true);
        }

        public void FixedUpdate() {
            if (m_Playing) {
                WhilePlaying(Time.fixedDeltaTime);
            }
        }

        private void Stop() {
            UnfreezePlayer();

            foreach (CutsceneCharacter npc in m_NPCs) {
                npc.characterInputChain.OnEnd(npc.characterInputSystem);
                npc.characterInputSystem.FullClear();
                npc.characterInputSystem.SetChain(npc.onEndChain);
            }
            
            m_Playing = false;
            m_Ticks = 0f;
            m_FadeOut = false;
            m_FadeIn = false;

            if (m_Volume != null) { 
                m_Volume.weight = 0f; 
            }

            Destroy(gameObject);

        }

        private void WhilePlaying(float dt) {
            m_Ticks += dt;
            if (m_Ticks >= m_Duration) {
                Stop();
                return;
            }

            if (m_Volume != null && m_Ticks < m_FadeInDuration) {
                if (m_FadeInDuration != 0f) { 
                    m_Volume.weight += dt / m_FadeInDuration; 
                }
                else { 
                    m_Volume.weight = 1f; 
                }
                if (m_Volume.weight > 1f) { m_Volume.weight = 1f; }
            }
            else if (m_Volume != null && m_Ticks > m_Duration - m_FadeOutDuration) {
                if (m_FadeOutDuration != 0f) { 
                    m_Volume.weight -= dt / m_FadeOutDuration; 
                }
                else { 
                    m_Volume.weight = 0f; 
                }
                if (m_Volume.weight < 0f) { m_Volume.weight = 0f; }
            }

        }

    }

}
