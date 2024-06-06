/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gobblefish.Audio;
using Platformer.Character;

namespace Platformer.Tests {

    using Entities.Components;

    public class ActivateAllChildSpitters : MonoBehaviour {

        public List<int> nodeTriggers;

        public void Activate(int nodeIndex) {
            if (nodeTriggers.Contains(nodeIndex)) {
                foreach (Transform child in transform) {
                    if (child.GetComponent<Spitting>() != null) {
                        child.GetComponent<Spitting>().OnStartSpitting();
                    }
                }
            }
        }

    }

}