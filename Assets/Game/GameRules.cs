using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour {

    /* --- Static Tags --- */
    public static string PlayerTag = "Player";
    public static string AnimalTag = "Animal";
    public static string GroundTag = "Ground";

    /* --- Static Variables --- */
    // Instance.
    public static GameRules Instance;
    // Player.
    public static Controller MainPlayer;
    // Level
    public static LevelLoader MainLoader;
    // Objects.
    public static GameObject BackgroundObject;
    public static GameObject GameOverObject;
    public static GameObject GlobalLightObject;
    // Movement.
    public static float VelocityDamping = 0.95f;
    public static float MovementPrecision = 0.05f;
    public static float GravityScale = 1f;
    public static float Acceleration = 100f;
    public static float Floatiness = 0.4f;
    public static float KnockbackDuration = 0.15f;
    public static float BoundLimit = 100f;
    // Animation.
    public static float FrameRate = 24f;
    public static float OutlineWidth = 1f / 16f;
    // Camera.
    public static UnityEngine.Camera MainCamera;
    public static Vector3 MousePosition => MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

    /* --- Parameters --- */
    public float timeScale = 1f;

    /* --- Properties --- */
    public Controller mainPlayer;
    public LevelLoader levelLoader;
    public GameObject backgroundObject;
    public GameObject globalLightObject;
    public GameObject gameOverObject;
    public float velocityDamping = 0.95f;
    public float gravityScale;
    public float frameRate;
    public int cameraX;
    public int cameraY;
    public int pixelsPerUnit;
    public bool reset;
    public bool followPlayer;
    public Vector3 followOffset;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        if (followPlayer) {
            MainCamera.transform.position = MainPlayer.transform.position + followOffset;
        }
        // Shake the camera
        if (shake) {
            shake = Shake();
        }

        Time.timeScale = timeScale;
    }

    /* --- Methods --- */
    private void Init() {
        // Set these static variables.
        MainCamera = Camera.main;
        MainPlayer = mainPlayer;
        MainLoader = levelLoader;

        BackgroundObject = backgroundObject;
        GameOverObject = gameOverObject;
        GlobalLightObject = globalLightObject;
        // GlobalLightObject.SetActive(false);

        VelocityDamping = velocityDamping;
        GravityScale = gravityScale;
        FrameRate = frameRate;

        // Instance
        Instance = this;
    }

    [Header("Shake")]
    [SerializeField, ReadOnly] public float shakeStrength = 1f;
    [SerializeField, ReadOnly] public float shakeDuration = 0.5f;
    [SerializeField, ReadOnly] float elapsedTime = 0f;
    [SerializeField, ReadOnly] public bool shake;
    public AnimationCurve curve;

    public bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position += (Vector3)Random.insideUnitCircle * shakeStrength;
        return true;
    }

    /* --- Events --- */
    public static void CameraShake(float strength, float duration) {
        if (strength == 0f) {
            return;
        }
        if (!Instance.shake) {
            Instance.shakeStrength = strength;
            Instance.shakeDuration = duration;
            Instance.shake = true;
        }
        else {
            Instance.shakeStrength = Mathf.Max(Instance.shakeStrength, strength);
            Instance.shakeDuration = Mathf.Max(Instance.shakeDuration, Instance.elapsedTime + duration);
        }
    }

    public static void GameOver() {
        GameOverObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public static void ResetLevel() {
        if (MainLoader != null) {
            MainLoader.load = true;
        }
    }

    /* --- Debug --- */
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(cameraX, cameraY, 1f));
    }

}

public class ReadOnlyAttribute : PropertyAttribute {

}
