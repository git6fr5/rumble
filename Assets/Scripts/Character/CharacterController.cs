/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilities;
using Platformer.Input;
using Platformer.Obstacles; // For the respawn block, the shadow block and the orbs.
using Platformer.Decor; // For the dust and sparkles
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Character {

    ///<summary>
    /// Controls a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterState : MonoBehaviour {

        #region Variables.

        /* --- Components --- */

        // The input system attached to this body.
        private InputSystem m_Input => GetComponent<InputSystem>();
        public InputSystem Input => m_Input;

        // The rigidbody attached to this controller.
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public Rigidbody2D Body => m_Body;
        
        // The main collider attached to this body.
        private CircleCollider2D m_Collider => GetComponent<CircleCollider2D>();
        public CircleCollider2D Collider => m_Collider;

        /* --- Members --- */

        // Checks whether the character is on the ground.
        [SerializeField, ReadOnly] 
        private bool m_OnGround;
        public bool OnGround => m_OnGround;
        
        // Checks whether the character is facing a wall.
        [SerializeField, ReadOnly] 
        private bool m_FacingWall;
        public bool FacingWall => m_FacingWall;
        
        // Checks what direction the controller is facing.
        [SerializeField, ReadOnly] 
        private bool m_FacingDirection;
        public bool FacingDirection => m_FacingDirection;

        // Whether the direction that this is facing is locked.
        [SerializeField, ReadOnly]
        private bool m_DirectionLocked = false;

        // Checks whether the character is rising.
        [SerializeField, ReadOnly] 
        private bool m_Rising;
        public bool Rising => m_Rising;

        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly] 
        private Timer m_DisableTimer = 0f;
        public bool Disabled => m_DisableTimer.Active;

        // The block that this character respawns at.
        [SerializeField] 
        private RespawnBlock m_RespawnBlock;
        public RespawnBlock RespawnBlock => m_RespawnBlock;

        // The effect thats played to show an impact for this character.
        [SerializeField] 
        private Dust m_ImpactEffect;
        
        // The effect thats played to show this characters movement.
        [SerializeField] 
        private Sparkle m_TrailEffect;
        
        // The sound thats played when the character dies.
        [SerializeField] 
        private AudioClip m_OnDeathSound;
        
        // The sound thats played when the character respawns.
        [SerializeField] 
        private AudioClip m_OnRespawnSound;
        
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

        #endregion

        public void Die() {

            // The visual feedback played when dying.
            // Game.HitStop();
            // Game.Visuals.Particles.PlayEffect(m_ImpactEffect);
            m_ImpactEffect.Activate();
            // Game.Visuals.Particles.PauseEffect(m_TrailEffect);
            m_TrailEffect.Stop();
            Game.Audio.Sounds.PlaySound(m_OnDeathSound, 0.15f);

            // Noting the death in the stats.
            // Game.Level.AddDeath();
            Game.Score.AddDeath();
            
            // Resetting the character.
            DisableAllAbilityActions();
            m_Body.Stop();
            transform.position = m_RespawnBlock.RespawnPosiiton;

            // Resetting the level.
            Orb.ResetAll();
            Star.ResetAll();
            GhostBlock.ResetAll();
            Game.Visuals.Camera.Recolor(Game.Visuals.DefaultPalette);

            Disable(RespawnBlock.RESPAWN_DELAY);
            StartCoroutine(IERespawn(RespawnBlock.RESPAWN_DELAY));

        }

        private IEnumerator IERespawn(float delay) {
            yield return new WaitForSeconds(delay);
            m_DefaultAction.Enable();
            Game.Audio.Sounds.PlaySound(m_OnRespawnSound, 0.15f);
        }

        public void SetResetBlock(RespawnBlock block) {
            m_RespawnBlock = block;
            Star.CollectAllFollowing(transform);
        }

        public void Disable(float duration) {
            m_DisableTimer.Start(duration);
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
            Timer.TickDown(ref m_DisableTimer, Time.fixedDeltaTime);

            m_Rising = m_Body.Rising();
            m_FacingDirection = m_DirectionLocked ? m_FacingDirection : m_Input.Direction.Horizontal;
            m_OnGround = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.down, Game.Physics.CollisionLayers.Ground);
            m_FacingWall = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, m_FacingDirection,  Game.Physics.CollisionLayers.Ground);

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

        public void DisableAllAbilityActions() {
            m_Dash.Enable(this, false);
            m_Hop.Enable(this, false);
            m_Ghost.Enable(this, false);
            m_Shadow.Enable(this, false);
            m_Sticky.Enable(this, false);
        }

    }

}

