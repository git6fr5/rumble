// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Levels {

    public class DecorationSection : MonoBehaviour {

        public static DecorationSection New(string name) {
            return new GameObject(name, typeof(DecorationSection)).GetComponent<DecorationSection>();
        }

    }

}
