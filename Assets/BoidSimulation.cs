using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class BoidSimulationControl : MonoBehaviour
{

    public GameObject BoidsPrefab = null;
    public GameObject foodPrefab = null;
    public GameObject targetObject = null;
    public GameObject obstaclePrefab = null;

    public int numboidToSpawn = 10;
    public List<Boid> boids = null;

    
    public ControlMode controlMode = ControlMode.Seek;
    public enum ControlMode
    {
        Seek,
        Pursue,
        Food,
        Obstacle
    }
    private void Start()
    {
        targetObject = GameObject.Find("target");

        for (int i = 0; i < numboidToSpawn; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(0, 1.4f), Random.Range(-0.9f, 0.9f));
            Quaternion rotation = Random.rotation;

            GameObject spawnedBoid = Instantiate(BoidsPrefab, position, rotation);

            Boid boidComponent = spawnedBoid.GetComponent<Boid>();

            boidComponent.speedMax = Random.Range(0.5f, 1.5f);
            boidComponent.accelMax = Random.Range(0.5f, 1.5f);

            boids.Add(boidComponent);

            spawnedBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0f, 1f, 0.5f, 1f));

            spawnedBoid.GetComponent<Rigidbody>().linearVelocity = Random.onUnitSphere * 0.3f;

            spawnedBoid.transform.localScale *= Random.Range(0.9f, 3f);
        }

    }





    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlMode = ControlMode.Seek;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controlMode = ControlMode.Pursue;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controlMode = ControlMode.Food;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            controlMode = ControlMode.Obstacle;
        }
        if (Input.GetMouseButtonDown(0) && controlMode == ControlMode.Food)
        {
            SpawnFood();
        }
        if (Input.GetMouseButtonDown(0) && controlMode == ControlMode.Obstacle)
        {
            SpawnObstacle();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetSim();
        }
    }
   
   public void FoodArrivalBehaviour()
    {
        for (int i = 0; i < boids.Count; i++)

        {
            boids[i].currentLinearAcceleration = Vector3.zero;
            float foodSeekRadius = 0.8f;
            Collider[] colliders = Physics.OverlapSphere(boids[i].transform.position, foodSeekRadius);

            Food closestFood = null;
            float closestFoodDistance = float.PositiveInfinity;

            foreach (Collider collider in colliders)
            {
                Food food = collider.GetComponent<Food>();
                if (food != null)
                {
                    float distanceToFood = Vector3.Distance(food.transform.position, boids[i].transform.position);
                    if (distanceToFood < closestFoodDistance)
                    {
                        closestFoodDistance = distanceToFood;
                        closestFood = food;
                    }
                }
            }
            if (closestFood != null)
            {
               Vector3 accel = boids[i].Arrive(closestFood.transform.position, boids[i].accelMax, 0.05f, 0.3f);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green);
                
                if (closestFoodDistance < 0.06f)
                {
                    Destroy(closestFood.gameObject);
               
                }
            }
             



        }
    }

    private void FixedUpdate()
    { 
       for (int i = 0; i < boids.Count; i++)
        {
            boids[i].currentLinearAcceleration = Vector3.zero;
        }
        FoodArrivalBehaviour();
        foreach (Boid boid in boids)
        {
            boid.currentLinearAcceleration += boid.ObstacleAvoidance(0.4f, boid.accelMax);         
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool didHit = Physics.Raycast(ray, out hitInfo, 100);

        if (didHit)
        {
            targetObject.transform.position = hitInfo.point;
        }
        

        switch (controlMode)
        {
            case ControlMode.Seek:
                {
                    SeekModeControl();
                    break;
                }

            case ControlMode.Pursue:
                {
                    PursueModeControl();
                    break;
                }

        }

        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].currentLinearAcceleration = Vector3.ClampMagnitude(boids[i].currentLinearAcceleration, boids[i].accelMax);
            boids[i].rigidBody.linearVelocity += boids[i].currentLinearAcceleration * Time.fixedDeltaTime;

        }
    }
    private void SpawnFood()
    {
        Instantiate(foodPrefab, targetObject.transform.position, Random.rotation);
    }
    private void SpawnObstacle()
    {
        Instantiate(obstaclePrefab, targetObject.transform.position, Quaternion.identity);
    }
    private void SeekModeControl()
    {
        
        for (int i = 0; i < boids.Count; i++)
        {

            Vector3 accel;
            if (Input.GetMouseButton(0))
            {
                accel = boids[i].Seek(targetObject.transform.position, boids[i].accelMax);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green);
            }
            else if (Input.GetMouseButton(1))
            {
                accel = boids[i].Flee(targetObject.transform.position, boids[i].accelMax);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.red);
            }
            
        }
    }
    private void ResetSim()
    {
        foreach (Boid b in boids)
        {
            Destroy(b.gameObject);
        }
        boids.Clear();
        foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
        {
            Destroy(food);
        }
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(obstacle);
        }
        Start();
    }

   
    private void PursueModeControl()
    {
        for (int i = 0; i < boids.Count; i++)
        { 
            Vector3 accel;


            if (Input.GetMouseButton(0))
            {
                accel = boids[i].Pursue(targetObject.transform.position, boids[i].accelMax, boids[i].speedMax);
             boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green);

            }
            else if (Input.GetMouseButton(1))
            { 
                accel = boids[i].Evade(targetObject.transform.position, boids[i].accelMax, boids[i].speedMax);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.red);
            }
        }
    }
}