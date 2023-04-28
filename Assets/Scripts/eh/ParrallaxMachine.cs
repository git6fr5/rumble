/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityExtensions;
// Platformer.
using Platformer.Objects.Platforms;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Objects.Decorations {

    /// <summary>
    /// It does two things.
    /// Imagine there is a semi circle of these statues around the camera watching you
    /// At the corners of the level, the
    /// <summary>
    public class ParrallaxMachine : MonoBehaviour {

        public Transform[] transforms;

        public float[] depths;

        public Vector2[] positions;

        public float scale;

        void Start() {

            // depths = new float[transforms.Length];
            // positions = new Vector2[positions.Length];

            // for (int i = 0; i < transforms.Length; i++) {
            //     depths[i] = transforms[i].position.z;
            //     positions[i] = (Vector2)transforms.position;
            // }

        }

        void FixedUpdate() {

            // transform.position =

        }


    }

}
