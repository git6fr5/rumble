/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ElevatorPlatform : Platform {

    /* --- Parameters --- */
    [SerializeField] private Transform[] pathPoints = null;
    [SerializeField] private float pauseDuration = 0f;

    /* --- Properties --- */
    [SerializeField, ReadOnly] private int pathIndex;
    [SerializeField, ReadOnly] private float pauseTicks;

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
        if ((target - transform.position).sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            if (container.Count > 0 && pauseTicks > pauseDuration) {
                pathIndex = (pathIndex + 1) % pathPoints.Length;
                target = pathPoints[pathIndex].position;
                pauseTicks = 0f;
            }
            else {
                target = transform.position;
                pauseTicks += Time.deltaTime;
            }
        }
    }

}
