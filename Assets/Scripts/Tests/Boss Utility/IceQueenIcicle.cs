/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Tests {

    using Entities.Utility;

    ///<summary>
    ///
    ///<summary>
    public class IceQueenIcicle : Projectile {

        // [HideInInspector] private Vector3 m_Origin;

        // // Construction.
        // [SerializeField] private AnimationCurve m_ConstructionCurve;
        // [SerializeField] private float m_ConstructionDuration;
        // [SerializeField, ReadOnly] private float m_ConstructionTicks;
        // [HideInInspector] private float ConstructionRatio => m_ConstructionCurve.Evaluate(m_ConstructionTicks / m_ConstructionDuration);
        // [HideInInspector] private bool Constructing => m_ConstructionTicks < m_ConstructionDuration;

        // #endregion

        // public virtual void Fire(float speed, Vector2 direction) {
        //     gameObject.SetActive(true);
        //     m_ChargeTimer.Start(m_ChargeDuration);
        // }

        // private void DelayedFire() {
        //     m_Body.Move(direction.normalized * 0.5f);
        //     m_Body.SetVelocity(speed * direction);
        // }

        // void Awake() {
        //     // Initialize the shield.
        //     m_Body.simulated = false;
        //     m_Origin = transform.position;
        //     transform.localScale = new Vector3(0f, 0f, 0f);
        //     m_ConstructionTicks = 0f;
        // }

        // void LateUpdate() {
        //     bool finished = m_ChargeTimer.TickDown(Time.deltaTime);
        //     transform.localScale = new Vector3(1f, 1f, 1f) * m_ChargeTimer.InverseRatio;

        //     if (finished) {
        //         DelayedFire();
        //     }
        // }

        // public void Shatter() {
        //     //
        // }

    }
}