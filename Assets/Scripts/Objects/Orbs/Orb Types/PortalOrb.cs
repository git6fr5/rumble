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
    /// A block that teleports the character between two points.
    ///<summary>
    public class PortalOrb : OrbObject {

        #region Enumerations.
        
        public enum PortalType {
            A, B, C, D
        }

        #endregion

        // The type of portal that this.
        [SerializeField]
        private PortalType m_PortalType = PortalType.A;
        public PortalType Type => m_PortalType;

        [SerializeField]
        private Transform m_ActiveIndicator;

        #region Methods.

        // The functionality for when a block is touched.
        protected override void OnTouch(CharacterController character) {
            base.OnTouch(character);

            PortalOrb targetPortal = FindPortalTarget();
            if (targetPortal != null) {
                Teleport(character, targetPortal);
            }
        }

        // Finds the other portal that this portal teleports the character to.
        public PortalOrb FindPortalTarget() {
            PortalOrb[] portals = (PortalOrb[])GameObject.FindObjectsOfType<PortalOrb>();
            for (int i = 0; i < portals.Length; i++) {
                if (IsPortalTarget(portals[i])) {
                    return portals[i];
                }
            }
            return null;
            // return Game.Level.CurrentRoom.Entities.Find(entity => IsPortalTarget(entity.GetComponent<PortalBlock>(), m_PortalType));
        }

        // Checks whether something is the portal block.
        public bool IsPortalTarget(PortalOrb portal) {
            if (portal == null || portal == this) { return false; }

            if (portal.Type == m_PortalType) {
                return true;
            }
            return false;
        }

        // Teleports the character to the given block.
        public void Teleport(CharacterController character, PortalOrb targetPortal) {

            targetPortal.OnTeleportedTo();

            // Disable the orb for a bit.
            m_ActiveIndicator.gameObject.SetActive(false);
            m_Hitbox.enabled = false;
            StartCoroutine(IEReset());

            character.Body.velocity = new Vector2(character.Body.velocity.x, Mathf.Abs(character.Body.velocity.y));
            character.transform.position = targetPortal.transform.position;

        }

        public void OnTeleportedTo() {
            // Disable the orb for a bit.
            m_ActiveIndicator.gameObject.SetActive(false);
            m_Hitbox.enabled = false;
            StartCoroutine(IEReset());
        }

        // Resets the object to its default state.
        public override void Reset() {
            base.Reset();
            m_ActiveIndicator.gameObject.SetActive(true);
        }

        #endregion

    }

}