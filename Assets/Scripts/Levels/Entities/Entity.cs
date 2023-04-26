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

        // The world space offset this entity should be loaded at.
        [SerializeField] 
        private Vector2 m_LoadOffset = new Vector2(0f, 0f);
        public Vector2 LoadOffset => m_LoadOffset;
        
        #endregion
        
        // Initialize this entity with the given settings.
        public void Init(Vector2Int gridPosition, Vector3 position) {
            m_GridPosition = gridPosition;
            transform.localPosition = position + (Vector3)m_LoadOffset;
            gameObject.SetActive(true);
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

        // Duplicate an entity.
        protected virtual Entity Duplicate(Transform parent) {
            Entity entity = Instantiate(gameObject, Vector3.zero, transform.localRotation, parent).GetComponent<Entity>();
            if (Singular) {
                Destroy(gameObject);
            }
            return entity;
        }

        // How to interpret the control data for this specific entity.
        public virtual void OnControl(int index, List<LDtkTileData> controlData) { }

        // Generate a entities from a list.
        public static List<Entity> Generate(List<Entity> entities, List<LDtkTileData> entityData,  List<Entity> entityReferences, Transform parent, Vector2Int origin) {

            for (int i = 0; i < entityData.Count; i++) {
                bool preloaded = entities.Find(ent => ent.VectorID == entityData[i].vectorID && ent.GridPosition == entityData[i].gridPosition) != null;
                Entity entity = EntityManager.GetEntityByVectorID(entityData[i].vectorID, entityReferences);
                if (entity != null && !preloaded) {
                    entity = entity.Duplicate(parent);
                    entities.Add(entity);
                    Vector3 entityPosition = Room.GridToWorldPosition(entityData[i].gridPosition, origin);
                    entity.Init(entityData[i].gridPosition, entityPosition);
                }
            }
            return entities;
        }

        // Set the control data for the entites.
        public static List<Entity> SetControls(List<Entity> entities, List<LDtkTileData> controlData) {
            for (int i = 0; i < controlData.Count; i++) {
                Entity entity = entities.Find(entity => entity.GridPosition == controlData[i].gridPosition);
                if (entity != null) {
                    entity.OnControl(i, controlData);
                }
            }
            return entities;
        }

    }

}
