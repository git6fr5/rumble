using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Stores specific data on how to generate the level.
/// </summary>
public class Environment : MonoBehaviour {

    /* --- Components --- */
    // Entities.
    [SerializeField] public Transform controlParentTransform; // The location to look for the entities.
    [SerializeField] public Transform decorParentTransform; // The location to look for the entities.
    [SerializeField] public Transform spiritParentTransform; // The location to look for the entities.
    [SerializeField] public Transform snailParentTransform; // The location to look for the entities.
    [SerializeField] public Transform platformParentTransform; // The location to look for the entities.
    // Tiles.
    [SerializeField] public RuleTile floorTile; // A set of sprites used to tile the floor of the level.

    /* --- Parameters --- */

    /* --- Properties --- */
    [SerializeField, ReadOnly] public List<Entity> controls;
    [SerializeField, ReadOnly] public List<Entity> decor;
    [SerializeField, ReadOnly] public List<Entity> snails;
    [SerializeField, ReadOnly] public List<Entity> spirits;
    [SerializeField, ReadOnly] public List<Entity> platforms;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        RefreshEntities();
    }

    /* --- Entity Methods --- */
    // Refreshes the set of entities.
    void RefreshEntities() {
        FindEntities(controlParentTransform, ref controls);
        FindEntities(decorParentTransform, ref decor);
        FindEntities(snailParentTransform, ref snails);
        FindEntities(spiritParentTransform, ref spirits);
        FindEntities(platformParentTransform, ref platforms);
    }

    private void FindEntities(Transform parent, ref List<Entity> entityList) {
        entityList = new List<Entity>();
        foreach (Transform child in parent) {
            FindAllEntitiesInTransform(child, ref entityList);
        }
    }

    // Recursively searches through the transform for all entity components.
    void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {

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

    // Returns the first found entity with a matching ID.
    public Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
        for (int i = 0; i < entityList.Count; i++) {
            if (entityList[i].vectorID == vectorID) {
                return entityList[i];
            }
        }
        return null;
    }

}
