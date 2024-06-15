// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
using Gobblefish.Animation;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    using Entities.Utility;

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [CreateAssetMenu(fileName="ChargedProjectileAction", menuName ="Actions/ChargedProjectile")]
    public class ChargedProjectileAction : ChargeAction {

        [SerializeField]
        private Projectile projectile;

        // The direction the player was facing before the dash started.
        [SerializeField]
        private Vector2 direction = new Vector2(0f, 0f);

        // The speed of the actual dash.
        [SerializeField]
        protected float speed = 5f;
        
        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            
            if (m_ActionTimer.Active) {
                OnStartPostaction(character);
                OnEndAction(character);
            }
            else {
                m_ActionPhase = ActionPhase.None;
            }

        }

        protected override void OnStartAction(CharacterController character) {
            base.OnStartAction(character);
            
            character.Animator.PlayAnimation("OnStartFire");
            m_ActionTimer.Start(0.02f);

            if (tmp != null) {
                tmp.Fire(speed * ChargeValue, direction);
            }
            
        }

        protected override void OnStartPostaction(CharacterController character) {
            base.OnStartPostaction(character);

            m_ActionTimer.Start(0.02f);
            character.Animator.PlayAnimation("OnStartPostfire");
            character.Default.Enable(character, true);
            m_Refreshed = true;
            tmp = null;

        }

        protected override void OnEndAction(CharacterController character) {
            base.OnEndAction(character);
            character.Animator.StopAnimation("OnStartFire");
            character.Animator.StopAnimation("OnStartPostfire");
        }

        Projectile tmp;
        public AnimationCurve growthCurve;

        // Start the character charging.
        protected override void OnStartPreaction(CharacterController character) {
            base.OnStartPreaction(character);
            tmp = projectile.CreateInstance(character.transform.position + 3f * Vector3.down);
            tmp.transform.localScale = Vector3.zero;
        }

        protected override void WhilePreaction(CharacterController character, float dt) {
            base.WhilePreaction(character, dt);

            if (tmp != null) {
                tmp.transform.localScale = growthCurve.Evaluate(ChargeValue) * projectile.transform.localScale;
                tmp.PointInDirection(Vector3.down);
            }

        }

    }
}