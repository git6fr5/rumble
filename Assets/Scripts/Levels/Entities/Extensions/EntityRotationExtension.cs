/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Levels.Entities {

    public interface IRotatable {
        void SetRotation(float rotation);
    }

    public static class EntityRotationExtensions {

        public static void SetRotation(this Entity entity) {
            IRotatable rotatable = entity.GetComponent<IRotatable>();
            if (rotatable == null) {
                return;
            }
            
            float rotation = entity.GetRotation(Rotations);
            rotatable.SetRotation(rotation);

        }

        public static float GetRotation(this Entity entity, List<RotationID> rotations) {
            // Get the current rotation ID and return its rotation.
            RotationID rotationID = rotations.Find(rotationID => rotationID.VectorID == m_VectorID);
            if (rotationID == null) {
                return 0f;
            }
            return rotationID.Rotation;
        }

    }
}