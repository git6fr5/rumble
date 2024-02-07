/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;

/* --- Definitions --- */
using Game = Platformer.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class Power : MonoBehaviour {

        public enum Type {
            None, 
            Dash, 
            Hop, 
            Ghost, 
            Shadow, 
            Sticky
        }

        // The type of orb this is.
        [SerializeField] 
        protected Type m_Type;

        // Collects this orb.
        public void SwapPower() {
            CharacterController character = Game.MainPlayer;

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
                    // character.Ghost.Enable(character, true);
                    break;
                case Type.Shadow:
                    // character.Shadow.Enable(character, true);
                    break;
                case Type.Sticky:
                    character.Sticky.Enable(character, true);
                    break;
                default:
                    character.Default.Enable(character, true);
                    break;
            }

        }
        
    }
}