using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

public class AutoLoader : LevelLoader {

    public List<Banana> bananas = new List<Banana>();
    public Level[] levels;
    public int numLevelsToLoad;
    public Banana banana;

    void Start() {

    }

    // Update is called once per frame
    protected override void Update() {
        if (load) {
            LoadLevels();
            load = false;
        }
        ActivateLevels();
    }

    private void LoadLevels() {

        Level newLevel = Instantiate(level, Vector3.zero, Quaternion.identity, transform).GetComponent<Level>();
        OpenLevel(newLevel, id);
        newLevel.gameObject.SetActive(true);

        for (int i = 1; i < numLevelsToLoad; i++) {
            newLevel = Instantiate(level, Vector3.zero, Quaternion.identity, transform).GetComponent<Level>();
            OpenLevel(newLevel, id + i, false);

            Banana newBanana = Instantiate(banana, Vector3.zero, Quaternion.identity, transform).GetComponent<Banana>();
            newBanana.transform.position = newLevel.GridToWorldPosition(newLevel.controlPosition + new Vector2Int(0, -1));
            newBanana.level = newLevel;
            newBanana.gameObject.SetActive(true);

            bananas.Add(newBanana);

        }

    }

    private void ActivateLevels() {

        for (int i = 0; i < bananas.Count; i++) {

            if ((GameRules.MainPlayer.transform.position - bananas[i].transform.position).sqrMagnitude < 50f * 50f) {
                bananas[i].level.gameObject.SetActive(true);
            }

        }

    }

}
