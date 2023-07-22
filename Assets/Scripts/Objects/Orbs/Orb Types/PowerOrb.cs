/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Orbs;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Orbs {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class PowerOrb : OrbObject {

        #region Enumerations.
        
        public enum Type {
            None, 
            Dash, 
            Hop, 
            Ghost, 
            Shadow, 
            Sticky
        }

        #endregion

        #region Variables.

        /* --- Members --- */

        // The type of orb this is.
        [SerializeField] 
        protected Type m_Type;

        // The palette for this particular orb.
        [SerializeField] 
        protected Texture2D m_Palette;
        public Texture2D Palette => m_Palette;

        #endregion

        #region Methods.

        // Collects this orb.
        protected override void OnTouch(CharacterController character) {
            base.OnTouch(character);

            // Swap the power based on the type of orb.
            character.DisableAllAbilityActions();
            switch (m_Type) {
                case Type.Dash:
                    character.Dash.Enable(character, true);
                    break;
                case Type.Hop:
                    character.Hop.Enable(character, true);
                    break;
                case Type.Ghost:
                    character.Ghost.Enable(character, true);
                    break;
                case Type.Shadow:
                    character.Shadow.Enable(character, true);
                    break;
                case Type.Sticky:
                    character.Sticky.Enable(character, true);
                    break;
                default:
                    character.Default.Enable(character, true);
                    break;
            }

            // The feedback on collecting a power orb.
            // Game.Visuals.Camera.RecolorScreen(m_Palette);

            // Disable the orb for a bit.
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
            StartCoroutine(IEReset());

        }
        
        // Resets the orb.
        public override void Reset() {
            m_SpriteRenderer.material.SetTexture("_TargetPalette", m_Palette);
            base.Reset();
        }

        #endregion
        
    }
}