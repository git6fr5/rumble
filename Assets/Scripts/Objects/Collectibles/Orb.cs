/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Obstacles;
using Platformer.Decor; // For the particles.

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Timer = Platformer.Utilities.Timer;
using CharacterController = Platformer.Character.CharacterController;
using ColorPalette = Platformer.Visuals.Rendering.ColorPalette;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(CircleCollider2D))]
    public class Orb : MonoBehaviour {

        #region Enumerations.
        
        public enum Type {
            None, 
            Dash, 
            Hop, 
            Ghost, 
            Shadow, 
            Sticky
        }

        #endregion

        #region Variables.

        /* --- Constants --- */

        // The amount of time before the orb naturally respawns.
        private const float RESET_DELAY = 1.5f;

        // The amount of time before it starts blinking.
        private const float RESET_BLINK_DELAY = 0.7f;

        // The amount of times the orb blinks before it reappears.
        private const float RESET_BLINK_COUNT = 3;

        // The opacity of the orb when it first blinks back into screen.
        private const float RESET_BASE_OPACITY = 0.2f;

        // The opacity increase of the orb per blink.
        private const float RESET_OPACITY_PER_BLINK = 0.075f;

        // The period with which this bobs.
        private const float PERIOD = 3f;

        // The ellipse with which an orb moves.
        private static Vector2 ELLIPSE = new Vector2(0f, 2f/16f);

        /* --- Components --- */

        // The sprite renderer attached to this gameObject.
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();

        // The collider attached to this gameObject
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();

        /* --- Members --- */

        // Used to cache the origin of what this orb is centered around.
        [SerializeField, ReadOnly] 
        private Vector3 m_Origin;

        // Tracks the position of the orb in its floating cycle.
        [HideInInspector] 
        private float m_FloatTimer = new Timer(PERIOD, PERIOD);

        // The type of orb this is.
        [SerializeField] 
        protected Type m_Type;

        // The palette for this particular orb.
        [SerializeField] 
        protected ColorPalette m_Palette;
        public ColorPalette Palette => m_Palette;

        // The effect that plays when this orb is collected
        [SerializeField] 
        private VisualEffect m_CollectEffect;
        
        // The sound that plays when this orb is collected.
        [SerializeField] 
        private AudioClip m_CollectSound;
        
        // The effect that when this orb is reset.
        [SerializeField] 
        private VisualEffect m_RefreshEffect;
        
        // The sound that plays when this orb is reset.
        [SerializeField] 
        private AudioClip m_RefreshSound;

        // The sound that plays when this orb blinks.
        [SerializeField]
        private AudioClip m_BlinkSound;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            m_Hitbox.isTrigger = true;
            m_Origin = transform.position;
            m_Palette.SetSimple(m_SpriteRenderer.material);
            m_FloatTimer.Start(PERIOD);
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_FloatTimer.Cycle(Time.fixedDeltaTime);
            transform.Cycle(m_FloatTimer.Value, m_FloatTimer.MaxValue, m_Origin, ELLIPSE);
        }

        // Runs everytime something enters this trigger area.
        void OnTriggerEnter2D(Collider2D collider) {
            CharacterController controller = collider.GetComponent<CharacterController>();
            if (controller != null) {
                Collect(controller);
            }
        }

        // Collects this orb.
        void Collect(CharacterController controller) {

            // Swap the power based on the type of orb.
            controller.DisableAllAbilityActions();
            switch (m_Type) {
                case Type.Dash:
                    controller.Dash.Enable(controller, true);
                    break;
                case Type.Hop:
                    controller.Hop.Enable(controller, true);
                    break;
                case Type.Ghost:
                    controller.Ghost.Enable(controller, true);
                    break;
                case Type.Shadow:
                    controller.Shadow.Enable(controller, true);
                    break;
                case Type.Sticky:
                    controller.Sticky.Enable(controller, true);
                    break;
                default:
                    controller.Default.Enable(controller, true);
                    break;
            }

            // The feedback on collecting something.
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sound.PlaySound(m_CollectSound, 0.05f);
            Game.Visuals.Particles.PlayEffect(m_CollectEffect);
            Game.Visuals.Particles.PlayEffect(controller.ImpactEffect);
            Game.Visuals.Camera.Recolor(m_Palette);
            
            // Disable the orb for a bit.
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
            StartCoroutine(IEReset());

        }

        // Reset after a delay.
        IEnumerator IEReset() {
            yield return new WaitForSeconds(RESET_BLINK_DELAY);

            // Set up the colors.
            Color cacheColor = m_SpriteRenderer.color;
            Color tempColor = m_SpriteRenderer.color;
            tempColor.a = RESET_BASE_OPACITY;
            
            // Blink the orb a couple of times.
            m_SpriteRenderer.color = tempColor; 
            for (int i = 0; i < 2 * RESET_BLINK_COUNT; i++) {
                tempColor.a += RESET_OPACITY_PER_BLINK;
                m_SpriteRenderer.color = tempColor; 
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                Game.Audio.Sound.PlaySound(m_BlinkSound, 0.05f);
                yield return new WaitForSeconds((RESET_DELAY - RESET_BLINK_DELAY) / (float)(2 * RESET_BLINK_COUNT));
            }
            
            // Reset the orbs color.
            m_SpriteRenderer.color = cacheColor;
            m_SpriteRenderer.enabled = true;

            // Wait one more blink just because it feels more correct.
            yield return new WaitForSeconds((RESET_DELAY - RESET_BLINK_DELAY) / (float)RESET_BLINK_COUNT);

            // Make the orb collectible agains.
            Game.Visuals.Particles.PlayEffect(m_RefreshEffect);
            Game.Audio.Sound.PlaySound(m_RefreshSound, 0.15f);
            m_Hitbox.enabled = true;
            m_SpriteRenderer.enabled = true;

            yield return null;
        
        }
        
        // Resets the orb.
        public void Reset() {
            m_Hitbox.enabled = false;
            m_SpriteRenderer.enabled = false;
            StartCoroutine(IEReset()); 
        }

        // Reset all the orbs in the scene.
        public static void ResetAll() {
            Orb[] orbs = (Orb[])GameObject.FindObjectsOfType(typeof(Orb));
            for (int i = 0; i < orbs.Length; i++) {
                orbs[i].Reset();
            }
        }

        #endregion
        
    }
}