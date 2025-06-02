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
        if(rigidBody.linearVelocity.sqrMagnitude < 0.01f)
        {
            transform.forward = Vector3.Lerp(transform.forward, rigidBody.linearVelocity, 0.7f);
        }
        

    }


    public Vector3 Seek(Vector3 target, float acceleration)
    {
        Vector3 toTarget = target - transform.position;

        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 accel = toTargetNormalized * acceleration;

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
        Ray WhiskerRight = new Ray (transform.position, Quaternion.Euler(new Vector3(-10, 0, 0)) * transform.forward);


        Ray WhiskerLeft = new Ray(transform.position, Quaternion.Euler(new Vector3(10, 0, 0)) * transform.forward);

        RaycastHit hitInfoLeft;
        RaycastHit hitInfoRight;

        bool didHitLeft = Physics.Raycast(WhiskerLeft,out hitInfoLeft, lookAheadDistance);


            if (didHitLeft)
            {
                accelOut = transform.right * acceleration;
                Debug.DrawLine(WhiskerRight.origin, hitInfoLeft.point, Color.red);

            }
            else
            {
            Debug.DrawRay(WhiskerLeft.origin, WhiskerLeft.direction * lookAheadDistance, Color.yellow);
            }

            bool didHitRight = Physics.Raycast(WhiskerRight, out hitInfoRight, lookAheadDistance);
        if (didHitRight)
        {
             accelOut = -transform.right * acceleration;

            Debug.DrawLine(WhiskerLeft.origin, hitInfoRight.point, Color.red);
        }
        else
        {
            Debug.DrawRay(WhiskerRight.origin, WhiskerRight.direction * lookAheadDistance, Color.yellow);
        }



            return Vector3.zero;
    }
}
