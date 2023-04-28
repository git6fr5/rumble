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

    ///<summary>
    ///
    ///<summary>
    public class EntityManager : MonoBehaviour {

        #region Variables.

        // A list of all the reference entities.
        [SerializeField] 
        private List<Entity> m_References;

        // A list of all the entities.
        public List<Entity> All => m_References;

        #endregion

        #region Methods.

        public void OnGameLoad() {
            FindEntities(transform, ref m_References);
        }

        private static  void FindEntities(Transform parent, ref List<Entity> entityList) {
            entityList = new List<Entity>();
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child, ref entityList);
            }
        }

        private static void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {
            // If we've found an entity, don't go any deeper.
            if (parent.GetComponent<Entity>() != null) {
                entityList.Add(parent.GetComponent<Entity>());
            }
            else if (parent.childCount > 0) {
                foreach (Transform child in parent) {
                    FindAllEntitiesInTransform(child, ref entityList);
                }
            }
        }
        
        public static Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
            for (int i = 0; i < entityList.Count; i++) {
                if (entityList[i].VectorID == vectorID) {
                    return entityList[i];
                }
                else {
                    if (entityList[i].Rotations != null && entityList[i].Rotations.Count > 0) {
                        for (int j = 0; j < entityList[i].Rotations.Count; j++) {
                            if (entityList[i].Rotations[j].VectorID == vectorID) {
                                entityList[i].SetCurrentVectorID(vectorID);
                                return entityList[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        #endregion

    }
    
}