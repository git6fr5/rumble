/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Platformer.LevelEditing {

    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteDecoration : Decoration {

        private SpriteRenderer m_SpriteRenderer;

        // Options.
        public List<Sprite> m_Options = new List<Sprite>();

        // Controls.
        public bool m_Next = false;
        public bool m_Prev = false;
        public int m_OptionIndex = 0;

        [Header("Flip")]
        public bool m_FlipHorizontally = false;
        public bool m_FlipVertically = false;
        [Header("Rotate")]
        // public bool m_Rotate90 = false;
        // public bool m_RotateBack90 = false;
        public bool m_Rotate45 = false;
        public bool m_RotateBack45 = false;
        public bool m_Rotate5 = false;
        public bool m_RotateBack5 = false;

        [HideInInspector]
        public bool m_PauseForReload;

        void OnEnable() {
            this.m_Next = false;
            this.m_Prev = false;
            this.m_Rotate45 = false;
            this.m_RotateBack45 = false;
            this.m_Rotate5 = false;
            this.m_RotateBack5 = false;
            this.m_PauseForReload = true;
        }

        void Update() {
            if (m_PauseForReload) {
                m_PauseForReload = false;
                return;
            }

            if (!Application.isPlaying) {

                if (m_SpriteRenderer == null) {
                  m_SpriteRenderer = GetComponent<SpriteRenderer>();
                }

                if (m_FlipHorizontally) {
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                    m_FlipHorizontally = false;
                }

                if (m_FlipVertically) {
                    Vector3 localScale = transform.localScale;
                    localScale.y *= -1f;
                    transform.localScale = localScale;
                    m_FlipVertically = false;
                }

                // if (m_Rotate90) { Rotate(-90f, ref m_Rotate90); }
                // if (m_RotateBack90) { Rotate(90f, ref m_RotateBack90); }
                if (m_Rotate45) { Rotate(-45f, ref m_Rotate45); }
                if (m_RotateBack45) { Rotate(45f, ref m_RotateBack45); }
                if (m_Rotate5) { Rotate(-5f, ref m_Rotate5); }
                if (m_RotateBack5) { Rotate(5f, ref m_RotateBack5); }

                if (m_Options.Count == 0) {
                    return;
                }

                if (m_Next) {
                    m_OptionIndex++;
                    m_Next = false;
                }
                else if (m_Prev) {
                    m_OptionIndex--;
                    m_Prev = false;
                }

                m_OptionIndex = m_OptionIndex < 0 ? m_Options.Count + m_OptionIndex : m_OptionIndex;
                m_OptionIndex = m_OptionIndex % m_Options.Count;
                
                if (m_SpriteRenderer.sprite != m_Options[m_OptionIndex]) {
                    m_SpriteRenderer.sprite = m_Options[m_OptionIndex];
                }
                
            }

        }

        public void Rotate(float angle, ref bool p) {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z += angle;
            transform.eulerAngles = eulerAngles;
            p = false;
        }

    }

}