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

        public BoxCollider2D Hitbox => m_Hitbox;

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
                    TeleportC(character, portalTarget);
                }
            }
        }

        // Teleports the character to the given block.
        public void TeleportC(CharacterController character, PortalBlock portalBlock) {
            Vector3 offset = ((character.transform.position + (Vector3)character.Collider.offset) - transform.position);
            
            // The sum of the radius' of both the colliders.
            // Which is the offset the teleport has to be so that
            // they are not colliding after.
            Vector3 radiusOffset = new Vector3(1f, 1f, 0f) * (character.Collider.radius + 0.1f);

            // The position that the character should be moved at.
            // Edited as we figure out more information.
            Vector3 position = portalBlock.transform.position + (Vector3)portalBlock.Hitbox.offset - (Vector3)character.Collider.offset;

            // In order to check the direction that we need
            // to move the player.
            float dir = Mathf.Sign(character.Input.Direction.Horizontal);
            radiusOffset += Vector3.up * m_Hitbox.size.y;
            radiusOffset.x *= dir;
            radiusOffset += Vector3.right * dir * m_Hitbox.size.x / 2f;

            position += radiusOffset;

            // Teleport the character.
            Vector3 newVelocity = new Vector2(character.FacingDirection * character.Default.Speed, character.Default.JumpSpeed);
            Vector3 newPosition = position;

            character.gameObject.SetActive(false);

            StartCoroutine(IETeleport(character, newVelocity, newPosition));

            IEnumerator IETeleport(CharacterController character, Vector3 velocity, Vector3 position) {
                yield return new WaitForSeconds(0.5f);
                character.Body.velocity = velocity;      
                character.transform.position = position;
                Game.Physics.Time.RunHitStop(16);
            }
        
        }

        // Teleports the character to the given block.
        public void TeleportA(CharacterController character, PortalBlock portalBlock) {
            Vector3 offset = ((character.transform.position + (Vector3)character.Collider.offset) - transform.position);
            
            // The sum of the radius' of both the colliders.
            // Which is the offset the teleport has to be so that
            // they are not colliding after.
            float radSum = character.Collider.radius + 0.1f;

            // In order to check the direction that we need
            // to move the player.
            float y = Mathf.Abs(character.Body.velocity.y);
            float x = Mathf.Abs(character.Body.velocity.x);

            Vector3 position = portalBlock.transform.position + (Vector3)portalBlock.Hitbox.offset - (Vector3)character.Collider.offset;

            if (y > x) {

                radSum += m_Hitbox.size.y / 2f;
                position += Vector3.up * radSum;

            }
            else {

                float dir = Mathf.Sign(character.Body.velocity.x);
                radSum += m_Hitbox.size.x / 2f;
                position += Vector3.right * dir * radSum;

            }

            // Teleport the character.
            character.Body.velocity = new Vector2(character.Body.velocity.x, character.Default.JumpSpeed);            
            character.transform.position = position;
            Game.Physics.Time.RunHitStop(16);
        
        }

        #endregion

    }

}