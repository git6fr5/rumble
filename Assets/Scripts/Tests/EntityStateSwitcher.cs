/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Tests {

    using Entities.Components;
    using Entities.Triggers;
    using Entities.Utility;

    public class EntityStateSwitcher : MonoBehaviour {

        [System.Serializable]
        public class EntityState {
            public Pathing path;
            public GameObject activeObject;
        }

        public float stateChangeDuration = 1f;

        public List<EntityState> states = new List<EntityState>();
        public int stateID = 0;
        public int prevStateID = 0;

        public bool changingStates = false;

        Vector3 origin;
        Vector3 dest;

        public float ticks;

        void Start() {
            foreach (var s in states) {
                foreach (var n in s.path.nodes) {
                    n.OnReached.AddListener(OnReachedNode);
                }
                s.path.gameObject.SetActive(false);
                s.activeObject.SetActive(false);
            }
        }

        void OnReachedNode(int nodeIndex) {
            print("reached node " + nodeIndex.ToString());
            if (states[stateID].activeObject.GetComponent<Platformer.Tests.ActivateAllChildSpitters>() != null) {
                states[stateID].activeObject.GetComponent<Platformer.Tests.ActivateAllChildSpitters>().Activate(nodeIndex);
            }
        }

        void FixedUpdate() {

            if (stateID != prevStateID && !changingStates) {

                origin = transform.position;

                states[stateID].path.OnFinishResetting();
                dest = states[stateID].path.transform.position; 

                changingStates = true;

                transform.SetParent(null);
                states[prevStateID].path.gameObject.SetActive(false);

            }

            if (changingStates) {

                ticks += Time.fixedDeltaTime;
                transform.position = origin + (dest - origin) * ticks / stateChangeDuration;

                if (ticks / stateChangeDuration < 0.5f) {
                    if (states[prevStateID].activeObject != null) {
                        states[prevStateID].activeObject.transform.localScale = 2f * (0.5f - ticks / stateChangeDuration) * new Vector3(1f, 1f, 1f);
                    }
                }
                else {
                    if (states[prevStateID].activeObject != null) {
                        states[prevStateID].activeObject.SetActive(false);
                    }
                    if (states[prevStateID].activeObject != null) {
                        states[stateID].activeObject.SetActive(true);
                        states[stateID].activeObject.transform.localScale = 2f * (ticks / stateChangeDuration - 0.5f) * new Vector3(1f, 1f, 1f);
                    }
                }

                if (ticks >= stateChangeDuration) {
                    states[stateID].path.OnFinishResetting();
                    
                    transform.SetParent(states[stateID].path.transform);
                    transform.localPosition = Vector3.zero;

                    states[prevStateID].path.gameObject.SetActive(false);
                    states[stateID].path.gameObject.SetActive(true);

                    changingStates = false;
                    prevStateID = stateID;

                    ticks = 0f;

                }

            }

        }


    }

}