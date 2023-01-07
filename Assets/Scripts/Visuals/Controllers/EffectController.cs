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

        // The effect dictionary that tracks all the effects being played in the game.
        public Dictionary<int, Effect> m_EffectDictionary = new Dictionary<int, Effect>();

        #endregion

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