// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using Platformer.Physics;

using Gobblefish.Input;

namespace Platformer.Levels {

    public class Cutscene : MonoBehaviour {

        [System.Serializable]
        public class NPCCutscene {
            public NPCInputSystem input;
            public NPCInputChain chain;
            public bool dissappearOnEnd;
        }

        public Volume m_Volume;

        public float m_FadeOutDuration = 1f;

        public bool end = false;

        public NPCCutscene[] npcs;

        public bool playing = false;

        public float ticks = 0f;

        public float duration = 3f;

        void Play() {
            print("Player");
            
            foreach (NPCCutscene npc in npcs) {
                npc.input.SetChain(npc.chain);
                npc.input.gameObject.SetActive(true);
            }

            playing = true;
            ticks = 0f;
            end = false;

        }

        public void PlayerEntered(Platformer.Character.CharacterController character) {
            if (character == Platformer.PlayerManager.Character) {
                Play();
                character.Body.Stop();
                character.Body.SetWeight(0f);
                character.Disable(duration);
                character.Default.Enable(character, false);
                character1 = character;
            }
        }

        public Platformer.Character.CharacterController character1 = null;

        public void Update() {
            float dt = Time.deltaTime;
            if (ticks > duration || !playing) {
                return;
            }

            ticks += dt;

            if (ticks > duration - m_FadeOutDuration) {
                end = true;
            }

            if (ticks > duration) {

                // character.Disable(duration);
                if (character1 != null) { 
                    character1.Default.Enable(character1, true);
                }

                foreach (NPCCutscene npc in npcs) {
                    npc.input.gameObject.SetActive(!npc.dissappearOnEnd);
                }

                playing = false;
            }

            // if (play) {
            //     Play();
            //     play = false;
            // }

            if (end && m_Volume != null) {
                m_Volume.weight -= m_FadeOutDuration * dt;
            }

        }

    }

}
