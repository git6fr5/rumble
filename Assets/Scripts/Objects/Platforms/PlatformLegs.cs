/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using Leg = Platformer.Objects.Decorations.DecorationController;

namespace Platformer.Objects.Platforms {

    ///<summary>
    ///
    ///<summary>
    public class PlatformLegs : MonoBehaviour {

        #region Variables.

        public const float SPACING = 1f;

        public const float INSET = 0f;

        [SerializeField]
        private float m_BlendValue = 0f;

        [SerializeField]
        private float m_BlendFactor = 1f;

        [SerializeField]
        private Leg m_BaseLeg;

        [SerializeField, ReadOnly]
        private List<Leg> m_Legs = new List<Leg>();

        [SerializeField, ReadOnly]
        private float m_Direction = 1f;

        //
        [SerializeField]
        private TransformAnimation m_FrontLegAnimation = null;
        public TransformAnimation FrontLegAnim => m_FrontLegAnimation;

        //
        [SerializeField]
        private TransformAnimation m_BackLegAnimation = null;
        public TransformAnimation BackLegAnim => m_BackLegAnimation;

        //
        [SerializeField]
        private TransformAnimation m_IdleAnimation = null;
        public TransformAnimation IdleAnimation => m_IdleAnimation;

        // The animation being changed to.
        private TransformAnimation m_TargetFrontLegAnimation;
        private TransformAnimation m_TargetBackLegAnimation;

        #endregion

        void FixedUpdate() {
            if (m_BlendValue < 1f) {
                Blend(Time.fixedDeltaTime);
            }
        }

        void Blend(float dt) {
            m_BlendValue += m_BlendFactor * dt;

            float maxVal = 0f;
            float incMax = 0f;
            float baseVal = 0f;
            float incVal = 0f;

            for (int n = 0; n < m_Legs.Count; n++) {

                Leg leg = m_Legs[n];
                for (int i = 0; i < leg.AnimatedPieces.Length; i++) {

                    GetValues(leg, ref maxVal, ref baseVal, ref incMax, ref incVal, i, n);

                    if (i % 2 == 0) {
                        m_TargetFrontLegAnimation.AnimationTimer.Set(baseVal + incVal);
                        leg.AnimatedPieces[i].transform.Blend(leg.AnimatedPieces[i].Animation, m_TargetFrontLegAnimation, m_BlendValue);
                    }
                    else {
                        m_TargetBackLegAnimation.AnimationTimer.Set((maxVal - baseVal) + incVal);
                        leg.AnimatedPieces[i].transform.Blend(leg.AnimatedPieces[i].Animation, m_TargetBackLegAnimation, m_BlendValue);
                    }
                }

            }

            if (m_BlendValue >= 1f) {

                for (int n = 0; n < m_Legs.Count; n++) {
                    Leg leg = m_Legs[n];
                    leg.enabled = true;

                    for (int i = 0; i < leg.AnimatedPieces.Length; i++) {

                        GetValues(leg, ref maxVal, ref baseVal, ref incMax, ref incVal, i, n);

                        if (i % 2 == 0) {
                            leg.AnimatedPieces[i].Animation.Set(m_TargetFrontLegAnimation, baseVal + incVal, maxVal * 2f);
                        }
                        else {
                            leg.AnimatedPieces[i].Animation.Set(m_TargetBackLegAnimation, (maxVal - baseVal) + incVal, maxVal * 2f);
                        }
                    
                    }
                }

            }
            
        }

        public void CreateLegs(int gridLength, float actualLength) {
            m_Legs = new List<Leg>();
            
            // Special case when the platform is too short for proper construction.
            if (gridLength <= 1) {
                m_BaseLeg.transform.localPosition += (gridLength - 1f) * new Vector3(0.5f, 0f, 0f);
                m_BaseLeg.gameObject.SetActive(true);
                m_Legs.Add(m_BaseLeg);
            }
            else {

                float distanceTraversed = 0f;
                float totalDistance = actualLength - INSET - SPACING;

                float maxVal = 0f;
                float incMax = 0f;
                float baseVal = 0f;
                float incVal = 0f;
                int n = 0;

                while (distanceTraversed < totalDistance) {
                    // Instantiate and edit the new legs.
                    Leg newLeg = Instantiate(m_BaseLeg.gameObject).GetComponent<Leg>();
                    newLeg.transform.SetParent(transform);
                    newLeg.transform.localPosition = Vector3.right * distanceTraversed + Vector3.up * m_BaseLeg.transform.localPosition.y;
                    newLeg.gameObject.SetActive(true);


                    for (int i = 0; i < newLeg.AnimatedPieces.Length; i++) {
                        
                        GetValues(newLeg, ref maxVal, ref baseVal, ref incMax, ref incVal, i, n);
                        
                        if (i % 2 == 0) {
                            newLeg.AnimatedPieces[i].Animation.Set(m_FrontLegAnimation, baseVal + incVal, maxVal * 2f);
                        }
                        else {
                            newLeg.AnimatedPieces[i].Animation.Set(m_BackLegAnimation, (maxVal - baseVal) + incVal, maxVal * 2f);
                        }

                    }

                    // Add the leg to the list of legs.
                    m_Legs.Add(newLeg);

                    // Increment the distance.
                    distanceTraversed += SPACING;
                    n += 1;

                }

            }
            
        }

        public void SetLegAnimation(TransformAnimation frontLegAnimatoin, TransformAnimation backLegAnimation) {
            m_TargetFrontLegAnimation = frontLegAnimatoin;
            m_TargetBackLegAnimation = backLegAnimation;
            
            m_BlendValue = 0f;
            
            foreach (Leg leg in m_Legs) {
                leg.enabled = false;
            }
            
        }

        // Transform animation is a class and so is passed by referenced and therefore i can just do this.
        public void SetAnimationDirection(TransformAnimation animation, float direction, float invertPosition) {
            m_Direction = direction;

            float rotationScale = direction > 0 ? 1f : -1f;
            float scaleScale = -1f * rotationScale;
            float posScale = invertPosition * rotationScale;

            animation.RotationScale = Mathf.Abs(animation.RotationScale) * rotationScale;
            animation.ScaleScale = Mathf.Abs(animation.ScaleScale) * scaleScale;
            animation.PositionScale = Mathf.Abs(animation.PositionScale) * posScale;
                    
        }

        public void GetValues(Leg leg, ref float maxVal, ref float baseVal, ref float incMax, ref float incVal, int i, int n) {
            maxVal = leg.AnimatedPieces[i].Animation.AnimationTimer.MaxValue / 2f;
            if (maxVal != 0f) {
                incMax = maxVal; // 2f / 3f;
                baseVal = i % 2 == 0 ? maxVal : 0f;
                int count = m_Legs.Count > 2 ? m_Legs.Count - 1 : 2;
                incVal = ((((float)n / (count - 1))) % 1) * incMax;
                if (m_Direction < 0f) {
                    incVal = incMax - incVal;
                }
            }
        }
    }

    

}
