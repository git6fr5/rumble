/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Rendering {

    /// <summary>
    /// Arcade style background.
    /// </summary>
    public class GridRenderer : MonoBehaviour {

        public static GridRenderer Instance;

        /* --- Enumerations --- */
        public enum GridRenderMode {
            Point,
            Line,
            Quads
        }

        [SerializeField] private bool m_Rebuild;
        [SerializeField] GridRenderMode m_RenderMode = GridRenderMode.Point;
        [SerializeField] private int m_Interpolations;
        [SerializeField] Gradient m_ColorGradient;
        
        [SerializeField, ReadOnly] private Grid m_Grid;
        public Grid MainGrid => m_Grid;
        [SerializeField] private MeshFilter m_GridMeshFilter;
        [SerializeField] private MeshRenderer m_GridMeshRenderer;

        // Settings
        [SerializeField] private float m_PointScale;
        [SerializeField] private float m_MassPerPoint = 1;
        [Range(0.95f, 1f)] public float m_MassVelocityDamp = 0.995f; // The lower, the more snappy?
        [Range(100f, 1000f)] public float m_SpringDisplacementFactor = 100f; // The higher, the faster it snaps back (catch-all for all types of springs).
        [Range(0.95f, 1f)] public float m_SpringTaughtness = 0.995f;

        [Range(10, 100)] public int m_VerticalPrecision = 5;
        [Range(10, 100)] public int m_HorizontalPrecision = 5;
        [SerializeField] private int m_AnchorPrecision = 3;
        
        [Range(0.0001f, 0.9999f)] public float m_BorderAnchorStiffness = 0.999f; // The higher, the faster it snaps back (for these particular springs)
        [Range(0.0001f, 0.9999f)] public float m_BorderAnchorDamping = 0.001f; // The higher, the faster it snaps back (for these particular springs)
        [Range(0.001f, 0.9999f)] public float m_AnchorStiffness = 0.999f;  // The higher, the faster it snaps back (for these particular springs)
        [Range(0.0001f, 0.9999f)] public float m_AnchorDamping = 0.001f; // The higher, the faster it snaps back (for these particular springs)
        [Range(0.0001f, 0.9999f)] public float m_SpringStiffness = 0.999f;  // The higher, the faster it snaps back (for these particular springs)
        [Range(0.0001f, 0.9999f)] public float m_SpringDamping = 0.001f; // The higher, the faster it snaps back (for these particular springs)
        
        void Update() {
            if (m_Rebuild) {
                BuildGrid();
                m_Rebuild = false;
            }
        }

        public void BuildGrid() {
            m_GridMeshFilter.mesh = new Mesh();
            m_Grid = new Grid(
                m_MassPerPoint,
                m_MassVelocityDamp,
                m_VerticalPrecision,
                m_HorizontalPrecision,
                m_SpringDisplacementFactor,
                m_SpringTaughtness,
                m_BorderAnchorStiffness,
                m_BorderAnchorDamping,
                m_AnchorPrecision,
                m_AnchorStiffness,
                m_AnchorDamping,
                m_SpringStiffness,
                m_SpringDamping
            );
            m_Grid.Update(0.02f);
        }

        void FixedUpdate() {
            if (m_Grid == null) { return; }

            m_Grid.Update(Time.fixedDeltaTime);

            List<Vector3> positions;
            List<Color> colors;
            List<int> indices;
            float particleMaxSpeed = 10f;
            float size = m_PointScale / Screen.PixelSize;

            switch (m_RenderMode) {
                case GridRenderMode.Point:
                    GridToPoints(m_Grid.points, m_ColorGradient, particleMaxSpeed, out positions, out colors, out indices);
                    Render(m_GridMeshFilter, MeshTopology.Points, positions, colors, indices);
                    return;
                case GridRenderMode.Quads:
                    GridToQuads(m_Grid.points, m_ColorGradient, particleMaxSpeed, size, m_Interpolations, out positions, out colors, out indices);
                    Render(m_GridMeshFilter, MeshTopology.Triangles, positions, colors, indices);
                    return;
                default:
                    return;
            }

        }

        public static void Render(MeshFilter mf, MeshTopology mt, List<Vector3> p, List<Color> c, List<int> id) {
            mf.mesh.SetVertices(p);
            mf.mesh.SetIndices(id.ToArray(), mt, 0);
            mf.mesh.colors = c.ToArray();
        }


        // Performance is actually imperative here, so readability might take
        // a bit of a hit.
        public static void GridToPoints(PointMass[][] m, Gradient g, float v, out List<Vector3> p, out List<Color> c, out List<int> id) {
            
            float r = 0f;
            int i = 0;
            int y = m.Length;
            int x = m[0].Length;

            p = new List<Vector3>();
            c = new List<Color>();
            id = new List<int>();

            for (int j = 0; j < y; j++) {
                for (int k = 0; k < x; k++) {

                    p.Add(m[j][k].position);

                    r = Mathf.Min(1f, m[j][k].velocity.sqrMagnitude / (v * v));
                    c.Add(g.Evaluate(r));
                    
                    id.Add(i);
                    i = i + 1;

                }
            }

        }

        public static void GridToQuads(PointMass[][] m, Gradient g, float v, float s, int lp, out List<Vector3> p, out List<Color> c, out List<int> id) {
            // m is point masses
            // g is colour gradient
            // v is max velocity for color gradient evaluation
            // s is quad size
            // p is output positions
            // c is output colors
            // id is output indices
            // lp_r is the ratio of the current interpolation.
            // lp_pos is the position of this current interpolation.
            // lp_vel is the velocity of this current interpolation.

            int i = 0;
            int y_max = m.Length;
            int x_max = m[0].Length;
            float lp_r; 
            Vector3 lp_pos; 
            Vector3 lp_vel;
            float lp_s;

            p = new List<Vector3>();
            c = new List<Color>();
            id = new List<int>();

            for (int y = 0; y < y_max; y++) {
                for (int x = 0; x < x_max; x++) {
                    AddQuad(m[y][x].position, m[y][x].velocity,  g,  v,  s, ref p, ref c, ref i, ref id);

                    // Linear interpolation along the vertical axis
                    if (y > 1) {
                        for (int l = 0; l < lp; l++) {
                            lp_r = (l + 1f) / (lp + 1f);
                            lp_s = s * Mathf.Sin(Mathf.PI * lp_r) / 4f + s / 4f;
                            lp_pos = Vector3.Lerp(m[y-1][x].position, m[y][x].position, lp_r);
                            lp_vel = Vector3.Lerp(m[y-1][x].velocity, m[y][x].velocity, lp_r);
                            AddQuad(lp_pos, lp_vel,  g,  v,  lp_s, ref p, ref c, ref i, ref id);
                        }
                    }
                    
                    // Linear interpolation along the horizontal axis
                    if (x > 1) {
                        for (int l = 0; l < lp; l++) {
                            lp_r = (l + 1f) / (lp + 1f);
                            lp_s = s * Mathf.Sin(Mathf.PI * lp_r) / 4f + s / 4f;
                            lp_pos = Vector3.Lerp(m[y][x-1].position, m[y][x].position, lp_r);
                            lp_vel = Vector3.Lerp(m[y][x-1].velocity, m[y][x].velocity, lp_r);
                            AddQuad(lp_pos, lp_vel,  g,  v,  lp_s, ref p, ref c, ref i, ref id);
                        }

                    }
                
                }

            }
                            
        }

        public static void AddQuad(Vector3 pos, Vector3 vel, Gradient g, float v, float s, ref List<Vector3> p, ref List<Color> c, ref int i, ref List<int> id) {
            // The positions.
            p.Add(pos + new Vector3(-s / 2f, -s / 2f, 0f));
            p.Add(pos + new Vector3(s / 2f, -s / 2f, 0f));
            p.Add(pos + new Vector3(-s / 2f, s / 2f, 0f));
            p.Add(pos + new Vector3(s / 2f, s / 2f, 0f));

            // First triangle.
            id.Add(i + 0);
            id.Add(i + 1);
            id.Add(i + 3);

            // Second triangle.
            id.Add(i + 0);
            id.Add(i + 2);
            id.Add(i + 3);

            // Add the colors.
            float r = Mathf.Min(1f, vel.sqrMagnitude / (v * v));
            c.Add(g.Evaluate(r));
            c.Add(g.Evaluate(r));
            c.Add(g.Evaluate(r));
            c.Add(g.Evaluate(r));

            i = i + 4;
        }

        public void Spin(Vector3 position, float force, float radius, float direction) {
            if (m_Grid == null) { return; }
            
            position = position - transform.position;
            position.z = 0f;
            if (direction == 1f) {
                m_Grid.ApplyClockwiseForce(force, position, radius, 100f, 1f);
            }
            else {
                m_Grid.ApplyCounterClockwiseForce(force, position, radius, 100f, 1f);
            }
        }

        public void Implode(Vector3 position, float force, float radius, float taughtness = 1f) {
            if (m_Grid == null) { return; }
            
            position = position - transform.position;
            position.z = 0f;
            m_Grid.ApplyImplosiveForce(force, position, radius, 100f, taughtness);
        }

        public void Explode(Vector3 position, float force, float radius, float taughtness = 1f) {
            if (m_Grid == null) { return; }
            
            position = position - transform.position;
            position.z = 0f;
            m_Grid.ApplyExplosiveForce(force, position, radius, 100f, taughtness);
        }

        public void Ripple(Vector3 position, float force, float radius, float interval, int count) {
            if (m_Grid == null) { return; }
            
            position = position - transform.position;
            position.z = 0f;
            StartCoroutine(IERipple(position, force, radius, interval, count));
        }

        private IEnumerator IERipple(Vector3 position, float force, float radius, float interval, int count) {
            for (int i = 0; i < count; i++) {
                m_Grid.ApplyExplosiveForce(force, position, radius, 100f, 0.5f);
                yield return new WaitForSeconds(interval);
            }
        }

        public void Impulses(Vector3 position, float force, float radius, float interval, int count) {
            if (m_Grid == null) { return; }
            
            position = position - transform.position;
            position.z = 0f;
            StartCoroutine(IEImpulse(position, force, radius, interval, count));
        }

        private IEnumerator IEImpulse(Vector3 position, float force, float radius, float interval, int count) {
            for (int i = 0; i < count; i++) {
                m_Grid.ApplyImplosiveForce(force, position, radius, 100f, 0.5f);
                yield return new WaitForSeconds(interval);
            }
        }
        
    }

}
