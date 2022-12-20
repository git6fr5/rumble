/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Visuals {

    ///<summary>
    /// The settings for using the visuals in the game.
    ///<summary>
    public static class VisualSettings {

        // The amount of pixels per unit, essentially, the quality.
        private static int m_PixelsPerUnit = 64;
        public static int PixelsPerUnit => m_PixelsPerUnit;

        // The amount of time it takes the camera to snap between sections of a run.
        private static int m_CameraSnapTime = 0.33f;

        // Whether the camera movement between sections should be smooth.
        private static int m_SmoothCameraMovement = true;

        // The actual snap time.
        public static float CameraSnapTime = m_SmoothCameraMovement ? m_CameraSnapTime : Time.deltaTime;

        // The amount of camera shake.
        private static float m_CameraShakeStrength = 1f;

        // Whether camera shake is enabled.
        private static bool m_CameraShakeDisabled = false;

        // The actual camera shake value.
        public static float CameraShakeStrength => m_CameraShakeDisabled ? 0f : m_CameraShakeStrength;

        // The target frame rate this game runs at.
        private static float m_FrameRate = 60f;

        // Whether to limit the frame rate or let it run at max.
        private static bool m_LimitFrameRate = false;

        // The actual frame rate the game should run at.
        public static float FrameRate => m_LimitFrameRate ? -1f : m_FrameRate;


    }

}