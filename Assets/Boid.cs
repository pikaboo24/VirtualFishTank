using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Boid : MonoBehaviour
{

    public Rigidbody rigidBody;

    public float speedMax = 2;
    public float accelMax = 3;
    public Vector3 currentLinearAcceleration = Vector3.zero;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, rigidBody.linearVelocity, Color.red);

    }
    private void FixedUpdate()
    {
        float speed = rigidBody.linearVelocity.magnitude;
        if (speed > speedMax)
        {
            rigidBody.linearVelocity = rigidBody.linearVelocity * speedMax / speed;

        }
        if(rigidBody.linearVelocity.sqrMagnitude > 0.01f)
        {
            
            transform.forward = Vector3.Lerp(transform.forward, rigidBody.linearVelocity.normalized, 0.7f);
            
        }
        
        
    }


    public Vector3 Seek(Vector3 target, float acceleration)
    {
        Vector3 toTarget = target - transform.position;

        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 accel = toTargetNormalized * acceleration;

        return accel;


    }
    public Vector3 Flee(Vector3 target, float acceleration)
    {
        Vector3 fromTarget = target - transform.position;
        Vector3 fromTargetNormalized = fromTarget.normalized;
        Vector3 accel = -fromTargetNormalized * acceleration;
        return accel;
    }
    public Vector3 Pursue(Vector3 target, float acceleration, float desiredSpeed)
    {
        Vector3 toTarget = target - transform.position;

        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 desiredVelocity = toTargetNormalized * desiredSpeed;

        Vector3 deltaVel = desiredVelocity - rigidBody.linearVelocity;

        Vector3 accel = deltaVel.normalized * acceleration;
        return accel;
    }
    public Vector3 Evade(Vector3 target, float acceleration, float desiredSpeed)
    {
        Vector3 fromTarget = transform.position - target;
        Vector3 fromTargetNormalized = fromTarget.normalized;
        Vector3 desiredVelocity = fromTargetNormalized * desiredSpeed;
        Vector3 deltaVel = desiredVelocity - rigidBody.linearVelocity;
        Vector3 accel = deltaVel.normalized * acceleration;
        return accel;
    }

    public Vector3 Arrive(Vector3 target, float acceleration, float arriveInnerRadius, float arriveOuterRadius)
    {
        Vector3 toTarget = target - transform.position;

        float distance = toTarget.magnitude;

        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 desiredVelocity = toTargetNormalized * speedMax;

        float distancePercentage = 1.0f;

        if (distance < arriveInnerRadius)
        {
            desiredVelocity = Vector3.zero;
            DebugDrawing.DrawCircle(transform.position,Quaternion.Euler(-90, 0, 0), arriveInnerRadius, 8, Color.red, Time.fixedDeltaTime);
        } else if (distance < arriveOuterRadius)
        {
           
            distancePercentage = distance / arriveOuterRadius;
            desiredVelocity *= distancePercentage;
            DebugDrawing.DrawCircleDotted(transform.position, Quaternion.Euler(90, 0, 0), arriveOuterRadius, 16, 0.01f, 0.01f, Color.red, Time.fixedDeltaTime);
        }


        Vector3 deltaVel = desiredVelocity - rigidBody.linearVelocity;

        Vector3 accel = deltaVel.normalized * acceleration * distancePercentage;
   
     
        return accel;
    }
   
    public Vector3 ObstacleAvoidance(float lookAheadDistance, float acceleration)
    {
        Vector3 accelOut = Vector3.zero;

        Ray whiskerLeft = new Ray(transform.position, Quaternion.AngleAxis(-20, transform.up) * transform.forward);
        Ray whiskerRight = new Ray(transform.position, Quaternion.AngleAxis(20, transform.up) * transform.forward);

        RaycastHit hitLeft;
        RaycastHit hitRight;

        bool didHitLeft = Physics.Raycast(whiskerLeft, out hitLeft, lookAheadDistance, ~0, QueryTriggerInteraction.Collide);
        bool didHitRight = Physics.Raycast(whiskerRight, out hitRight, lookAheadDistance, ~0, QueryTriggerInteraction.Collide);

        Debug.DrawRay(whiskerLeft.origin, whiskerLeft.direction * lookAheadDistance, Color.yellow);
        Debug.DrawRay(whiskerRight.origin, whiskerRight.direction * lookAheadDistance, Color.yellow);

        if (didHitLeft && !didHitRight)
        {
            accelOut += transform.right * acceleration;
        }
        else if (didHitRight && !didHitLeft)
        {
            accelOut += -transform.right * acceleration;
        }
        else if (didHitLeft && didHitRight)
        {
            accelOut += (transform.right - transform.forward).normalized * acceleration;
        }

        return accelOut;
    }

}
