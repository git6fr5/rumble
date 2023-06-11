/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityExtensions;
using UnityEngine.Rendering.Universal;

/* --- Definitions --- */
using Game = Platformer.Management.GameManager;

namespace Platformer.Visuals.Animation {

[RequireComponent(typeof(LineRenderer))]
public class TrailAnimator : MonoBehaviour {

    // The line renderer attached to this trail.
    private LineRenderer lineRenderer;

    // The width of this trail..
    [SerializeField] private float width;
    // The duration after which the trail fades. 
    [SerializeField] private float fadeInterval;
    // The precision of the trail.
    [SerializeField] private float segmentLength;
    
    // The positions along the trail.
    private List<Vector3> positions;
    // The last cached position of this transform.
    private Vector3 cachedPosition;

    // Runs once before the first frame.
    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.endWidth = 0f;
        lineRenderer.startWidth = width;
        positions = new List<Vector3>();
    }

    // Runs once every frame.
    void FixedUpdate() {
        float dx = (cachedPosition - transform.position).magnitude;
        if (dx > segmentLength) {
            AddPosition();
        }
    }

    // Adds a point
    void AddPosition() {
        positions.Insert(0, transform.position);
        cachedPosition = transform.position;
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        StartCoroutine(IERemove());
    }

    // Removes the end of the trail.
    private IEnumerator IERemove() {
        yield return new WaitForSeconds(fadeInterval);
        if (positions.Count > 0) {
            positions.RemoveAt(positions.Count - 1);
        }
        else if (positions.Count == 1) {
            positions.RemoveAt(0);
        }
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    void OnEnable() {
        positions = new List<Vector3>();
    }

}
    
}