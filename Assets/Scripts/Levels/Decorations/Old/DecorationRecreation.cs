/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.LevelEditing {

    [ExecuteInEditMode]
    public class DecorationRecreation : MonoBehaviour {

        public float alpha;

        private SpriteRenderer[] m_SpriteRenderers;

        void Update() {
            if (!Application.isPlaying) {
                GetAllSpriteRenderers();
                for (int i = 0; i < m_SpriteRenderers.Length; i++) {
                    Color color = m_SpriteRenderers[i].color;
                    color.a = alpha;
                    m_SpriteRenderers[i].color = color;
                }
            }
        }

        private void GetAllSpriteRenderers() {
            m_SpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        }

    }

}