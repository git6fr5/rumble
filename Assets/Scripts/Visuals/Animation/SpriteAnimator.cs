/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
using UnityEngine.Rendering.Universal;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Visuals.Animation {

    [System.Serializable]
    public class SpriteAnimation {
        
        [Header("Animation Parameters")]
        public Sprite[] sprites;
        public float ticks;
        private float t => (ticks * fps) % (float)sprites.Length;

        [Header("Frame")]
        public float fps;
        public int currentFrame;

        [Header("Color Gradient")]
        public Gradient colorGradient;

        [Header("Rendering Order")]
        public int orderOffset;

        public void Tick(float dt) {
            ticks += Time.fixedDeltaTime;
        }

        public Sprite GetFrame() {
            return sprites[(int)Mathf.Floor(t)];
        }

        public Color GetColor() {
            return colorGradient.Evaluate(t); 
        }

    }

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour {

        // The idle light animation.
        [SerializeField]
        private SpriteAnimation m_Animation;

        [SerializeField]
        private bool m_Animate;

        // The light attacted to this.
        private SpriteRenderer m_SpriteRenderer;

        private int initialOffset;

        // Runs once on instantiation.
        void Start() {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            initialOffset = m_SpriteRenderer.sortingOrder;
            m_SpriteRenderer.sortingOrder += m_Animation.orderOffset;
        }

        void FixedUpdate() {
            if (!m_Animate) { return; }
            Animate(Time.fixedDeltaTime);
        }

        public void Animate(float dt) {
            m_Animation.Tick(dt);
            m_SpriteRenderer.sprite = m_Animation.GetFrame();
            m_SpriteRenderer.color = m_Animation.GetColor();
        }

        public static void Animate(SpriteRenderer spriteRenderer, SpriteAnimation animation, float dt) {
            animation.Tick(dt);
            spriteRenderer.sprite = animation.GetFrame();
            spriteRenderer.color = animation.GetColor();
        }

        public void SetAnimation(SpriteAnimation animation) {
            m_Animation = animation;
            m_Animation.ticks = Random.Range(0f, 3f);
            if (m_SpriteRenderer != null && m_Animation != null && m_Animation.sprites.Length > 0) {
                m_SpriteRenderer.sortingOrder = initialOffset + m_Animation.orderOffset;
                Animate(0f);
            }        
        }

        public void SetFrameRate(float fps) {
            m_Animation.fps = fps;
        }

        public void Play() {
            m_Animate = true;
        }

        public void Stop() {
            m_Animate = false;
            if (m_SpriteRenderer != null && m_Animation != null && m_Animation.sprites.Length > 0) {
                m_SpriteRenderer.sprite = m_Animation.sprites[0];
            }
        }

    }

}
