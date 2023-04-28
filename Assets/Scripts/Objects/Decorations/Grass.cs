/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;
using IRotatable = Platformer.Levels.Entities.IRotatable;

namespace Platformer.Objects.Decorations {

    ///<summary>
    /// Decorates a level with grass.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Grass : MonoBehaviour, IRotatable {

        #region Variables.

        /* --- Components --- */
        
        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        
        /* --- Members --- */
        
        // The different types of grass available.
        [SerializeField] 
        private Sprite[] m_Variations;

        [SerializeField]
        private float m_Rotation;

        #endregion

        // Initalizes from the LDtk files.
        public void SetRotation(float rotation) {
            m_Rotation = rotation;
            transform.eulerAngles = Vector3.forward * rotation;
        }

        // Runs once on instantiation.
        void Start() {
            // m_SpriteRenderer.sortingLayerName = Screen.RenderingLayers.Foreground;
            // m_SpriteRenderer.color = Screen.ForegroundColorShift;
            Pick();
        }

        // Picks a random grass sprite.
        public void Pick() {
            // TODO: Implement
            int index = Random.Range(0, m_Variations.Length); // Utilities.RandomIndex(m_Variations.Length);
            m_SpriteRenderer.sprite = m_Variations[index];
        }

    }

}
