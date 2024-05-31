/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    // a single catmull-rom curve
    public struct CatmullRomCurve {
        public Vector2 p0, p1, p2, p3;
        public float alpha;

        public CatmullRomCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float alpha) {
            (this.p0, this.p1, this.p2, this.p3) = (p0, p1, p2, p3);
            this.alpha = alpha;
        }

        // Evaluates a point at the given t-value from 0 to 1
        public Vector2 GetPoint(float t) {
            // calculate knots
            const float k0 = 0;
            float k1 = GetKnotInterval(p0, p1);
            float k2 = GetKnotInterval(p1, p2) + k1;
            float k3 = GetKnotInterval(p2, p3) + k2;

            // evaluate the point
            float u = Mathf.LerpUnclamped(k1, k2, t);
            Vector2 A1 = Remap(k0, k1, p0, p1, u);
            Vector2 A2 = Remap(k1, k2, p1, p2, u);
            Vector2 A3 = Remap(k2, k3, p2, p3, u);
            Vector2 B1 = Remap(k0, k2, A1, A2, u);
            Vector2 B2 = Remap(k1, k3, A2, A3, u);
            return Remap(k1, k2, B1, B2, u);
        }

        static Vector2 Remap(float a, float b, Vector2 c, Vector2 d, float u) {
            return Vector2.LerpUnclamped(c, d, (u - a) / (b - a));
        }

        float GetKnotInterval(Vector2 a, Vector2 b) {
            return Mathf.Pow(Vector2.SqrMagnitude(a - b), 0.5f * alpha);
        }

    }

    [RequireComponent(typeof(LineRenderer))]
    public class LineAnimator : MonoBehaviour, IPositionArrayReciever {

        protected LineRenderer lineRenderer;

        Vector3[] o;

        public bool a;
        public bool b;
        public bool catmul;

        void Awake() {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
        }

        public void RecievePositions(Vector3[] positions) {
            if (catmul) {
                if (positions.Length == 4) { Catmullize(positions); }
                else {
                    _Catmullize(positions);
                }
            }
            else if (positions.Length == 3) {
                if (a) { RecievePositionsA(positions); }
                else if (b) { RecievePositionsB(positions); }
            }
            else {
                lineRenderer.positionCount = positions.Length;
                lineRenderer.SetPositions(positions);
            }
        }

        public void RecievePositionsA(Vector3[] p) {
            int subdivisions = 20;

            if (o == null || o.Length != subdivisions) {
                o = new Vector3[subdivisions];
            }

            // P = (1−t)2P1 + 2(1−t)tP2 + t2P3
            float r = 0f;
            float _s = (float)subdivisions;
            for (int j = 0; j < subdivisions; j++) {
                r = (float)j / _s;

                o[j] = (1-r)*(1-r)*p[0] + 2*(1-r)*p[1] + r*r*p[2]; 

            }

            lineRenderer.positionCount = subdivisions;
            lineRenderer.SetPositions(o);

        }

        public void RecievePositionsB(Vector3[] p) {
            int subdivisions = 5;

            if (o == null || o.Length != subdivisions) {
                o = new Vector3[subdivisions];
            }

            // P = (1−t)2P1 + 2(1−t)tP2 + t2P3
            float r = 0f;
            float _s = (float)subdivisions;
            for (int j = 0; j < subdivisions; j++) {
                r = (float)j / _s;

                o[j] = (1-r)*(1-r)*p[2] + 2*(1-r)*p[1] + r*r*p[0];
                // o[j] = (1-r)*(1-r)*p[2] + 2*(1-r)*p[1] + r*r*p[0]; 

            }

            lineRenderer.positionCount = subdivisions;
            lineRenderer.SetPositions(o);

        }

        public void Catmullize(Vector3[] p) {
            int subdivisions = 20;

            CatmullRomCurve romCurve = new CatmullRomCurve(p[0], p[1], p[2], p[3], 0.5f);

            if (o == null || o.Length != subdivisions) {
                o = new Vector3[subdivisions];
            }

            // P = (1−t)2P1 + 2(1−t)tP2 + t2P3
            float r = 0f;
            float _s = (float)subdivisions;
            for (int j = 0; j < subdivisions; j++) {
                r = (float)j / _s;

                o[j] = romCurve.GetPoint(r);
                // o[j] = (1-r)*(1-r)*p[2] + 2*(1-r)*p[1] + r*r*p[0]; 

            }

            lineRenderer.positionCount = subdivisions;
            lineRenderer.SetPositions(o);

        }

        public void _Catmullize(Vector3[] p) {
            int subdivisions = 20;

            List<CatmullRomCurve> romCurves = new List<CatmullRomCurve>();
            for (int i = 0; i < p.Length - 3; i++) {
                romCurves.Add(new CatmullRomCurve(p[i], p[i+1], p[i+2], p[i+3], 0.5f));
            }
             
            if (o == null || o.Length != subdivisions) {
                o = new Vector3[subdivisions];
            }

            o[0] = p[0];

            // P = (1−t)2P1 + 2(1−t)tP2 + t2P3
            float r = 0f;
            float _s = (float)subdivisions;
            int n = 0;
            float t = 0f;
            for (int j = 1; j < subdivisions; j++) {
                r = (float)j / _s;

                t = r * romCurves.Count;
                n = (int)Mathf.Floor(t);

                o[j] = romCurves[n].GetPoint(t - n);
                // o[j] = (1-r)*(1-r)*p[2] + 2*(1-r)*p[1] + r*r*p[0]; 

            }

            lineRenderer.positionCount = subdivisions;
            lineRenderer.SetPositions(o);

        }

        

    }

}