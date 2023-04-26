/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;
using Platformer.Objects;

namespace Platformer.Levels.Entities {

    /// <summary>
    /// An entity object readable by the level loader.
    /// That specifically inteprets control data in order to get
    /// a position within a cycle.
    /// <summary>
    public class RotatableEntity : Entity {

        #region Data.

        [System.Serializable]
        public class RotationID {
            public float Rotation;
            public Vector2Int VectorID;
        }

        #endregion

        #region Variables.

        [SerializeField]
        private List<RotationID> m_Rotations = new List<RotationID>();
        public List<RotationID> Rotations => m_Rotations;

        #endregion

        // Duplicate an entity.
        protected override Entity Duplicate(Transform parent) {
            Entity entity = base.Duplicate(parent);
            entity.GetComponent<RotatableEntity>().SetRotation();
            return entity;
        }

        public void SetCurrentVectorID(Vector2Int vectorID) {
            m_VectorID = vectorID;
        }

        public virtual void SetRotation() {}

        protected float GetRotation() {
            RotationID rotationID = m_Rotations.Find(rotationID => rotationID.VectorID == m_VectorID);
            if (rotationID == null) {
                print("why?");
                return 0f;
            }
            print(rotationID.VectorID);
            return rotationID.Rotation;
        }

    }

}