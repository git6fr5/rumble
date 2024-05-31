// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEditor;
using LDtkUnity;

namespace Platformer.Tests {

    // [ExecuteInEditMode]
    public class SendFloatArray : MonoBehaviour {

        public Material material;

        public int count;
        public float rad;
        public bool send;

        public Vector2[] s;
        public float dist;
        public float var;
        public int lineSegments;
        public bool draw;

        public float _rScale;
        public float _xScale;

        public Vector2 shiftX;
        public Vector2 shiftY;

        private SpriteRenderer spriteRenderer;

        void Update() {
            if (draw) {
                DrawLine();
                draw = false;
            }
            if (send) {
                DrawLine();
                Send();
                send = false;
            }
        }

        void DrawLine() {

            Vector2 dir = Vector2.right;

            s = new Vector2[lineSegments]; 
            s[0] = -dist / 2f * dir + (Vector2)transform.localPosition;
            for (int i = 1; i < lineSegments; i++) {
                // s[i] += s[i-1] + dist * dir / (float)lineSegments;
                s[i] += s[0] + i * dist * dir / (float)lineSegments;
                if (i != lineSegments) {
                    s[i] += (Vector2)(Quaternion.Euler(0f, 0f, 90f) * dir) * Random.Range(-var, var);
                }
            }

        }

        void OnDrawGizmos() {
            if (s == null) { return; }

            for (int i = 1; i < s.Length; i++) {
                Gizmos.DrawLine(s[i], s[i-1]);
            }

        }


        void Send() {

            count = Random.Range(3, 6);

            float[] pX = new float[count];
            float[] pY = new float[count];
            float[] rScale = new float[count];
            float[] xScale = new float[count];

            float[] rot = new float[count];

            for (int i = 0; i < count; i++) {
                
                // Vector2 p = Random.insideUnitCircle * rad;
                // pX[i] = p.x;
                // pY[i] = p.y;

                float r = (float)i/(float)count;
                r += Random.Range(shiftX.x, shiftX.y);
                if (r >= 1) {
                    r = 0.99f;
                }
                else if (r < 0f) {
                    r = 0.01f;
                }

                Vector2 p = GetPointOnS(r);
                pX[i] = p.x;
                pY[i] = p.y + Random.Range(shiftY.x, shiftY.y);

                rScale[i] = Random.Range(0.9f, 1.1f) * _rScale;
                xScale[i] = Random.Range(1.5f, 2f) * _xScale;

                rot[i] = Random.Range(-15f, 15f) + (r - 0.5f) * 2f * 30f;

            }

            material.SetInt("_pCount", count);
            material.SetFloatArray("pX", pX);
            material.SetFloatArray("pY", pY);
            material.SetFloatArray("_rScale", rScale);
            material.SetFloatArray("_xScale", xScale);
            material.SetFloatArray("_rotArr", rot);

            float[] sY = new float[lineSegments];

            for (int i = 0; i < lineSegments; i++) {
                sY[i] = s[i].y;
            }

            material.SetFloatArray("sY", sY);
            material.SetFloat("dX", dist / (float)lineSegments);
            material.SetFloat("sX0", s[0].x);
            material.SetFloat("_sCount", lineSegments);

            if (Application.isPlaying) {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.material = material;
                spriteRenderer.material.SetFloat("_sCount", lineSegments);
            }

        }

        Vector2 GetPointOnS(float r) {
            
            float rNorm = r * (s.Length-1);

            int left = (int)Mathf.Floor(rNorm);
            int right = (int)Mathf.Ceil(rNorm);

            return s[left] + (rNorm - (float)left) * (s[right]-s[left]);

        }

    }

}