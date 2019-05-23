using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class BarrelControl : MonoBehaviour
{

    #region Existing member variables and events (do not change)

    private TransformState root, platform, leftWall, rightWall, barrelPivot, barrel, mouth;
    private Transform shellSpawn;

    public float platformRotation;

    public GameObject shellPrefab;

    [Range(0, 45)]
    public float barrelElevation;

    public float launchSpeed = 20;

    public float cooldown = 3.0f;
    private float currentCooldown = 0;

    void Start()
    {
        shellSpawn = transform.Find("BarrelPivot").Find("Mouth").Find("ShellSpawn");
    }

    void Update()
    {
        OverrideTransforms();
        currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0) currentCooldown = 0;
    }

    public void Fire()
    {
        OverrideTransforms();
        if (currentCooldown > 0) return;
        currentCooldown = cooldown;

        // Fire a projectile now
        GameObject newShell = GameObject.Instantiate(shellPrefab, shellSpawn.position, shellSpawn.rotation) as GameObject;
        newShell.GetComponent<Rigidbody>().velocity = newShell.transform.forward * launchSpeed;
    }

    private void OverrideTransforms()
    {
        // Continuously reset the orientation of the geometry and run it all from here;
        // only thing that changes is the Y orientation of the platform, and the X orientation of the barrelPivot

        if (root == null)
        {
            transform.localRotation = Quaternion.identity;
            root = new TransformState(transform);
        }
        if (platform == null) platform = new TransformState(transform.Find("Platform"));
        if (leftWall == null) leftWall = new TransformState(platform.Find("LeftWall"));
        if (rightWall == null) rightWall = new TransformState(platform.Find("RightWall"));
        if (barrelPivot == null)
        {
            transform.Find("BarrelPivot").rotation = Quaternion.identity;
            barrelPivot = new TransformState(transform.Find("BarrelPivot"));
        }
        if (barrel == null) barrel = new TransformState(barrelPivot.Find("Barrel"));
        if (mouth == null) mouth = new TransformState(barrelPivot.Find("Mouth"));

        root.Restore();
        platform.Restore();
        leftWall.Restore();
        rightWall.Restore();
        barrelPivot.Restore();
        barrel.Restore();
        mouth.Restore();

        barrelPivot.originalTransform.Rotate(Vector3.right, barrelElevation);

        if (platformRotation < -360) platformRotation += 720;
        if (platformRotation > 360) platformRotation -= 720;

        transform.Rotate(Vector3.up, platformRotation);
    }

    #endregion

    #region Utility methods that will probably be useful to you

    /// <summary>
    /// Gets the estimated time to land for a shell fired from the cannon in its current configuration.
    /// </summary>
    /// <returns></returns>
    private float GetEstimatedShotLandingTime()
    {
        float shellRadius = shellPrefab.transform.localScale.y * 0.5f;
        Vector3 startLocation = shellSpawn.position;
        Vector3 startVelocity = shellSpawn.forward * launchSpeed;

        Vector3 currentLocation = startLocation, previousLocation = currentLocation;
        float time = 0f, timeStep = 0.01f;
        float estimatedShotTime = 0;
        while (currentLocation.y > shellRadius)
        {
            time += timeStep;
            previousLocation = currentLocation;
            currentLocation = startLocation + startVelocity * time + Vector3.down * 0.5f * 9.8f * time * time;
            estimatedShotTime = time;
            if (time > 10)
            {
                estimatedShotTime = -1;
                Debug.LogError("Time is 10 and currentLocation is " + currentLocation.ToString("F2") + "; if this has happened, you've probably bumped something in the Cannon instance and will need to revert it.");
                currentLocation = Vector3.zero;
            }
        }
        return estimatedShotTime;
    }

    /// <summary>
    /// Gets the estimated landing location for a shell fired from the cannon in its current configuration.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetEstimatedShotLandingLocation()
    {
        float shellRadius = shellPrefab.transform.localScale.y * 0.5f;
        Vector3 startLocation = shellSpawn.position;
        Vector3 startVelocity = shellSpawn.forward * launchSpeed;

        Vector3 currentLocation = startLocation, previousLocation = currentLocation;
        float time = 0f, timeStep = 0.01f;
        while (currentLocation.y > shellRadius)
        {
            time += timeStep;
            previousLocation = currentLocation;
            currentLocation = startLocation + startVelocity * time + Vector3.down * 0.5f * 9.8f * time * time;
            if (time > 10)
            {
                Debug.LogError("Time is 10 and currentLocation is " + currentLocation.ToString("F2") + "; if this has happened, you've probably bumped something in the Cannon instance and will need to revert it.");
                currentLocation = Vector3.zero;
            }
        }

        // More precise location by lerping for y = 0...
        float py = previousLocation.y, cy = currentLocation.y, tz = py / (py - cy);
        currentLocation = previousLocation + (currentLocation - previousLocation) * tz;

        return currentLocation;
    }

    #endregion


    #region Add new code here

    private void OnDrawGizmos()
    {
        // Wouldn't it be useful to *see* where and when a shot will land?
    }

    #endregion

}

#region TransformState utility class for constraining the cannon (do not change)

public class TransformState
{
    public Vector3 localPosition, localScale;
    public Quaternion localRotation;
    public Transform originalTransform;

    public TransformState(Transform t)
    {
        this.originalTransform = t;
        UpdateState();
    }

    public void Restore()
    {
        this.RestorePosition().RestoreRotation().RestoreScale();
    }

    public void UpdateState()
    {
        this.localPosition = originalTransform.localPosition;
        this.localRotation = originalTransform.localRotation;
        this.localScale = originalTransform.localScale;
    }

    public TransformState RestorePosition()
    {
        originalTransform.localPosition = localPosition;
        return this;
    }

    public TransformState RestoreRotation()
    {
        originalTransform.localRotation = localRotation;
        return this;
    }

    public TransformState RestoreScale()
    {
        originalTransform.localScale = localScale;
        return this;
    }

    public Transform Find(string child)
    {
        return originalTransform.Find(child);
    }

    public Transform transform { get { return originalTransform; } }
}

#endregion
