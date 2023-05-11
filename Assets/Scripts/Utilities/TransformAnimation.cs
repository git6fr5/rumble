/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtensions {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class TransformAnimation {

        #region Variables

        public Timer AnimationTimer;
        
        public bool Loop;

        public float PositionScale;

        public float ScaleScale;

        public float RotationScale;

        public float ScaleOffset;

        [SerializeField]
        private AnimationCurve m_PositionCurveX;
        public AnimationCurve PositionCurveX => m_PositionCurveX;

        [SerializeField]
        private AnimationCurve m_PositionCurveY;
        public AnimationCurve PositionCurveY => m_PositionCurveY;

        public Vector2 Position => PositionScale * new Vector2(m_PositionCurveX.Evaluate(AnimationTimer.Ratio), m_PositionCurveY.Evaluate(AnimationTimer.Ratio));

        [SerializeField]
        private AnimationCurve m_ScaleCurveX;
        public AnimationCurve ScaleCurveX => m_ScaleCurveX;

        [SerializeField]
        private AnimationCurve m_ScaleCurveY;
        public AnimationCurve ScaleCurveY => m_ScaleCurveY;

        public Vector2 Scale => new Vector2(ScaleOffset, ScaleOffset) + ScaleScale * new Vector2(m_ScaleCurveX.Evaluate(AnimationTimer.Ratio), m_ScaleCurveY.Evaluate(AnimationTimer.Ratio));

        [SerializeField]
        private AnimationCurve m_RotationCurve;
        public AnimationCurve RotationCurve => m_RotationCurve;

        public Quaternion Rotation => Quaternion.Euler(0f, 0f, RotationScale * m_RotationCurve.Evaluate(AnimationTimer.Ratio));

        public void Set(TransformAnimation animation, float t, float max) {
            m_PositionCurveX = animation.m_PositionCurveX;
            m_PositionCurveY = animation.PositionCurveY;
            m_ScaleCurveX = animation.ScaleCurveX;
            m_ScaleCurveY = animation.ScaleCurveY;
            m_RotationCurve = animation.RotationCurve;
            PositionScale = animation.PositionScale;
            ScaleScale = animation.ScaleScale;
            RotationScale = animation.RotationScale;
            ScaleOffset = animation.ScaleOffset;
            AnimationTimer.Set(t, true, max);
        }

        #endregion    



    }

}