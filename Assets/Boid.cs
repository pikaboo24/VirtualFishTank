using UnityEngine;
using UnityEngine.Rendering;

public class Boid : MonoBehaviour
{
    
    public Rigidbody rigidBody;
    
    public float speedMax = 2;
    public float accelMax = 3;
   
    private void Start()
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

        transform.forward = rigidBody.linearVelocity;
       
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




}
