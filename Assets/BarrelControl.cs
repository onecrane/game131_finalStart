using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        originalTransform.localPosition = localPosition;
        return this;
    }

    public TransformState RestoreScale()
    {
        originalTransform.localPosition = localPosition;
        return this;
    }

    public Transform Find(string child)
    {
        return originalTransform.Find(child);
    }

    public Transform transform {  get { return originalTransform; } }
}


[ExecuteInEditMode]
public class BarrelControl : MonoBehaviour
{

    private TransformState platform, leftWall, rightWall, barrelPivot, barrel, mouth;

    public float platformRotation, barrelElevation;

    void Start()
    {
        platform = new TransformState(transform.Find("Platform"));
        leftWall = new TransformState(platform.Find("LeftWall"));
        rightWall = new TransformState(platform.Find("RightWall"));
        barrelPivot = new TransformState(transform.Find("BarrelPivot"));
        barrel = new TransformState(barrelPivot.Find("Barrel"));
        mouth = new TransformState(barrelPivot.Find("Mouth"));
    }

    void Update()
    {
        // Continuously reset the orientation of the geometry and run it all from here;
        // only thing that changes is the Y orientation of the platform, and the X orientation of the barrelPivot

        platform.Restore();
        leftWall.Restore();
        rightWall.Restore();
        barrelPivot.Restore();
        barrel.Restore();
        mouth.Restore();

    }
}
