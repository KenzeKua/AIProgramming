using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour: MonoBehaviour
{
    [Header("The Sheep Handler")]
    public GameObject theSheepHandler;

    [Header("Target object or mouse")]
    public bool mouseTarget = false;

    public Transform targetPosition;
    private Vector3 mousePosition;

    [Header("Seeking")]
    public float mass = 15;
    public float maxVelocity = 3;
    public float maxForce = 15;

    public Vector3 velocity;
    private Vector3 desiredVelocity;
    private Vector3 steering;

    [Header("Avoidance")]
    public bool avoid = false;
    public float avoidRadius = 5;
    public float visionRange = 3;

    public float maxAvoidVelocity = 3;
    public float maxAvoidForce = 15;

    private Vector3 avoidVelocity;

    [Header("Flock Separation")]
    public bool separate = false;
    public float separateDistance = 1.5f;
    public float visionFlockmates = 3; // Not used

    public float maxSeparateVelocity = 3;
    public float maxSeparateForce = 15;

    private int separateCount = 0;
    private Vector3 separateVelocity;

    [Header("Flock Alignment")]
    public bool align = false;
    public float alignDistance = 1.5f;

    public float maxAlignVelocity = 3;
    public float maxAlignForce = 15;

    private int alignCount = 0;
    private Vector3 alignVelocity;

    [Header("Flock Cohesion")]
    public bool cohere = false;
    public float cohereDistance = 1.5f;

    public float maxCohereVelocity = 3;
    public float maxCohereForce = 15;

    private int cohereCount = 0;
    private Vector3 cohereVelocity;

    private void Start()
    {
        // Mouse target
        mousePosition = new Vector3(Random.Range(-7, 7), Random.Range(-4, 4), 0);

        // Seek
        velocity = new Vector3(Random.Range(-7, 7), Random.Range(-4, 4), 0);
        desiredVelocity = new Vector3(Random.Range(-7, 7), Random.Range(-4, 4), 0);
        steering = Vector3.zero;

        // Avoid
        avoidVelocity = Vector3.zero;

        // Separation
        separateVelocity = Vector3.zero;

        // Alignment
        alignVelocity = Vector3.zero;

        // Cohesion
        cohereVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
        }

        Seek();
        if (avoid)
        {
            Avoidance();
        }
        if (separate)
        {
            FlockSeparation();
        }
        if (align)
        {
            FlockAlignment();
        }
        if (cohere)
        {
            FlockCohesion();
        }

        steering /= mass;
        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity);

        transform.position += velocity * Time.deltaTime;
        transform.up = velocity.normalized;

        Debug.DrawRay(transform.position, velocity.normalized * visionRange, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * visionRange, Color.magenta);
    }

    void Seek()
    {
        if (!mouseTarget)
        {
            desiredVelocity = targetPosition.transform.position - transform.position;
        }
        else if (mouseTarget)
        {
            desiredVelocity = mousePosition - transform.position;
        }

        desiredVelocity = desiredVelocity.normalized * maxVelocity;

        steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
    }

    void Avoidance()
    {
        for (int i = 0; i < theSheepHandler.GetComponent<FlockHandler>().obstacles.Length; i++)
        {
            if (Vector3.Distance(transform.position + transform.up * visionRange, theSheepHandler.GetComponent<FlockHandler>().obstacles[i].transform.position) < avoidRadius)
            {
                avoidVelocity = (theSheepHandler.GetComponent<FlockHandler>().obstacles[i].transform.position - (transform.position + transform.up * visionRange)).normalized * maxAvoidVelocity;
                //avoidVelocity = Vector3.ClampMagnitude(avoidVelocity, maxAvoidForce);

                //steering = -avoidVelocity;
                steering -= avoidVelocity;

                Debug.DrawRay(transform.position + transform.up * visionRange, avoidVelocity.normalized * avoidRadius, Color.cyan);
            }
        }
    }

    void FlockSeparation()
    {
        separateCount = 0;
        separateVelocity = Vector3.zero;

        for (int i = 0; i < theSheepHandler.GetComponent<FlockHandler>().sheepList.Length; i++)
        {
            if (theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position != this.transform.position &&
                Vector3.Distance(transform.position,
                                 theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position)
                                 < separateDistance)
            {
                separateVelocity += theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position - 
                                    transform.position;

                separateCount++;
            }
        }
        separateVelocity /= separateCount;
        separateVelocity = separateVelocity.normalized * maxSeparateVelocity;
        separateVelocity = Vector3.ClampMagnitude(separateVelocity, maxSeparateForce);

        steering -= separateVelocity;
    }
    void FlockAlignment()
    {
        alignCount = 0;
        alignVelocity = Vector3.zero;

        for (int i = 0; i < theSheepHandler.GetComponent<FlockHandler>().sheepList.Length; i++)
        {
            if (theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position != this.transform.position &&
                Vector3.Distance(transform.position,
                                 theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position)
                                 < alignDistance)
            {
                alignVelocity += theSheepHandler.GetComponent<FlockHandler>().sheepList[i].GetComponent<SteeringBehaviour>().velocity;

                alignCount++;
            }
        }
        alignVelocity /= alignCount;
        alignVelocity = alignVelocity.normalized * maxAlignVelocity;
        alignVelocity = Vector3.ClampMagnitude(alignVelocity, maxAlignForce);

        steering += alignVelocity;
    }
    void FlockCohesion()
    {
        cohereCount = 0;
        cohereVelocity = Vector3.zero;

        for (int i = 0; i < theSheepHandler.GetComponent<FlockHandler>().sheepList.Length; i++)
        {
            if (Vector3.Distance(transform.position,
                                 theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position)
                                 < cohereDistance)
            {
                cohereVelocity += theSheepHandler.GetComponent<FlockHandler>().sheepList[i].transform.position;

                cohereCount++;
            }
        }
        cohereVelocity /= cohereCount;
        cohereVelocity = transform.position - cohereVelocity;
        cohereVelocity = cohereVelocity.normalized * maxCohereVelocity;
        cohereVelocity = Vector3.ClampMagnitude(cohereVelocity, maxCohereForce);

        steering += cohereVelocity;
    }
}
