/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Blocks;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Blocks {

    ///<summary>
    /// A block that teleports the character between two points.
    ///<summary>
    public class PortalBlock : BlockObject {

        #region Enumerations.
        
        public enum PortalType {
            A, B, C, D
        }

        #endregion

        #region Variables.

        // The type of portal that this.
        [SerializeField]
        private PortalType m_PortalType = PortalType.A;
        public PortalType Type => m_PortalType;

        #endregion

        #region Methods.

        // Portals should always be active.
        protected override bool CheckActivationCondition() {
            return true;
        }

        // Finds the other portal that this portal teleports the character to.
        public PortalBlock FindPortalTarget() {
            PortalBlock[] portalBlocks = (PortalBlock[])GameObject.FindObjectsOfType<PortalBlock>();
            for (int i = 0; i < portalBlocks.Length; i++) {
                if (IsPortalTarget(portalBlocks[i])) {
                    return portalBlocks[i];
                }
            }
            return null;
            // return Game.Level.CurrentRoom.Entities.Find(entity => IsPortalTarget(entity.GetComponent<PortalBlock>(), m_PortalType));
        }

        // Checks whether something is the portal block.
        public bool IsPortalTarget(PortalBlock portalBlock) {
            if (portalBlock == null || portalBlock == this) { return false; }

            if (portalBlock.Type == m_PortalType) {
                return true;
            }
            return false;
        }

        
        // The functionality for when a block is touched.
        protected override void OnTouched(CharacterController character, bool touched) {
            if (touched) {
                PortalBlock portalTarget = FindPortalTarget();
                if (portalTarget != null) {
                    Teleport(character, portalTarget);
                }
            }
        }

        // Teleports the character to the given block.
        public void Teleport(CharacterController character, PortalBlock portalBlock) {
            Vector3 offset = (character.transform.position - transform.position).normalized;
            if (character.Body.velocity.y < 0f) {
                character.Body.velocity = new Vector2(character.Body.velocity.x, character.Default.JumpSpeed);
            }
            offset.y = -Mathf.Abs(offset.y);
            character.transform.position = portalBlock.transform.position - 0.75f * offset;
            Game.Physics.Time.RunHitStop(16);
        }

        #endregion

    }

}