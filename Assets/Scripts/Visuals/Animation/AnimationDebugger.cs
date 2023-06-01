/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
using UnityEngine.Rendering.Universal;

namespace Platformer.Visuals.Animation {

    [System.Serializable]
    public class AnimationDebugEvent {
        public string name;
        public float duration;
        public bool loop;
        public bool push;
        public bool remove;
    }

    ///<summary>
    ///
    ///<summary>
    public class AnimationDebugger : MonoBehaviour {

        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private AnimationDebugEvent[] m_DebugEvents;

        void Update() {
            for (int i = 0; i < m_DebugEvents.Length; i++) {
                if (m_DebugEvents[i].push) {
                    m_Animator.PushAnimation(m_DebugEvents[i].name, m_DebugEvents[i].loop, m_DebugEvents[i].duration);
                    m_DebugEvents[i].push = false;
                }
                else if (m_DebugEvents[i].remove) {
                    m_Animator.RemoveAnimation(m_DebugEvents[i].name);
                    m_DebugEvents[i].remove = false;
                }
            }
        }

    }

}
