using UnityEngine;

/// <summary>
/// Controls the wandering agent's movement.
/// 
/// Remember, this is the target you're trying to hit. How can you determine where and when it is located as it wanders?
/// </summary>
public class ConstrainedWander : MonoBehaviour
{
    public int startingSeed = 131;

    public Vector3 centerPoint;
    public float wanderRadius = 40;

    public float wanderCircleRadius = 2;
    public float wanderCircleDistance = 3;
    public float wanderJitterFactor = 1000;
    public float moveSpeed = 5.0f;


    public float centerFactor, wanderFactor;
    public Vector3 seekDirection;

    private Vector3 wanderTargetDirection;

    private void Start()
    {
        wanderTargetDirection = transform.forward;
        Random.InitState(startingSeed);
    }

    private void FixedUpdate()
    {
        // Blend Wander and Seek based on distance from centerPoint

        // Update orientation of wanderTargetDirection
        wanderTargetDirection = Quaternion.AngleAxis(Random.Range(-wanderJitterFactor, wanderJitterFactor) * Time.fixedDeltaTime, Vector3.up) * wanderTargetDirection;

        Vector3 toWanderTarget = transform.forward * wanderCircleDistance + wanderTargetDirection * wanderCircleRadius;
        toWanderTarget.y = 0;

        Vector3 toCenter = centerPoint - transform.position;
        toCenter.y = 0;
        centerFactor = toCenter.magnitude / wanderRadius;
        wanderFactor = 1 - centerFactor;
        toCenter.Normalize();

        if (wanderFactor < 0)
        {
            wanderFactor = 0;
            centerFactor = 1;
        }

        seekDirection = toCenter * centerFactor + toWanderTarget.normalized * wanderFactor;
        seekDirection.Normalize();

        transform.forward = seekDirection;
        transform.position += seekDirection * moveSpeed * Time.fixedDeltaTime;

        if (Time.timeSinceLevelLoad >= 60) this.enabled = false;



        // If you're going to do things in this method, it's probably best to do them below this line,
        // and to not disrupt anything above it.


    }


}
