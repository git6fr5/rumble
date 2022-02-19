using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour {

    public Label label;

    void Update() {
        string instruction = "";

        Spirit spirit = GameRules.MainPlayer.spirit;
        if (spirit == null) {
            instruction = "To Possess";
        }
        else if (spirit.GetComponent<FireSpirit>() != null) {
            instruction = "To Fly";
        }
        else if (spirit.GetComponent<LightSpirit>() != null) {
            instruction = "To Dash";
        }

        label.SetText(instruction);

    }
}
