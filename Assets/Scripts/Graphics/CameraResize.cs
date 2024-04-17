/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
//
// using Gobblefish.Graphics;

namespace Gobblefish.Graphics {

    ///<summary>
    /// Controls the position and quality of the camera.
    ///<summary>
    [RequireComponent(typeof(Camera))]
    public class CameraResize : MonoBehaviour {

        void Start() {

            if (Platformer.Levels.LevelManager.Instance != null) {
                
                if (Platformer.Levels.LevelManager.Instance.Sections.Count > 0) {
                    Platformer.Levels.LevelSection section = Platformer.Levels.LevelManager.Instance.Sections[0];

                    print("Camera: " + section.Width.ToString());
                    print("Camera: " + section.Height.ToString());

                    Camera cam = GetComponent<Camera>();
                    Vector2 camSize = cam.GetOrthographicDimensions();

                    print("Camera: " + camSize.x.ToString());
                    print("Camera: " + camSize.y.ToString());

                    float factor = 1f;
                    if (section.Width > camSize.x) {
                        factor = section.Width / camSize.x;
                    }
                    if (section.Width > camSize.x) {
                        factor = Mathf.Max(section.Height / camSize.y, factor);
                    }

                    cam.orthographicSize *= factor;

                }

            }
            
        }

    }

}