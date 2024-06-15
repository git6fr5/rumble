// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Gobblefish {

    public class MovementAnimator : MonoBehaviour {

        public const float FACE_ANIMATION_SCALE_DURATION = 2f;

        public const float FULL_CALCULATION_DEPTH = 100f;
        public const float STEP_CALCULATION_DEPTH = 100f;

        // The distance that needs to be covered over this movement.
        [SerializeField]
        private AnimationCurve m_MovementCurve;

        [SerializeField, ReadOnly]
        private float m_DistanceFactor = 0f;

        public static MovementAnimator New(GameObject gameObject, AnimationCurve curve, float distance) {
            MovementAnimator newMovement = gameObject.AddComponent<MovementAnimator>();
            newMovement.Set(curve, distance);
            return newMovement;
        }

        public void Set(AnimationCurve curve, float distance) {
            m_MovementCurve = curve;
            m_DistanceFactor = NormalizedAreaFactor(m_MovementCurve, 0f, 1f, distance);
        }

        // Runs once every fixed interval.
        public float GetStepDistance(float t, float dt) {
            return IntegrationStep(m_MovementCurve, t, dt, m_DistanceFactor);
        }

        // Reset the movement parameters.
        public void RecalculateDistance(Vector3 origin, Vector3 dest) {
            m_DistanceFactor = NormalizedAreaFactor(m_MovementCurve, 0f, 1f, (origin-dest).magnitude);
        }

        //
        public static float NormalizedAreaFactor(AnimationCurve animCurve, float t = 0, float dt = 1f, float m = 1f) {
            return m / EulerIntegration(animCurve, t, t+dt, dt / FULL_CALCULATION_DEPTH);
        }

        //
        public static float IntegrationStep(AnimationCurve f, float t, float dt, float m = 1f) {
            return m * EulerIntegration(f, t, t+dt, dt / STEP_CALCULATION_DEPTH);
        }

        public static float EulerIntegration(AnimationCurve f, float lB = 0, float rB = 1, float w = 0.01f) {

            // Cache these values for memory efficiency.
            float A = 0f;
            float yL = 0f;
            float yR = 0f;
            float yAve = 0f;

            // Step through and add the box areas. 
            for (float x = lB; x < rB-w; x+=w) {
                
                // Get the values at either end.
                yL = f.Evaluate(x);
                yR = f.Evaluate(x+w);

                // Get the average value.
                yAve = ((yL+yR) / 2f);

                // Add the box area.
                A += yAve * w;

            }

            // Return the total evaluated area.
            return A;

        }

    }

}