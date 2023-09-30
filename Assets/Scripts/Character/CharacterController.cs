/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityExtensions;
using UnityEngine.SceneManagement;
// Platformer.
using Platformer.Input;
using Platformer.Character;
using Platformer.Character.Actions;
using Platformer.Entities.Components;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Character {

    ///<summary>
    /// Controls a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterController : MonoBehaviour {

        #region Variables.

        /* --- Components --- */

        // The input system attached to this body.
        private InputSystem m_Input = null;
        public InputSystem Input => m_Input;

        // The rigidbody attached to this controller.
        private Rigidbody2D m_Body = null;
        public Rigidbody2D Body => m_Body;
        
        // The main collider attached to this body.
        private CircleCollider2D m_Collider = null;
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
        public bool Rising => !m_OnGround && m_Rising;

        // Checks whether the character is falling.
        [SerializeField, ReadOnly] 
        private bool m_Falling = false;
        public bool Falling => !m_OnGround && m_Falling;

        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly] 
        private Timer m_DisableTimer = new Timer(0f, 0f);
        public bool Disabled => m_DisableTimer.Active;
        private bool m_Dying = false;

        // The block that this character respawns at.
        [SerializeField, ReadOnly] 
        private Respawn m_Respawn;
        public Respawn CurrentRespawn => m_Respawn;

        // The sprite that is used for the death impact effect.
        [SerializeField]
        private Sprite m_OnDeathParticle;

        // The sound thats played when the character dies.
        [SerializeField] 
        private AudioClip m_OnDeathSound;

        [SerializeField, ColorUsage(true, true)]
        private Color m_DeathColor;
        
        // The sound thats played when the character respawns.
        [SerializeField] 
        private AudioClip m_OnRespawnSound;

        // The sprite that is used for the action impact effect.
        [SerializeField]
        private Sprite m_OnActionParticle;
        public Sprite OnActionParticle => m_OnActionParticle;
        
        // Actions.
        [SerializeField] 
        private DefaultAction m_DefaultAction;
        public DefaultAction Default => m_DefaultAction;

        // Used for reference for the power actions.
        [HideInInspector]
        private List<CharacterAction> m_PowerActions = new List<CharacterAction>();

        // The dash action.
        [SerializeField] 
        private DashAction m_DashAction;
        public DashAction Dash => m_DashAction;
        
        // The hop action.
        [SerializeField] 
        private HopAction m_HopAction;
        public HopAction Hop => m_HopAction;
        
        // The ghost action.
        [SerializeField] 
        private GhostAction m_GhostAction;
        public GhostAction Ghost => m_GhostAction;
        
        // The shadow action.
        [SerializeField] 
        private ShadowAction m_ShadowAcction;
        public ShadowAction Shadow => m_ShadowAcction;
        
        // The sticky action.
        [SerializeField] 
        private StickyAction m_StickyAction;
        public StickyAction Sticky => m_StickyAction;

        #endregion

        void Awake() {
            m_Input = GetComponent<InputSystem>();
            m_Body = GetComponent<Rigidbody2D>();
            m_Collider = GetComponent<CircleCollider2D>();
        }

        void Start() {
            
            m_DefaultAction.Enable(this, true);
            m_PowerActions = new List<CharacterAction>() {
                m_DashAction,
                m_HopAction,
                m_GhostAction, 
                m_ShadowAcction,
                m_StickyAction
            };

            EnableAllAbilityActions();
            DisableAllAbilityActions();
        }

        public void Reset() {
            if (m_Respawn == null) {
                SceneManager.LoadScene("Game");
            }

            if (m_Dying) {
                return;
            }
            
            // m_Animator.ColoredBurst(m_DeathColor);
            // The visual feedback played when dying.
            Game.Physics.Time.RunHitStop(16);
            // Game.Visuals.Effects.PlayImpactEffect(m_OnDeathParticle,30, 5f, transform, Vector3.zero);
            Game.Audio.Sounds.PlaySound(m_OnDeathSound, 0.15f);

            // Noting the death in the stats.
            Game.Level.AddDeath();
            Game.Level.Reset();
            
            // Resetting the character.
            Disable(Respawn.RESPAWN_DELAY);
            DisableAllAbilityActions();
            m_Body.Stop();
            m_Dying = true;

            transform.position = m_Respawn.RespawnPosition;
            StartCoroutine(IERespawn(Respawn.RESPAWN_DELAY));

        }

        private IEnumerator IERespawn(float delay) {
            yield return new WaitForSeconds(delay);
            m_Dying = false;
            m_DefaultAction.Enable(this, true);
            Game.Audio.Sounds.PlaySound(m_OnRespawnSound, 0.15f);
        }

        public void SetRespawn(Respawn respawn) {
            // Game.Objects.Reset();
            print("respawning");
            if (m_Respawn != null) {
                m_Respawn.Deactivate();
            }
            m_Respawn = respawn;
            m_Respawn.Activate();
        }

        public void Disable(float duration) {
            m_DisableTimer.Start(duration);
        }

        public void LockDirection(bool lockDirection, float direction = 0f) {
            m_DirectionLocked = lockDirection;
            if (direction == 0f) {
                return;
            }
            m_FacingDirection = direction;
        }

        void Update() {
            if (m_DisableTimer.Active) { return; }
            
            m_DefaultAction.InputUpdate(this);
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].InputUpdate(this);
            }
        
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
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].PhysicsUpdate(this, Time.fixedDeltaTime);
            }

        }

        public void EnableAllAbilityActions() {
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].Enable(this, true);
            }
        }

        public void DisableAllAbilityActions() {
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].Enable(this, false);
            }
        }

    }

}

