using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

public class WorldLoader : LevelLoader {

    public string startingLevelName = "The_Beginning";

    public Level[] levels;
    public List<Level> loadLevels = new List<Level>();
    public List<Level> loadedLevels = new List<Level>();
    public List<Level> deloadLevels = new List<Level>();

    void Start() {
        CollectLevels();
        LoadLevels();
        StartCoroutine(IELoadLevels());
        StartCoroutine(IESetPlayer());

    }

    private IEnumerator IESetPlayer() {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < levels.Length; i++) {
            if (startingLevelName == levels[i].levelName && levels[i].controlPositions != null && levels[i].controlPositions.Count > 0) {
                GameRules.MainPlayer.transform.position = levels[i].GridToWorldPosition(levels[i].controlPositions[0]);
                GameRules.MainPlayer.body.velocity = Vector2.zero;
                break;
            }
        }
        yield return null;
    }

    // Update is called once per frame
    protected override void Update() {
        // base.Update();
    }

    private IEnumerator IELoadLevels() {
        while (true) {
            LoadLevels();
            yield return new WaitForSeconds(0.25f);
        }
    }

    public override void Reset() {
        ResetAll();
    }

    private void ResetAll() {
        for (int i = 0; i < loadedLevels.Count; i++) {
            ResetLevel(loadedLevels[i]);
        }
        loadedLevels = new List<Level>();
        LoadLevels();
    }

    private void LoadLevels() {

        Controller player = GameRules.MainPlayer;
        loadLevels = new List<Level>();
        deloadLevels = new List<Level>();

        for (int i = 0; i < levels.Length; i++) {
            Level thisLevel = levels[i];
            for (int j = 0; j < thisLevel.controlPositions.Count; j++) {
                // newBanana.transform.position = newLevel.GridToWorldPosition(newLevel.controlPosition + new Vector2Int(0, -1));
                Vector3 position = thisLevel.GridToWorldPosition(thisLevel.controlPositions[j] + new Vector2Int(0, -1));
                Debug.DrawLine(player.transform.position, position, Color.red, 0.25f);
                if (!loadLevels.Contains(thisLevel) && (position - player.transform.position).sqrMagnitude < GameRules.BoundLimit * GameRules.BoundLimit) {
                    loadLevels.Add(thisLevel);
                }
            }
        }

        for (int i = 0; i < loadLevels.Count; i++) {
            if (!loadedLevels.Contains(loadLevels[i])) {
                OpenLevelByName(loadLevels[i], loadLevels[i].levelName, false);
                loadedLevels.Add(loadLevels[i]);
            }
        }

        for (int i = 0; i < loadedLevels.Count; i++) {
            if (!loadLevels.Contains(loadedLevels[i])) {
                deloadLevels.Add(loadedLevels[i]);
            }
        }

        for (int i = 0; i < deloadLevels.Count; i++) {
            ResetLevel(deloadLevels[i]);
            loadedLevels.Remove(deloadLevels[i]);
        }

    }

    private void CollectLevels() {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();
        List<Level> L_Levels = new List<Level>();

        for (int i = 0; i < json.Levels.Length; i++) {
            print(json.Levels[i].Identifier);

            // Create a new level instance.
            Level newLevel = Instantiate(level.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Level>();

            // Read the json data into that instance.
            newLevel.jsonID = i;
            newLevel.levelName = json.Levels[i].Identifier;
            newLevel.gameObject.name = newLevel.levelName;
            newLevel.gridSize = (int)json.DefaultGridSize;
            newLevel.height = (int)(json.Levels[i].PxHei / json.DefaultGridSize);
            newLevel.width = (int)(json.Levels[i].PxWid / json.DefaultGridSize);

            newLevel.worldHeight = (int)(json.Levels[i].WorldY / json.DefaultGridSize);
            newLevel.worldWidth = (int)(json.Levels[i].WorldX / json.DefaultGridSize);

            // Get the control positions.
            List<LDtkTileData> controlData = LoadLayer(json.Levels[i], ControlLayer, newLevel.gridSize);
            for (int j = 0; j < controlData.Count; j++) {
                if (controlData[j].vectorID == CheckPointID) {
                    newLevel.controlPositions.Add(controlData[j].gridPosition);
                }
            }

            // Store the data into an array.
            L_Levels.Add(newLevel);

        }

        levels = L_Levels.ToArray();

    }

}
