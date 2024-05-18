/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

namespace Platformer.Entities.Utility {

    public class ElongatableExtras : MonoBehaviour {

        [System.Serializable]
        public class OverrideLength {
            public int length;
            public GameObject gameObject;
        }

        // Overrides.
        [SerializeField]
        private OverrideLength[] m_Overrides;

        [SerializeField]
        private Light2D m_Light;

        [SerializeField]
        private Transform[] m_TileAlongPath;
        
        [SerializeField]
        private SpriteShapeController[] m_SubShapes;

        public void SetLength(int length, float height) {
            SetSubshapes(length);
            CheckForOverride(length);
            SetLightShape((float)length, height);
            TileObjects(length);
        }

        public void CheckForOverride(int length) {
            for (int i = 0; i < m_Overrides.Length; i++) {
                if (m_Overrides[i].gameObject != null) {
                    m_Overrides[i].gameObject.SetActive(length == m_Overrides[i].length);
                }
            }
        }

        public void SetSubshapes(int length) {
            if (m_SubShapes == null && m_SubShapes.Length <= 0) {
                return;
            }

            length -= 1;
            if (length <= 0) {
                return;
            }
            
            Quaternion q = transform.localRotation;
            for (int i = 0; i < m_SubShapes.Length; i++) {
                Spline subSpline = m_SubShapes[i].spline;
                subSpline.Clear();
                subSpline.InsertPointAt(0, Vector2.zero);
                subSpline.InsertPointAt(1, q * (length * Vector3.right));
                subSpline.SetTangentMode(0, ShapeTangentMode.Continuous);
                subSpline.SetTangentMode(1, ShapeTangentMode.Continuous);
            }

        }

        public void SetLightShape(float length, float height) {
            if (m_Light == null) { return; }
            print(m_Light.shapePath);

            List<Vector3> shapePath = new List<Vector3>(); //  new Vector3[4 + (int)length * 2]
            int i = 0;
            while (i < (int)length) {
                shapePath.Add(new Vector3(i, -height / 2f, 0f));
                i+=1;
            }
            while (i >= 0) {
                shapePath.Add(new Vector3(i, height / 2f, 0f));
                i-=1;
            }

            // m_Light.SetShapePath(shapePath);
            SetShapePath(m_Light, shapePath.ToArray());
            // m_Light.BroadcastMessage("UpdateMesh");
        
        }

        void SetFieldValue<T>(object obj, string name, T val) {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, val);
        }
        
        void SetShapePath(Light2D light, Vector3[] path) {
            SetFieldValue<Vector3[]>(light, "m_ShapePath",  path);
        }

        public void TileObjects(int length) {
            for (int i = 0; i < m_TileAlongPath.Length; i++) {
                for (int j = 0; j < length; j++) {
                    Transform newTile = Instantiate(m_TileAlongPath[i].gameObject).transform;
                    newTile.transform.SetParent(m_TileAlongPath[i].parent); 
                    newTile.localPosition = m_TileAlongPath[i].localPosition + Vector3.right * j; 
                }
            }
        }

    }

}