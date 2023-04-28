/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels.LDtk;
using Platformer.Levels.Entities;

namespace Platformer.Levels.Entities {

    [System.Serializable]
    public class RotationID {
        public float Rotation;
        public Vector2Int VectorID;
    }

    interface IInitializable {
        void Initialize(Vector3 worldPosition, float depth);
    }

    /// <summary>
    /// An entity object readable by the level loader.
    /// <summary>
    public class Entity : MonoBehaviour {

        #region Fields.

        // Whether this entity should be duplicated or taken directly.       
        [SerializeField] 
        private bool m_Singular = false;
        public bool Singular => m_Singular;

        // Whether this entity can be unloaded.
        [SerializeField] 
        private bool m_Unloadable = true;
        public bool Unloadable => m_Unloadable;

        // The mapping from LDtk to this entity.
        [SerializeField] 
        protected Vector2Int m_VectorID = new Vector2Int(0, 0);
        public Vector2Int VectorID => m_VectorID;

        // The grid space coordinates this entity should be loaded at.
        [SerializeField, ReadOnly] 
        private Vector2Int m_GridPosition = new Vector2Int(0, 0);
        public Vector2Int GridPosition {
            get { return m_GridPosition; }
            set {
                m_GridPosition = value;
            } 
        }

        // The world space offset this entity should be loaded at.
        [SerializeField] 
        private Vector2 m_LoadOffset = new Vector2(0f, 0f);
        public Vector2 LoadOffset => m_LoadOffset;

        [SerializeField]
        private List<RotationID> m_Rotations = new List<RotationID>();
        public List<RotationID> Rotations => m_Rotations;
        
        #endregion
        
        // Initialize this entity with the given settings.
        public void Init(List<LDtkTileData> controlData, Vector2Int roomOrigin) {
            // Cache the grid position of this entity.
            float depth = transform.position.z;
            Vector3 worldPosition = Room.GridToWorldPosition(m_GridPosition, roomOrigin) + (Vector3)m_LoadOffset;            

            // Check whether there is a control at this position.
            LDtkTileData controlTile = controlData.Find(control => control.gridPosition == gridPosition);

            Initalize(worldPosition, depth);
            SetRotation();

            // Depends on raycasting.
            SetLength(controlTile.index, controlData);
            SetPathing(controlTile.index, controlData); // (has to come after length is set)
            SetSpin(controlTile.index, controlData);
            
            gameObject.SetActive(true);
            
        }

        public void Activate() {
            IInitializable initializable = entity.GetComponent<IInitializable>();
            if (initializable == null) { return; }

            initializable.Activate(m_WorldPosition, m_Depth);

        }

        public void SetCurrentVectorID(Vector2Int vectorID) {
            m_VectorID = vectorID;
        }

        // Generate a entities from a list.
        public static List<Entity> Generate(List<Entity> entities, List<LDtkTileData> entityData,  List<LDtkTileData> controlData,List<Entity> entityReferences, Transform parent, Vector2Int roomOrigin) {

            for (int i = 0; i < entityData.Count; i++) {

                // Check whether this exact entity has already been loaded.
                bool preloaded = entities.Find(ent => ent.VectorID == entityData[i].vectorID && ent.GridPosition == entityData[i].gridPosition) != null;
                
                // Get the reference entity.
                Entity entity = EntityManager.GetEntityByVectorID(entityData[i].vectorID, entityReferences);
                if (entity != null && !preloaded) {
                    // Swap the reference entity for a duplicated entity.
                    entity = entity.Duplicate(parent);
                    entity.GridPosition = entityData[i].gridPosition;
                    entities.Add(entity);
                }
            }

            for (int i = 0; i < entities.Count; i++) {
                entity.Init(controlData, roomOrigin);
            }

            return entities;

        }

        // Duplicate an entity.
        protected virtual Entity Duplicate(Transform parent) {
            Entity entity = Instantiate(gameObject, Vector3.zero, transform.localRotation, parent).GetComponent<Entity>();
            if (Singular) {
                Destroy(gameObject);
            }
            return entity;
        }

        // Destroy and clean through a list of entities.
        public static List<Entity> Destroy(List<Entity> entities) {
            if (entities == null || entities.Count == 0) { return entities; }
            
            for (int i = 0; i < entities.Count; i++) {
                bool destroy = entities[i] != null && entities[i].Unloadable;
                if (destroy) {
                    Destroy(entities[i].gameObject);
                }
            }
            return entities;
        }

    }

}
