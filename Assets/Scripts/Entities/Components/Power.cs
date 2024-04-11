// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Platformer;
using Platformer.Character;

// Definitions.
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
            Sticky,
            Bouncy,
            Gravity,
        }

        // The type of orb this is.
        [SerializeField] 
        protected Type m_Type;

        [SerializeField]
        private SpriteRenderer m_SpriteRenderer;

        // Collects this orb.
        public void SwapPower() {
            CharacterController character = PlayerManager.Character;

            // Swap the power based on the type of orb.
            character.DisableAllAbilityActions();
            switch (m_Type) {
                case Type.Dash:
                    character.GetPowerAction(typeof(ChargeDashAction).ToString()).EnablePower(character, true, m_SpriteRenderer.sprite);
                    break;
                case Type.Hop:
                    character.GetPowerAction(typeof(HopAction).ToString()).EnablePower(character, true, m_SpriteRenderer.sprite);
                    break;
                // case Type.Ghost:
                //     character.GetPowerAction<GhostAction>().Enable(character, true);
                //     break;
                // case Type.Shadow:
                //     // character.Shadow.Enable(character, true);
                //     break;
                case Type.Sticky:
                    character.GetPowerAction(typeof(StickyAction).ToString()).EnablePower(character, true, m_SpriteRenderer.sprite);
                    break;
                case Type.Bouncy:
                    character.GetPowerAction(typeof(BouncyAction).ToString()).EnablePower(character, true, m_SpriteRenderer.sprite);
                    break;
                case Type.Gravity:
                    character.GetPowerAction(typeof(GravityAction).ToString()).EnablePower(character, true, m_SpriteRenderer.sprite);
                    break;
                default:
                    character.Default.Enable(character, true);
                    break;
            }

        }
        
    }
}