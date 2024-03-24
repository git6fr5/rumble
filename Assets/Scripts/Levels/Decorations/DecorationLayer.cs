// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.SceneManagement;
// Gobblefish.
using Gobblefish;

namespace Platformer.Levels {

    [ExecuteInEditMode]
    public class DecorationLayer : MonoBehaviour {

        [SerializeField]
        private List<SpriteRenderer> m_SpriteRenderers = new List<SpriteRenderer>();
        public List<SpriteRenderer> SpriteRenderers => m_SpriteRenderers;

    }

}