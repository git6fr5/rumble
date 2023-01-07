/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Visuals {

    ///<summary>
    ///
    ///<summary>
    public static class ParticleSystem {

        // Spawn the particle.
        public static void Spawn(this List<SpriteRenderer> particles, string name, Transform parent, Sprite particleSprite, float spawnRadius, int minSortingOrder, int maxSortingOrder, float minScale, float maxScale) {
            string particleName = name + " Particle " + particles.Count.ToString();
            SpriteRenderer spriteRenderer = new GameObject(particleName, typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = particleSprite;
            spriteRenderer.transform.position = parent.position + (Vector3)Random.insideUnitCircle * spawnRadius;
            spriteRenderer.sortingOrder = Random.Range(minSortingOrder, maxSortingOrder);
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.transform.localScale *= Random.Range(minScale, maxScale);
            particles.Add(spriteRenderer);
        }

        // Resets the particles.
        public static void Reset(this List<SpriteRenderer> particles) {
            particles.RemoveAll(particle => particle == null);
            for (int i = 0; i < particles.Count; i++) {
                UnityEngine.MonoBehaviour.Destroy(particles[i].gameObject);
            }
            particles = new List<SpriteRenderer>();

        }

        // Fades the particle out.
        public static void FadeOut(this List<SpriteRenderer> particles, float fadeSpeed, float fadeThreshold, float dt) {
            particles.RemoveAll(particle => particle == null);
            for (int i = 0; i < particles.Count; i++) {
                Color cacheColor = particles[i].color;
                cacheColor.a = cacheColor.a - fadeSpeed * dt;
                if (cacheColor.a < fadeThreshold) {
                    UnityEngine.MonoBehaviour.Destroy(particles[i].gameObject);
                }
                else {
                    particles[i].color = cacheColor;
                }
            }
        }

        // Rotates the particle.
        public static void Rotate(this List<SpriteRenderer> particles, float rotationSpeed, float dt) {
            particles.RemoveAll(particle => particle == null);
            Vector3 deltaAngles = Vector3.forward * rotationSpeed * dt;
            for (int i = 0; i < particles.Count; i++) {
                particles[i].transform.eulerAngles += deltaAngles;
            }
        }

        // Moves the particle in a specific direction.
        public static void Move(this List<SpriteRenderer> particles, Vector3 direction, float speed, float dt) {
            particles.RemoveAll(particle => particle == null);
            Vector3 dx = direction * speed * dt;
            for (int i = 0; i < particles.Count; i++) {
                particles[i].transform.position += dx;
            }
        }

        public static void RadialMove(this List<SpriteRenderer> particles, Vector3 origin, float speed, float dt) {
            for (int i = 0; i < particles.Count; i++) {
                Vector3 dx = (particles[i].transform.position - origin).normalized * speed * dt;
                particles[i].transform.position += dx;
            }
        }

        // Moves according to opacity on 1 axis and with regular movement on the other axis.
        public static void OpacityMove(this List<SpriteRenderer> particles, Vector3 opacityDir, float opacitySpeed, Vector3 direction, float speed, float dt) {
            particles.RemoveAll(particle => particle == null);
            Vector3 dx_0 = direction * speed * dt;
            for (int i = 0; i < particles.Count; i++) {
                Vector3 dx_1 = opacityDir * opacitySpeed * particles[i].GetComponent<SpriteRenderer>().color.a * dt;
                particles[i].transform.position += (dx_0 + dx_1);
            }
        }

        // Moves according to opacity in 2 different directions, with 2 different speeds.
        public static void OpacityMove2D(this List<SpriteRenderer> particles, Vector3 opacityDirA, float opacitySpeedA, Vector3 opacityDirB, float opacitySpeedB, float dt) {
            particles.RemoveAll(particle => particle == null);
            for (int i = 0; i < particles.Count; i++) {
                Vector3 dx_1 = opacityDirA * opacitySpeedA * particles[i].GetComponent<SpriteRenderer>().color.a * dt;
                Vector3 dx_0 = opacityDirB * opacitySpeedB * particles[i].GetComponent<SpriteRenderer>().color.a * dt;
                particles[i].transform.position += (dx_0 + dx_1);
            }
        }

    }
}
