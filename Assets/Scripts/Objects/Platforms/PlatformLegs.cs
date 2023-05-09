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

        public const float INSET = 0.5f;

        [SerializeField]
        private Leg m_BaseLeg;

        [SerializeField, ReadOnly]
        private List<Leg> m_Legs = new List<Leg>();

        //
        [SerializeField]
        private AudioClip m_MovingRightAnimation = null;

        //
        [SerializeField]
        private AudioClip m_MovingLeftAnimation = null;

        //
        [SerializeField]
        public TransformAnimation m_IdleAnimation;

        #endregion

        void FixedUpdate() {
            if (m_BlendValue < 1f) {
                Blend(Time.fixedDeltaTime);
            }
            else {
                Animate(Time.fixedDeltaTime);
            }
        }

        void Blend(float dt) {
            m_BlendValue += dt;

            foreach (Leg leg in legs) {
                for (int i = 0; i < leg.AnimatedPieces.Length; i++) {
                    leg.AnimatedPieces[i].transform.Blend(leg.AnimatedPieces[i].Animation, m_TargetAnimation, leg.AnimatedPieces[i].AnimationTimer.Value, m_BlendValue);
                }
            } 

            float timeCache = 0f;
            if (m_BlendValue >= 1f) {
                foreach (Leg leg in m_Legs) {
                    leg.enabled = true;

                    for (int i = 0; i < leg.AnimatedPieces.Length; i++) {
                        timeCache = leg.AnimatedPieces[i].Animation.AnimationTimer.Value; 
                        leg.AnimatedPieces[i].Animation = m_TargetAnimation;
                        leg.AnimatedPieces[i].Animation.AnimationTimer.Set(timeCache);
                    }
                }

            }
            
        }

        public void CreateLegs(int gridLength, float actualLength) {
            m_Legs = new List<Leg>();
            
            // Special case when the platform is too short for proper construction.
            if (gridLength <= 2) {
                m_BaseLeg.transform.localPosition += (gridLength - 1f) * new Vector3(0.5f, 0.2f, 0f);
                m_BaseLeg.gameObject.SetActive(true);
                m_Legs.Add(m_BaseLeg);
            }
            else {

                float distanceTraversed = SPACING;
                float totalDistance = actualLength - INSET - SPACING;
            
                while (distanceTraversed < totalDistance) {
                    // Instantiate and edit the new legs.
                    GameObject newLeg = Instantiate(m_BaseLeg.gameObject);
                    newLeg.transform.SetParent(transform);
                    newLeg.transform.localPosition = Vector3.right * distanceTraversed + Vector3.up * m_BaseLeg.transform.localPosition.y;
                    newLeg.SetActive(true);
                    // Add the leg to the list of legs.
                    m_Legs.Add(newLeg.GetComponent<Decorations.DecorationController>());
                    // Increment the distance.
                    distanceTraversed += SPACING;
                }

            }
            
        }

        public void SetLegAnimation(TransformAnimation animation) {
            TransformAnimation targetAnimation = animation;
            m_BlendValue = 0f;
            
            foreach (Leg leg in m_Legs) {
                leg.enabled = false;
            }
            
        }

        public void SetLegAnimation() {
            Vector3 direction = m_Path[m_PathIndex] - transform.position;
            float rotationScale = direction.x > 0 ? 1f : -1f;
            float scaleScale = -1f * rotationScale;
            float posScale = rotationScale;

            for (int n = 0; n < m_Legs.Count; n++) {

                Decorations.DecorationController leg = m_Legs[n];

                leg.enabled = true;
                for (int i = 0; i < leg.AnimatedPieces.Length; i++) {
                    
                    leg.AnimatedPieces[i].Animation.RotationScale = Mathf.Abs(leg.AnimatedPieces[i].Animation.RotationScale) * rotationScale;
                    leg.AnimatedPieces[i].Animation.ScaleScale = Mathf.Abs(leg.AnimatedPieces[i].Animation.ScaleScale) * scaleScale;
                    
                    float _posScale = i % 2 == 0 ? posScale : -posScale; 
                    leg.AnimatedPieces[i].Animation.PositionScale = Mathf.Abs(leg.AnimatedPieces[i].Animation.PositionScale) * _posScale;
                    
                    float maxVal = leg.AnimatedPieces[i].Animation.AnimationTimer.MaxValue;
                    
                    float incMax = 0.5f * maxVal; // 2f / 3f;
                    float baseVal = i % 2 == 0 ? maxVal / 2f : 0f;
                    float incVal = ((float)n / m_Legs.Count) * incMax;
                    if (direction.x > 0f) {

                        baseVal = maxVal / 2f - baseVal;
                        incVal = incMax - incVal;
                    }
                    
                    leg.AnimatedPieces[i].Animation.AnimationTimer.Set(baseVal + incVal);
                }   
            }
        }
        
    }

    

}
