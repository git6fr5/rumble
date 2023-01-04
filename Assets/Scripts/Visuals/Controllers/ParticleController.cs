/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using Platformer.Visuals;
using Platformer.Visuals.Effects;

namespace Platformer.Visuals {

    ///<summary>
    /// Controls the particle effects in the game.
    ///<summary>
    public class ParticleController : MonoBehaviour {

        public CircleEffect m_BaseCircleEffect = null;

        public Dictionary<int, Effect> m_EffectDictionary = new Dictionary<int, Effect>();

        public void PlayEffect(VisualEffect visualEffect) {
            //
        }

        public void PlayEffect(ParticleSystem particleEffect) {
            // particleEffect.gameObject.SetActive(true);
        }

        public void PauseEffect(VisualEffect visualEffect) {
            //
        }

        public int PlayCircleEffect(float duration, Transform parent, Vector3 localPosition) {
            CircleEffect circleEffect = Instantiate(m_BaseCircleEffect.gameObject).GetComponent<CircleEffect>();
            circleEffect.transform.parent = parent;
            circleEffect.transform.localPosition = localPosition;
            circleEffect.Play(duration);
            return AddToDictionary(circleEffect);
        }

        private int AddToDictionary(Effect effect) {
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

        public void StopEffect(int effectID) {
            foreach(KeyValuePair<int, Effect> item in m_EffectDictionary.Where(kvp => kvp.Value == null).ToList()) {
                m_EffectDictionary.Remove(item.Key);
            } 
            if (m_EffectDictionary.ContainsKey(effectID)) {
                m_EffectDictionary[effectID].Stop();
                m_EffectDictionary.Remove(effectID);
            }
        }

    }

}