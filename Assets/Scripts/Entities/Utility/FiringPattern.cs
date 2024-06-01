// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Entities.Utility {

    [CreateAssetMenu(fileName="FiringPattern", menuName="FiringPattern")]
    public class FiringPattern : ScriptableObject {

        public Projectile projectile;
        public int projectileCount;

        public float speedScale = 1f;
        public AnimationCurve speedCurve;
        public float speed(float t) { return speedScale * speedCurve.Evaluate(t); }

        public float delayScale = 0f;
        public AnimationCurve delayCurve;
        public float delay(float t) { return delayScale * delayCurve.Evaluate(t); }
        
        public float angleScale = 360f;
        public AnimationCurve angleCurve;
        public float angle(float t) { return angleScale * angleCurve.Evaluate(t); }
        

        public void Fire(Transform origin) {
            GameObject newObject = new GameObject("firing pattern holder", typeof(TempFiringPatternObject));
            TempFiringPatternObject tmpObj = newObject.GetComponent<TempFiringPatternObject>();
            tmpObj.pattern = this;
            tmpObj.origin = origin;
            tmpObj.Execute();
        }

    }

    public class TempFiringPatternObject : MonoBehaviour {

        public FiringPattern pattern;
        public Transform origin;

        public Vector3 lastKnownPos;
        public Vector3 lastKnownRot;

        public void Execute() {
            lastKnownPos = origin.position;
            lastKnownRot = origin.right;
            StartCoroutine(IEExecute());
        }

        private IEnumerator IEExecute() {
            for (int i = 0; i < pattern.projectileCount; i++) {
                if (origin != null) { 
                    lastKnownPos = origin.position; 
                    lastKnownRot = origin.right;
                }

                Projectile instance = pattern.projectile.CreateInstance(lastKnownPos);

                float t = (float)i / (float)pattern.projectileCount;
                instance.Fire(pattern.speed(t), Quaternion.Euler(0f, 0f, pattern.angle(t)) * lastKnownRot);

                if (pattern.delay(t)>0f) {
                    yield return new WaitForSeconds(pattern.delay(t));
                }

            }

            Destroy(gameObject);

        }
        
    }

}
