/* --- Libaries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Movement = Controller.Movement;
using Direction = Controller.Direction;
using Airborne = Controller.Airborne;

/// <summary>
/// Handles the collision framework and animation
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Mesh : MonoBehaviour {

    /* --- Data Structures --- */
    [System.Serializable]
    public struct AnimationData {
        public Sprite[] animation;
        public int startIndex;
        public int length;
        public float? interval;
        public float timeInterval;

        public AnimationData(Sprite[] animation, int startIndex, int length, float? interval = null) {
            this.animation = animation;
            this.startIndex = startIndex;
            this.length = length;
            this.interval = interval;
            this.timeInterval = 0f;
        }
    }

    [System.Serializable]
    public class EffectData {
        
        [HideInInspector] public Sprite[] animation;
        [SerializeField] public Particle particle;
        [SerializeField] public int frame;

        public bool Check(AnimationData animationData, int currFrame) {
            return animation == animationData.animation && frame == currFrame;
        }

        public void Create() {
            print("Creating");
            if (particle == null) {
                return;
            }
            GameObject newParticleObject = Instantiate(particle.gameObject, particle.transform.position, Quaternion.identity, null);
            newParticleObject.SetActive(true);
        }

    }

    /* --- Dictionaries --- */
    public static Dictionary<Direction, Quaternion> DirectionQuaternions = new Dictionary<Direction, Quaternion>() {
        {Direction.Right, Quaternion.Euler(0, 0, 0) },
        {Direction.Left, Quaternion.Euler(0, 180, 0) }
    };

    /* --- Components --- */
    [HideInInspector] private Controller controller;
    [HideInInspector] private CircleCollider2D collisionBall;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [Space(2), Header("Collisions")]
    [SerializeField] public Hurtbox hurtbox; // Handles the damage collision checks.
    [SerializeField] public Feetbox feetbox; // Handles the ground collision checks.

    /* --- Parameters --- */
    [Space(2), Header("Animations")]
    [SerializeField] private Sprite[] idle = null;
    [SerializeField] private Sprite[] move = null;
    [SerializeField] private Sprite[] jump = null;
    [SerializeField] private float stretchiness = 0.1f;

    [Space(2), Header("Effects")]
    [SerializeField] private EffectData movementEffect;

    /* --- Properties --- */
    [SerializeField] private AnimationData animationData; // Used to set what the current active animation is.
    [SerializeField, ReadOnly] private Vector2 stretch = Vector2.zero;

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        int currFrame = Animate();
        Effect(currFrame);
        Flip();
        Stretch();
    }

    /* --- Methods --- */
    private void Init() {
        controller = transform.parent.GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collisionBall = GetComponent<CircleCollider2D>();
        animationData = new AnimationData(idle, 0, idle.Length);

        movementEffect.animation = move;

    }

    private int Animate() {
        GetAnimation();
        // Set the current frame.
        float frameRate = (animationData.interval != null) ? animationData.length / (float)animationData.interval : GameRules.FrameRate;
        int index = animationData.startIndex + ((int)Mathf.Floor(animationData.timeInterval * frameRate) % animationData.length);
        spriteRenderer.sprite = animationData.animation[index];
        return index;
    }

    private void Effect(int currFrame) {

        if (movementEffect.Check(animationData, currFrame)) {
            movementEffect.Create();
        }

    }

    private void Flip() {
        transform.localRotation = DirectionQuaternions[controller.directionFlag];
    }

    private void Stretch() {
        stretch = new Vector2(1f, 1f);
        if (controller.airborneFlag != Airborne.Grounded) {
            float horizontalStretch = Mathf.Abs(controller.body.velocity.x) * stretchiness;
            float verticalStretch = Mathf.Abs(controller.body.velocity.y) * stretchiness;
            Vector2 deltaStretch = new Vector2((horizontalStretch - verticalStretch) / 2f, verticalStretch - horizontalStretch);
            stretch += deltaStretch;
        }
        spriteRenderer.material.SetVector("_Stretch", (Vector4)stretch);
    }

    /* --- Sub-Methods --- */
    private void GetAnimation() {
        Sprite[] prevAnimation = animationData.animation;
        animationData.timeInterval += Time.deltaTime;
        animationData.interval = null;
        if (controller.airborneFlag != Airborne.Grounded) {
            animationData.animation = jump;
            animationData.startIndex = 0;
            animationData.length = jump.Length;
        }
        else if (controller.movementFlag != Movement.Idle) {
            animationData.animation = move;
            animationData.startIndex = 0;
            animationData.length = move.Length;
        }
        else {
            animationData.animation = idle;
            animationData.startIndex = 0;
            animationData.length = idle.Length;
        }
        if (prevAnimation != animationData.animation) {
            print("Switching Animation");
            if (animationData.animation == jump) {
                print("JUMP");
            }
            if (animationData.animation == move) {
                print("MOVE");
            }
            if (animationData.animation == idle) {
                print("IDLE");
            }
            animationData.timeInterval = 0f;
        }
    }


}
