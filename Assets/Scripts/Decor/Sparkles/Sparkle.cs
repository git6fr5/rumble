// Libraries.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    public class Sparkle : MonoBehaviour {

        [SerializeField] private int m_MinSortingOrder;
        [SerializeField] private int m_MaxSortingOrder;

        [SerializeField] private Sprite m_Sprite;
        [SerializeField, ReadOnly] protected List<SpriteRenderer> m_Sparkles = new List<SpriteRenderer>();
        [SerializeField] protected float m_Ticks;
        [SerializeField] protected float m_Interval = 0.075f;
        [SerializeField] protected float m_Radius = 0.25f;

        [SerializeField] protected float m_FadeSpeed = 0.75f;   
        [SerializeField] protected float m_RotationSpeed = 5f; 

        void Start() {
            m_Ticks = m_Interval;
        }       

        void FixedUpdate() {
            if (!IsActive()) {
                Reset();
                return;
            }

            bool finished = Timer.TickDown(ref m_Ticks, Time.fixedDeltaTime);
            if (finished) {
                Spawn();
                ResetTimer();
            }
            AdjustColor(Time.fixedDeltaTime);
            AdjustRotation(Time.fixedDeltaTime);
            AdjustPosition(Time.fixedDeltaTime);
            AdjustScale(Time.fixedDeltaTime);

            ExtraStuff(Time.fixedDeltaTime);
        }

        protected virtual void ResetTimer() {
            m_Ticks = m_Interval;
        }

        protected virtual bool IsActive() {
            return true;
        }

        protected virtual void ExtraStuff(float dt) {
        }

        private void AdjustColor(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            for (int i = 0; i < m_Sparkles.Count; i++) {
                Color c = m_Sparkles[i].color;
                c.a = c.a - m_FadeSpeed * deltaTime;
                if (c.a < 0.1f) {
                    Destroy(m_Sparkles[i].gameObject);
                }
                else {
                    m_Sparkles[i].color = c;
                }
            }
        }

        protected virtual void AdjustRotation(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
            Vector3 deltaAngles = Vector3.forward * m_RotationSpeed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                m_Sparkles[i].transform.eulerAngles += deltaAngles;
            }
        }

        protected virtual void AdjustPosition(float deltaTime) {
            
        }

        protected virtual void AdjustScale(float deltaTime) {
            
        }

        private void Spawn() {
            SpriteRenderer spriteRenderer = new GameObject("Goo", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_Sprite;
            spriteRenderer.transform.position = transform.position + (Vector3)Random.insideUnitCircle * m_Radius;
            spriteRenderer.sortingOrder = Random.Range(m_MinSortingOrder, m_MaxSortingOrder);
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.transform.localScale *= Random.Range(0.5f, 2f);
            m_Sparkles.Add(spriteRenderer);
        }

        public void Play(bool play) {
            gameObject.SetActive(play);
        }

        public void Reset() {
            m_Sparkles.RemoveAll(thing => thing == null);
            for (int i = 0; i < m_Sparkles.Count; i++) {
                Destroy(m_Sparkles[i].gameObject);
            }
            m_Sparkles.RemoveAll(thing => thing == null);
        }

        private void OnDisable() {
            Reset();
        }

        private void OnDestroy() {
            Reset();
        }

    }
}