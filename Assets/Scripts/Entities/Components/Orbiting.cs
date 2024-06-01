/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Platformer;

// Definitions.
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    public class Orbiting : MonoBehaviour {

        // public Transform m_Center;
        // public float m_OrbitRadius;

        public float m_TimeToCompleteARevolution;
        float dAngle = 0f;

        // float m_PauseDuration

        void Start() {

            // float circ = 2 * Mathf.PI * m_OrbitRadius;
            dAngle = 360f / m_TimeToCompleteARevolution; // m_OrbitRadius; 

        }

        void FixedUpdate() {

            float dt = Time.fixedDeltaTime;
            Quaternion rot = Quaternion.Euler(0f, 0f, dAngle * dt);
            // transform.position = m_Center.position + m_OrbitRadius * (rot * (transform.position - m_Center.position).normalized);

            transform.localRotation = rot * transform.localRotation;

        }
        
    }
    
}