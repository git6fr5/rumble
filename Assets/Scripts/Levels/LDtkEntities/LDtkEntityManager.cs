/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.

namespace Platformer.Levels {

    [ExecuteInEditMode]
    public class LDtkEntityManager : MonoBehaviour {

        [System.Serializable]
        public class LDtkEntitySet {
            public string name;
            public List<LDtkEntity> collection;
        }

        // A list of all the reference entities.
        [SerializeField] 
        private List<LDtkEntitySet> m_LDtkEntities;
        private List<LDtkEntity> m_References = new List<LDtkEntity>();

        // A list of all the entities.
        public List<LDtkEntity> All => m_References;

        void Update() {
            if (!Application.isPlaying) {
                m_References = new List<LDtkEntity>();
                for (int i = 0; i < m_LDtkEntities.Count; i++) {
                    foreach (LDtkEntity ldtkEnt in m_LDtkEntities[i].collection) {
                        m_References.Add(ldtkEnt);
                    }
                }
            }
        }

        public LDtkEntity GetEntityByVectorID(Vector2Int vectorID) {
            for (int i = 0; i < m_References.Count; i++) {
                if (m_References[i].VectorID == vectorID) {
                    return m_References[i];
                }
                else {
                    if (m_References[i].Rotations != null && m_References[i].Rotations.Count > 0) {
                        for (int j = 0; j < m_References[i].Rotations.Count; j++) {
                            if (m_References[i].Rotations[j].VectorID == vectorID) {
                                m_References[i].SetCurrentVectorID(vectorID);
                                return m_References[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        // Generate a entities from a list.
        public List<LDtkEntity> Generate(LevelSection section, LDtkLayers ldtkLayers) {

            // entities, entityData, controlData, entityReferences, transform, worldPosition

            List<LDtkTileData> entityData = LDtkReader.GetLayerData(section.ldtkLevel, ldtkLayers.Entity);
            List<LDtkTileData> pathData = LDtkReader.GetLayerData(section.ldtkLevel, ldtkLayers.Path);

            for (int i = 0; i < entityData.Count; i++) {

                // Check whether this exact entity has already been loaded.
                bool preloaded = section.entities.Find(ent => ent.VectorID == entityData[i].vectorID && ent.GridPosition == entityData[i].gridPosition) != null;
                
                // Get the reference entity.
                LDtkEntity entity = GetEntityByVectorID(entityData[i].vectorID);
                if (entity != null && !preloaded) {
                    // Swap the reference entity for a duplicated entity.
                    entity = entity.Duplicate(section.transform);
                    entity.SetGridValues(entityData[i].gridPosition, entityData[i].gridSize);

                    //
                    entity.gameObject.SetActive(true);
                    entity.SetRotation();
                    entity.SetPosition(section.worldPosition);
                    entity.SetPath(pathData);
                    entity.SetLength(entityData);

                    //
                    section.entities.Add(entity);
                }
            }
   
            return section.entities;

        }

        // Destroy and clean through a list of entities.
        public static List<LDtkEntity> Destroy(List<LDtkEntity> entities) {
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