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
        public Vector2Int GridPosition => m_GridPosition;

        // The grid space coordinates this entity should be loaded at.
        [SerializeField, ReadOnly] 
        private Vector2Int m_WorldPosition = new Vector3(0, 0);
        public Vector2Int WorldPosition => m_WorldPosition;

        // The world space offset this entity should be loaded at.
        [SerializeField] 
        private Vector2 m_LoadOffset = new Vector2(0f, 0f);
        public Vector2 LoadOffset => m_LoadOffset;
        
        #endregion
        
        // Initialize this entity with the given settings.
        public void Init(List<LDtkTileData> controlData, Vector2Int vectorID, Vector2Int gridPosition, Vector2Int origin) {
            // Cache the grid position of this entity.
            m_Depth = transform.position.z;
            m_GridPosition = gridPosition;
            m_WorldPosition = Room.GridToWorldPosition(gridPosition, origin) + (Vector3)m_LoadOffset;            

            // Check whether there is a control at this position.
            LDtkTileData controlTile = controlData.Find(control => control.gridPosition == gridPosition);

            Initalize();
            SetRotation();
            SetLength();
            SetPathing();
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
        public static List<Entity> Generate(List<Entity> entities, List<LDtkTileData> entityData,  List<LDtkTileData> controlData,List<Entity> entityReferences, Transform parent, Vector2Int origin) {

            for (int i = 0; i < entityData.Count; i++) {

                // Check whether this exact entity has already been loaded.
                bool preloaded = entities.Find(ent => ent.VectorID == entityData[i].vectorID && ent.GridPosition == entityData[i].gridPosition) != null;
                
                // Get the reference entity.
                Entity entity = EntityManager.GetEntityByVectorID(entityData[i].vectorID, entityReferences);
                if (entity != null && !preloaded) {

                    // Swap the reference entity for a duplicated entity.
                    entity = entity.Duplicate(parent);
                    // Initialize the entity.
                    entity.Init(control, controlData, entityData, origin);
                    
                    entities.Add(entity);

                }
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
