/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 
/// </summary>
public class ExplodingPlatform : MovingPlatform {

    /* --- Static Variables --- */
    public static float DeathDelay = 2f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] public float timer = 0f;

    // Sets the target for this platform.
    protected override void Effect() {

        bool playerOnThisShit = false;
        Animal animal = null;
        for (int i = 0; i < container.Count; i++) {
            Controller controller = container[i];
            if (controller.GetComponent<Player>() != null) {
                playerOnThisShit = true;
                break;
            }
            if (controller.GetComponent<Animal>() != null) {
                animal = controller.GetComponent<Animal>();
                if (animal.isControlled) {
                    playerOnThisShit = true;
                    break;
                }
            }
        }

        if (playerOnThisShit) {
            timer += Time.deltaTime;
            Player player = (Player)GameObject.FindObjectOfType(typeof(Player));
            if (player != null) {
                player.Freeze(timer, DeathDelay, Color.blue);
            }
            if (animal != null) {
                animal.Freeze(timer, DeathDelay, Color.blue);
            }
        }
        else {
            timer -= 0.25f * Time.deltaTime;
            if (timer <= 0f) {
                timer = 0f;
            }
            else {
                Player player = (Player)GameObject.FindObjectOfType(typeof(Player));
                if (player != null) {
                    player.Freeze(timer, DeathDelay, Color.blue);
                }
                if (animal != null) {
                    animal.Freeze(timer, DeathDelay, Color.blue);
                }
            }
        }

        if (timer > DeathDelay) {
            GameRules.ResetLevel();
        }

    }


}
