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
    public struct SpriteAnimation {
        
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

        // Runs once on instantiation.
        void Awake() {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sortingLayerName = Game.Visuals.RenderingLayers.DecorLayer;
            m_SpriteRenderer.sortingOrder = Game.Visuals.RenderingLayers.DecorOrder + m_Animation.orderOffset;
        }

        void FixedUpdate() {
            if (!m_Animate) { return; }

            m_Animation.Tick(Time.fixedDeltaTime);
            m_SpriteRenderer.sprite = m_Animation.GetFrame();
            m_SpriteRenderer.color = m_Animation.GetColor();
        }

    }

}
