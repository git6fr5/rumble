// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Platformer.Tests {

    public class PlayTest : MonoBehaviour {

        public TMP_InputField inputField;

        public void OnPlay() {
            Debug.Log(inputField.text);
        }

    }

}