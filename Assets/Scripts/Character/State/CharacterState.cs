/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;
using Platformer.Obstacles;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

using Platformer.Decor;
using Star = Platformer.Obstacles.Star;

namespace Platformer.Character {

    ///<summary>
    /// A set of data that defines the state
    /// of a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterState : MonoBehaviour {

        #region Variables

        // Components.
        private InputSystem m_Input => GetComponent<InputSystem>();
        public InputSystem Input => m_Input;
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public Rigidbody2D Body => m_Body;
        private CircleCollider2D m_Collider => GetComponent<CircleCollider2D>();
        public CircleCollider2D Collider => m_Collider;

        // Settings.
        public bool IsPlayer => this == Game.MainPlayer;
        [SerializeField, ReadOnly] private bool m_OnGround;
        public bool OnGround => m_OnGround;
        [SerializeField, ReadOnly] private bool m_FacingWall;
        public bool FacingWall => m_FacingWall;
        public Vector3 FacingDirection => m_Input.Direction.Facing * Vector3.right;
        [SerializeField, ReadOnly] private float m_DisableTicks = 0f;
        public bool Disabled => m_DisableTicks > 0f;
        [SerializeField, ReadOnly] private bool m_MovementOverride = false;
        public bool MovementOverride => m_MovementOverride;
        [SerializeField, ReadOnly] private bool m_FallOverride = false;
        public bool FallOverride => m_FallOverride;
        
        // Actions.
        [SerializeField] private MoveAction m_Movement;
        public MoveAction Move => m_Movement;
        [SerializeField] private FlyAction m_Fly;
        public FlyAction Fly => m_Fly;
        [SerializeField] private FallAction m_Fall;
        public FallAction Fall => m_Fall;
         [SerializeField] private ClimbAction m_Climb;
        public ClimbAction Climb => m_Climb;

        [SerializeField] private JumpAction m_Jump;
        public JumpAction Jump => m_Jump;
        [SerializeField] private DashAction m_Dash;
        public DashAction Dash => m_Dash;
        [SerializeField] private HopAction m_Hop;
        public HopAction Hop => m_Hop;
        [SerializeField] private GhostAction m_Ghost;
        public GhostAction Ghost => m_Ghost;
        [SerializeField] private ShadowAction m_Shadow;
        public ShadowAction Shadow => m_Shadow;
        [SerializeField] private StickyAction m_Sticky;
        public StickyAction Sticky => m_Sticky;

        [SerializeField] private RespawnBlock m_RespawnBlock;
        public RespawnBlock Respawn => m_RespawnBlock;
        [SerializeField] private AudioClip m_ResetSound;
        [SerializeField] private AudioClip m_ResetSoundB;

        [SerializeField] private List<Star> m_Stars = new List<Star>();

        public Dust ExplodeDust;
        public Sparkle TrailSparkle;

        #endregion

        public void Reset() {
            // Game.HitStop();
            // transform.position = Vector3.up * 1.5f + m_RespawnBlock.Origin;
            ExplodeDust.Activate();
            TrailSparkle.Stop();

            Game.Score.AddDeath();
            
            DisableAllAbilityActions();
            m_Body.velocity = Vector2.zero;
            m_Body.gravityScale = 0f;
            transform.position = Vector3.up * 1.5f + m_RespawnBlock.Origin;

            Disable(0.12f);
            OverrideFall(true);
            OverrideMovement(true);
            Screen.Recolor(Screen.DefaultPalette);

            SoundManager.PlaySound(m_ResetSound, 0.15f);
            SoundManager.PlaySound(m_ResetSoundB, 0.15f);

            StartCoroutine(IEReset());

            Star[] stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
            for (int i = 0; i < stars.Length; i++) {
                stars[i].Reset();
            }

        }

        private IEnumerator IEReset() {
            // m_Collider.enabled = false;
            yield return new WaitForSeconds(0.12f);
            // m_Collider.enabled = true;
            OverrideFall(false);
            OverrideMovement(false);

            GhostBlock[] ghostBlocks = (GhostBlock[])GameObject.FindObjectsOfType(typeof(GhostBlock));
            for (int i = 0; i < ghostBlocks.Length; i++) {
                ghostBlocks[i].Reset();
            }

            // Orb[] orbs = (Orb[])GameObject.FindObjectsOfType(typeof(Orb));
            // for (int i = 0; i < orbs.Length; i++) {
            //     orbs[i].Reset();
            // }

        }

        public void DisableAllAbilityActions() {
            m_Dash.Enable(this, false);
            m_Hop.Enable(this, false);
            m_Ghost.Enable(this, false);
            m_Shadow.Enable(this, false);
            m_Sticky.Enable(this, false);
        }

        public void SetResetPoint(RespawnBlock block) {
            m_RespawnBlock = block;

            Star[] stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
            for (int i = 0; i < stars.Length; i++) {
                if (stars[i].Following == transform) {
                    stars[i].Collect();
                }
            }

        }

        public void Disable(float duration) {
            Timer.Start(ref m_DisableTicks, duration);
        }

        public void OverrideMovement(bool enable) {
            m_MovementOverride = enable;
        }

        public void OverrideFall(bool enable) {
            m_FallOverride = enable;
        }

        void Update() {
            m_Jump.Process(m_Body, m_Input, this);
            m_Dash.Process(m_Body, m_Input, this);
            m_Hop.Process(m_Body, m_Input, this);
            m_Ghost.Process(m_Body, m_Input, this);
            m_Shadow.Process(m_Body, m_Input, this);
            m_Sticky.Process(m_Body, m_Input, this);
        }

        void FixedUpdate() {
            Timer.TickDown(ref m_DisableTicks, Time.fixedDeltaTime);
            m_OnGround = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.down, Game.Physics.CollisionLayers.Ground);
            m_FacingWall = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, FacingDirection,  Game.Physics.CollisionLayers.Ground);

            m_Jump.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Dash.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Hop.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Ghost.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Shadow.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Sticky.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);

            m_Movement.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Fall.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Fly.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Climb.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
            
        }

    }

}

