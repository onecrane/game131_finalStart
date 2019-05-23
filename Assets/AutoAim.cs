using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// You may or may not want to create a custom inspector for this class,
/// depending on your approach to solving the problem.
/// 
/// Remember: You are submitting a series of times, rotations, and elevations - not code.
/// You only need to write a custom inspector if it will be useful to you in 
/// finding valid shots.
/// </summary>
public class AutoAim : MonoBehaviour
{

    public float[] shotTimes;
    public float[] platformRotations;
    public float[] barrelElevations;

    private int shotIndex = 0;

    private BarrelControl barrelControl;

    void Start()
    {
        barrelControl = GetComponent<BarrelControl>();
    }

    void Update()
    {
        if (shotIndex >= shotTimes.Length)
        {
            this.enabled = false;
            return;
        }

        float shotTime = shotTimes[shotIndex];
        if (Time.timeSinceLevelLoad >= shotTime)
        {
            // Fire!
            barrelControl.platformRotation = platformRotations[shotIndex];
            barrelControl.barrelElevation = barrelElevations[shotIndex];
            barrelControl.Fire();
            shotIndex++;
        }

    }
}
