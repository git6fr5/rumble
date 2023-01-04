/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteShapeController))]
    public class Obstacle : MonoBehaviour {

        #region Variables.

        /* --- Components --- */

        // The box collider attached to this platform.
        protected BoxCollider2D m_Collider => GetComponent<BoxCollider2D>();
        
        // The sprite shape renderer attached to this platform.
        protected SpriteShapeRenderer m_SpriteShapeRenderer => GetComponent<SpriteShapeRenderer>();
        
        // The sprite shape controller. attached to this platform.
        protected SpriteShapeController m_SpriteShapeController => GetComponent<SpriteShapeController>();

        // The spline attached to the sprite shape.
        protected Spline m_Spline => m_SpriteShapeController.spline;

        /* --- Members --- */
        
        // The position that this platform was spawned at.
        [HideInInspector]
        protected Vector3 m_Origin;
        
        // The path that this platform follows.
        [HideInInspector] 
        protected Vector3[] m_Path = null;
        
        // The current position in the path that the path is following.
        [SerializeField, ReadOnly] 
        protected int m_PathIndex;
        
        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            m_Origin = transform.position;
            m_SpriteShapeRenderer.sortingLayerName = Game.Visuals.RenderingLayers.Foreground;
        }

        #endregion

    }
    
}
