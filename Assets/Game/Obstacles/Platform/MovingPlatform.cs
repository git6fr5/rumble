/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class MovingPlatform : Platform {

    /* --- Static Variables --- */
    public static float SlowSpeed = 0.25f;
    public static float MidSpeed = 0.75f;
    public static float FastSpeed = 1.25f;

    /* --- Parameters --- */
    [SerializeField] private Transform[] pathPoints = null;

    /* --- Properties --- */
    [SerializeField, ReadOnly] private int pathIndex;

    /* --- Overridden Methods --- */
    protected override void Init() {
        base.Init(); // Runs the base initialization.
        pathIndex = 0;
        for (int i = 0; i < pathPoints.Length; i++) {
            pathPoints[i].transform.SetParent(null);
        }
    }

    // Sets the target for this platform.
    protected override void Target() {
        base.Target(); // Runs the base targetting.

        if (pathPoints == null || pathPoints.Length == 0) {
            return;
        }

        if ((target - transform.position).sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            pathIndex = (pathIndex + 1) % pathPoints.Length;
            target = pathPoints[pathIndex].position;
        }
    }

    /* --- Methods --- */
    public void Init(Transform endPoint, List<Transform> points, float speed) {
        this.pathPoints = points.ToArray();
        this.endPoint = endPoint;
        this.speed = speed;
        Init();
    }

}
