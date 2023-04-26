/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Platformer.Visuals.Effects;

namespace Platformer.Visuals {

    ///<summary>
    /// Controls the particle effects in the game.
    ///<summary>
    public class EffectController : MonoBehaviour {

        #region Variables.

        /* --- Member --- */

        // Base circle effect that is used over this game.
        [SerializeField]
        private CircleEffect m_BaseCircleEffect = null;

        // Base impact effect that is used over this game.
        [SerializeField]
        private ImpactEffect m_BaseImpactEffect = null;

        // Base impact effect that is used over this game.
        [SerializeField]
        private BubbleEffect m_BaseBubbleEffect = null;

            // Base impact effect that is used over this game.
        [SerializeField]
        private SparkEffect m_BaseSparkEffect = null;

        // The effect dictionary that tracks all the effects being played in the game.
        public Dictionary<int, Effect> m_EffectDictionary = new Dictionary<int, Effect>();

        #endregion

        public bool circle;
        public bool impact;
        public bool bubble;
        public bool spark;

        void Update() {
            if (circle) {
                PlayCircleEffect(0.2f, transform, Vector3.zero);
                circle = false;
            }
            if (impact) {
                PlayImpactEffect(null, 8, 0.15f, transform, Vector3.zero);
                impact = false;
            }
            if (bubble) {
                PlayBubbleEffect(null, -1, 0.75f, 5f, transform, Vector3.zero);
                bubble = false;
            }
            if (spark) {
                PlaySparkEffect(null, 8, 0.75f, 5f, false, transform, Vector3.zero);
                spark = false;
            }
        }

        public void PlayEffect(VisualEffect visualEffect) {
            //
        }

        public void PauseEffect(VisualEffect visualEffect) {
            //
        }

        // Plays a circle effect attached to something for a certain duration.
        public int PlayCircleEffect(float duration, Transform parent, Vector3 localPosition) {
            CircleEffect circleEffect = Instantiate(m_BaseCircleEffect.gameObject).GetComponent<CircleEffect>();
            circleEffect.transform.parent = parent;
            circleEffect.transform.localPosition = localPosition;
            circleEffect.Play(duration);
            return AddEffectToDictionary(circleEffect);
        }

        // Plays an impact effect attached to something for a certain.
        public int PlayImpactEffect(Sprite sprite, int count, float speed, Transform parent, Vector3 localPosition) {
            ImpactEffect impactEffect = Instantiate(m_BaseImpactEffect.gameObject).GetComponent<ImpactEffect>();
            impactEffect.transform.parent = parent;
            impactEffect.transform.localPosition = localPosition;
            impactEffect.Play(sprite, count, speed);
            return AddEffectToDictionary(impactEffect);
        }

        // Plays an impact effect attached to something for a certain.
        public int PlayBubbleEffect(Sprite sprite, int count, float speed, float rate, Transform parent, Vector3 localPosition) {
            BubbleEffect bubbleEffect = Instantiate(m_BaseBubbleEffect.gameObject).GetComponent<BubbleEffect>();
            bubbleEffect.transform.parent = parent;
            bubbleEffect.transform.localPosition = localPosition;
            bubbleEffect.Play(sprite, count, speed, rate);
            return AddEffectToDictionary(bubbleEffect);
        }

        // Plays an impact effect attached to something for a certain.
        public int PlaySparkEffect(Sprite sprite, int count, float speed, float rate, bool coagulate, Transform parent, Vector3 localPosition) {
            SparkEffect sparkEffect = Instantiate(m_BaseSparkEffect.gameObject).GetComponent<SparkEffect>();
            sparkEffect.transform.parent = parent;
            sparkEffect.transform.localPosition = localPosition;
            sparkEffect.Play(sprite, count, speed, rate, coagulate);
            return AddEffectToDictionary(sparkEffect);
        }

        // Stops an effect that is in the effect dictionary.
        public void StopEffect(int effectID) {
            foreach(KeyValuePair<int, Effect> item in m_EffectDictionary.Where(kvp => kvp.Value == null).ToList()) {
                m_EffectDictionary.Remove(item.Key);
            } 
            if (m_EffectDictionary.ContainsKey(effectID)) {
                m_EffectDictionary[effectID].Stop();
                m_EffectDictionary.Remove(effectID);
            }
        }

        // Pauses an effect that is in the effect dictionary.
        public void PauseEffect(int effectID) {
            foreach(KeyValuePair<int, Effect> item in m_EffectDictionary.Where(kvp => kvp.Value == null).ToList()) {
                m_EffectDictionary.Remove(item.Key);
            } 
            if (m_EffectDictionary.ContainsKey(effectID)) {
                m_EffectDictionary[effectID].Pause();
            }
        }

        // Adds the effect to a dictionary.
        private int AddEffectToDictionary(Effect effect) {
            foreach(KeyValuePair<int, Effect> item in m_EffectDictionary.Where(kvp => kvp.Value == null).ToList()) {
                m_EffectDictionary.Remove(item.Key);
            }            
            int randomID = Random.Range(0, 1000000);
            while (m_EffectDictionary.ContainsKey(randomID)) {
                randomID = Random.Range(0, 1000000);
            }
            m_EffectDictionary.Add(randomID, effect);
            return randomID;
        }

    }

}