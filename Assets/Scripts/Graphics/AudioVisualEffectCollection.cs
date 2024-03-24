// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish;
using Gobblefish.Graphics;
using Gobblefish.Audio;

namespace Platformer {

    [System.Serializable]
    public class AudioVisualEffect {
        public VisualEffect visualEffect;
        public AudioSnippet audioSnippet;
    }

    [CreateAssetMenu(fileName="AudioVisualEffectCollection", menuName="AudioVisual Effect Collection")]
    public class AudioVisualEffectCollection : Gobblefish.CollectionAsset<AudioVisualEffect> {

        // [SerializeField]
        // private bool m_AlwaysReinstantiate; 

        // private Dictionary<(Transform, string), VisualEffect> m_VFXDict = new Dictionary<(Transform, string), VisualEffect>();

        // public void Play() {

        // }

        // public void Play(string name) {

        //     // if (!m_AlwaysReinstantiate) {
        //     //     if (m_VFXDict.ContainsKey((transform, name)) && m_VFXDict[(transform, name)] != null) {
        //     //         m_VFXDict[(transform, name)].Play();
        //     //         return;
        //     //     }
        //     // }
            
        //     AudioVisualEffect av = Get(name);
        //     if (av.visualEffect != null) {
        //         VisualEffect newVfx = Duplicate(av.visualEffect, transform);
        //     }
        //     if (av.audioSnippet != null) {
        //         av.audioSnippet.Play();
        //     }

        // }

        public VisualEffect Duplicate(VisualEffect visualEffect, Transform transform) {
            VisualEffect duplicatedEffect = Instantiate(visualEffect, transform).GetComponent<VisualEffect>();
            duplicatedEffect.transform.localPosition = Vector3.zero;
            duplicatedEffect.gameObject.SetActive(true);
            return duplicatedEffect;
        }

    }


}
