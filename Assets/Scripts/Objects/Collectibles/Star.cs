// TODO: Clean

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
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Star : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before this star returns to its original position.
        private const float RESET_DELAY = 1.15f;

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
        
        [SerializeField] 
        private Vector2 m_Ellipse = new Vector2(0f, 2f/16f);

        [SerializeField] 
        private VisualEffect m_CollectEffect;
        
        [SerializeField] 
        private AudioClip m_CollectSound;
        
        [SerializeField] 
        private VisualEffect m_RefreshEffect;
        
        [SerializeField] 
        private AudioClip m_RefreshSound;

        [SerializeField] 
        private Transform m_Follow = null;
        public Transform Following => m_Follow;

        #endregion

        #region Methods.

        // Runs once before the first frame.
        void Start() {
            m_Hitbox.isTrigger = true;
            m_Origin = transform.position;
            m_FloatTimer.Start(PERIOD);
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            if (m_Follow == null) {
                m_FloatTimer.Cycle(Time.fixedDeltaTime);
                Obstacle.Cycle(transform, m_FloatTimer.Value, m_FloatTimer.MaxValue, m_Origin, ELLIPSE);
            }
            else {
                float mag = (m_Follow.position - transform.position).magnitude;
                Obstacle.Move(transform, m_Follow.position, mag * 5f, Time.fixedDeltaTime);
            }
        }

        // Runs everytime something enters this trigger area.
        void OnTriggerEnter2D(Collider2D collider) {
            CharacterController controller = collider.GetComponent<CharacterController>();
            if (controller != null) {
                Follow(controller);
            }
        }

        // Sets this star to follow a controller.
        public void Follow(CharacterController controller) {
            // Follows the transform.
            m_Follow = controller.transform;
            m_Hitbox.enabled = false;

            // The feedback on touching a star.
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sound.PlaySound(m_TouchSound, 0.05f);
            Game.Visuals.Particles.PlayEffect(m_TouchEffect);
            Game.Visuals.Particles.PlayEffect(controller.ImpactEffect);
        }

        // Collects this orb.
        public void Collect() {
            // The feedback on collecting a star.
            Game.Physics.Time.RunHitStop(8);
            Game.Audio.Sound.PlaySound(m_CollectSound, 0.05f);
            Game.Visuals.Particles.PlayEffect(m_CollectEffect);

            // Destroy the star once its been collected.
            Game.Score.AddStar(this);
            Destroy(gameObject);
        }

        public void Reset() {
            GameObject tempObject = new GameObject("Temporary Star Node");
            tempObject.transform.position = m_Origin;
            m_Follow = tempObject.transform;
            StartCoroutine(IEReset());
        }

        // Reset after a delay.
        IEnumerator IEReset() {
            // Inside the
            yield return new WaitForSeconds(RESET_BLINK_DELAY);

            // Make the orb collectible agains.
            Game.Visuals.Particles.PlayEffect(m_RefreshEffect);
            Game.Audio.Sound.PlaySound(m_RefreshSound, 0.15f);
            m_Hitbox.enabled = true;

            yield return null;
        
        }

        // Collects all stars that are currently following the given transform.
        public static void CollectAllFollowing(Transform transform) {
            Star[] stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
            for (int i = 0; i < stars.Length; i++) {
                if (stars[i].Following == transform) {
                    stars[i].Collect();
                }
            }
        }

        // Resets all the stars in the scene.
        public static void ResetAll() {
            Star[] stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
            for (int i = 0; i < stars.Length; i++) {
                stars[i].Reset();
            }
        }
        
        #endregion
        
    }
}