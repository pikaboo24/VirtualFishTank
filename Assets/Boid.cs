using UnityEngine;
using UnityEngine.Rendering;

public class Boid : MonoBehaviour
{
    
    GameObject targetObject;
    public Rigidbody rigidBody;
    public float speedMax = 2;
    public float accelMax = 3;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        
    }
    private void Start()
    {
        targetObject = GameObject.Find("target");
        rigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        AllignToVelocity();
    }
    private void FixedUpdate()
    {

        //Get displacement vector to target
        Vector3 toTarget = targetObject.transform.position - transform.position;  

        //Normalize
        Vector3 toTargetNormalized  =toTarget.normalized;

        //Determine acceleration
        Vector3 acceleration = toTargetNormalized * accelMax;


        rigidBody.linearVelocity += acceleration * Time.fixedDeltaTime;

        //Enforce a top speed
        rigidBody.linearVelocity = Vector3.ClampMagnitude(rigidBody.linearVelocity, speedMax);


    }
    public void AllignToVelocity()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, rigidBody.linearVelocity.normalized, Mathf.Deg2Rad * 1800 * Time.fixedDeltaTime, 100);
    }






}
