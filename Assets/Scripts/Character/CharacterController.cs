/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
// Platformer.
using Platformer.Input;
using Platformer.Character;
using Platformer.Character.Actions;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using SaveSystem = Platformer.Management.SaveSystem;
using RespawnBlock = Platformer.Objects.Blocks.RespawnBlock;
using ScoreOrb = Platformer.Objects.Orbs.ScoreOrb;

namespace Platformer.Character {

    ///<summary>
    /// Controls a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterController : MonoBehaviour {

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

        // The component used to animate this character.
        [SerializeField]
        private CharacterAnimator m_Animator;
        public CharacterAnimator Animator => m_Animator;

        /* --- Members --- */

        // Checks whether the character is on the ground.
        [SerializeField, ReadOnly] 
        private bool m_OnGround = false;
        public bool OnGround => m_OnGround;
        
        // Checks whether the character is facing a wall.
        [SerializeField, ReadOnly] 
        private bool m_FacingWall = false;
        public bool FacingWall => m_FacingWall;
        
        // Checks what direction the controller is facing.
        [SerializeField, ReadOnly] 
        private float m_FacingDirection = 1f;
        public float FacingDirection => m_FacingDirection;

        // Whether the direction that this is facing is locked.
        [SerializeField, ReadOnly]
        private bool m_DirectionLocked = false;

        // Checks whether the character is rising.
        [SerializeField, ReadOnly] 
        private bool m_Rising = false;
        public bool Rising => m_Rising;

        // Checks whether the character is falling.
        [SerializeField, ReadOnly] 
        private bool m_Falling = false;
        public bool Falling => m_Falling;

        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly] 
        private Timer m_DisableTimer = new Timer(0f, 0f);
        public bool Disabled => m_DisableTimer.Active;

        // The block that this character respawns at.
        [SerializeField, ReadOnly] 
        private RespawnBlock m_RespawnBlock;
        public RespawnBlock RespawnBlock => m_RespawnBlock;

        // The effect thats played to show an impact for this character.
        [SerializeField] 
        private ParticleSystem m_ImpactEffect;
        
        // The effect thats played to show this characters movement.
        [SerializeField] 
        private VisualEffect m_TrailEffect;
        
        // The sound thats played when the character dies.
        [SerializeField] 
        private AudioClip m_OnDeathSound;
        
        // The sound thats played when the character respawns.
        [SerializeField] 
        private AudioClip m_OnRespawnSound;
        
        // Actions.
        [SerializeField] 
        private DefaultAction m_DefaultAction;
        public DefaultAction Default => m_DefaultAction;

        [SerializeField] 
        private DashAction m_DashAction;
        public DashAction Dash => m_DashAction;
        
        [SerializeField] 
        private HopAction m_HopAction;
        public HopAction Hop => m_HopAction;
        
        [SerializeField] 
        private GhostAction m_GhostAction;
        public GhostAction Ghost => m_GhostAction;
        
        [SerializeField] 
        private ShadowAction m_ShadowAcction;
        public ShadowAction Shadow => m_ShadowAcction;
        
        [SerializeField] 
        private StickyAction m_StickyAction;
        public StickyAction Sticky => m_StickyAction;

        #endregion

        void Start() {
            m_DefaultAction.Enable(this, true);
            EnableAllAbilityActions();
            DisableAllAbilityActions();
        }

        public void Reset() {

            // The visual feedback played when dying.
            Game.Physics.Time.RunHitStop(16);
            Game.Visuals.Particles.PlayEffect(m_ImpactEffect);
            Game.Visuals.Particles.PauseEffect(m_TrailEffect);
            Game.Audio.Sounds.PlaySound(m_OnDeathSound, 0.15f);

            // Noting the death in the stats.
            Game.Level.AddDeath();
            Game.Level.Reset();
            
            // Resetting the character.
            Disable(RespawnBlock.RESPAWN_DELAY);
            DisableAllAbilityActions();
            m_Body.Stop();
            transform.position = m_RespawnBlock.RespawnPosition;
            StartCoroutine(IERespawn(RespawnBlock.RESPAWN_DELAY));

        }

        private IEnumerator IERespawn(float delay) {
            yield return new WaitForSeconds(delay);
            m_DefaultAction.Enable(this, true);
            Game.Audio.Sounds.PlaySound(m_OnRespawnSound, 0.15f);
        }

        public void SetResetBlock(RespawnBlock block) {
            m_RespawnBlock = block;
            ScoreOrb.CollectAllFollowing(transform);
        }

        public void Disable(float duration) {
            m_DisableTimer.Start(duration);
        }

        void Update() {
            if (m_DisableTimer.Active) { return; }

            m_DefaultAction.InputUpdate(this);
            m_DashAction.InputUpdate(this);
            m_HopAction.InputUpdate(this);
            m_GhostAction.InputUpdate(this);
            m_ShadowAcction.InputUpdate(this);
            m_StickyAction.InputUpdate(this);
        }

        void FixedUpdate() {
            m_DisableTimer.TickDown(Time.fixedDeltaTime);

            m_Rising = m_Body.Rising();
            m_Falling = m_Body.Falling();
            // m_DirectionLocked = m_DisableTimer.Active;
            m_FacingDirection = m_DirectionLocked ? m_FacingDirection : m_Input.Direction.Horizontal != 0f ? m_Input.Direction.Horizontal : m_FacingDirection;
            m_OnGround = Game.Physics.Collisions.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.down, Game.Physics.CollisionLayers.Ground);
            m_FacingWall = Game.Physics.Collisions.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.right * m_FacingDirection,  Game.Physics.CollisionLayers.Ground);

            m_DefaultAction.PhysicsUpdate(this, Time.fixedDeltaTime);
            m_DashAction.PhysicsUpdate(this, Time.fixedDeltaTime);
            m_HopAction.PhysicsUpdate(this, Time.fixedDeltaTime);
            m_GhostAction.PhysicsUpdate(this, Time.fixedDeltaTime);
            m_ShadowAcction.PhysicsUpdate(this, Time.fixedDeltaTime);
            m_StickyAction.PhysicsUpdate(this, Time.fixedDeltaTime);
        }

        public void EnableAllAbilityActions() {
            m_DashAction.Enable(this, true);
            m_HopAction.Enable(this, true);
            m_GhostAction.Enable(this, true);
            m_ShadowAcction.Enable(this, true);
            m_StickyAction.Enable(this, true);
        }

        public void DisableAllAbilityActions() {
            m_DashAction.Enable(this, false);
            m_HopAction.Enable(this, false);
            m_GhostAction.Enable(this, false);
            m_ShadowAcction.Enable(this, false);
            m_StickyAction.Enable(this, false);
        }

    }

}

