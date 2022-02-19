using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLabel : MonoBehaviour {

    public Label label;
    string levelName = "";

    public void Init(string name) {
        string[] splitString = name.Split('_');
        string concatString = "";
        for (int i = 0; i < splitString.Length; i++) {
            concatString += splitString[i] + " ";
        }

        levelName = concatString;
    }

    void Update() {
        label.SetText(levelName);
    }

}
