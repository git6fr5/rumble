// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Gobblefish.Input {

    ///<summary>
    /// Collects the inputs from a keyboard.
    ///<summary>
    public class NPCInputSystem : InputSystem {

        [System.Serializable]
        public class NPCInputBlock {
            public float horizontalDirection;
            public float verticalDirection;
            public bool[] actionInput;
            public float duration;

            public Vector2 direction => new Vector2(horizontalDirection, verticalDirection);
        }

        public NPCInputBlock[] m_InputChain;

        public float m_Ticks = 0f;

        public int m_Index = 0;

        public bool m_Loop = false;

        protected override void Awake() {
            CreateInputs();
        }

        protected override void CreateInputs() {
            int maxInput = 0;
            foreach (NPCInputBlock block in m_InputChain) {
                if (block.actionInput.Length > maxInput) { maxInput = block.actionInput.Length; }
            }

            m_Actions = new ActionInput[maxInput];
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i] = new ActionInput();
            }
        }

        protected override void Think(float dt) {
        }

        void FixedUpdate() {
            FixedThink(Time.fixedDeltaTime);
        }

        // Updates the inputs.
        protected void FixedThink(float dt) {

            if (m_Index >= m_InputChain.Length) {
                return;
            }

            m_Ticks += dt;
            if (m_Ticks >= m_InputChain[m_Index].duration) {
                m_Ticks -= m_InputChain[m_Index].duration;
                m_Index += 1;
            }

            if (m_Index >= m_InputChain.Length) {
                if (!m_Loop) {
                    return;
                }
                m_Index = 0;
            }
            
            // Updates the directional input.
            m_Direction.OnUpdate(m_InputChain[m_Index].direction);
            for (int i = 0; i < m_InputChain[m_Index].actionInput.Length; i++) {
                m_Actions[i].OnUpdate(m_InputChain[m_Index].actionInput[i], !m_InputChain[m_Index].actionInput[i], dt);
            }

        }

    }

}