using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlockHandler : MonoBehaviour
{
    bool isPlaying = true;

    [Header("Sheep of Sheep")]
    public bool mouseTarget = false;
    public Transform targetPosition;

    [SerializeField] GameObject sheep;
    [SerializeField] Slider sliderSheepAmount;
    [SerializeField] int sheepAmount;

    [SerializeField] Slider sliderMass;
    [SerializeField] float mass;
    [SerializeField] Slider sliderMaxVelocity;
    [SerializeField] float maxVelocity;
    [SerializeField] Slider sliderMaxForce;
    [SerializeField] float maxForce;

    [Header("Avoidance")]
    [SerializeField] bool avoid = false;

    [SerializeField] Slider sliderAvoidRadius;
    [SerializeField] float avoidRadius = 5;
    [SerializeField] Slider sliderVisionRange;
    [SerializeField] float visionRange = 3;

    [SerializeField] Slider sliderMaxAvoidVelocity;
    [SerializeField] float maxAvoidVelocity = 3;
    [SerializeField] Slider sliderMaxAvoidForce;
    [SerializeField] float maxAvoidForce = 15;

    [Header("Separation")]
    [SerializeField] bool separate = false;
    [SerializeField] Slider sliderSeparateDistance;
    [SerializeField] float separateDistance = 1.5f;

    [SerializeField] Slider sliderMaxSeparateVelocity;
    [SerializeField] float maxSeparateVelocity = 3;
    [SerializeField] Slider sliderMaxSeparateForce;
    [SerializeField] float maxSeparateForce = 15;

    [Header("Alignment")]
    [SerializeField] bool align = false;
    [SerializeField] Slider sliderAlignDistance;
    [SerializeField] float alignDistance = 1.5f;

    [SerializeField] Slider sliderMaxAlignVelocity;
    [SerializeField] float maxAlignVelocity = 3;
    [SerializeField] Slider sliderMaxAlignForce;
    [SerializeField] float maxAlignForce = 15;

    [Header("Cohesion")]
    [SerializeField] bool cohere = false;
    [SerializeField] Slider sliderCohereDistance;
    [SerializeField] float cohereDistance = 1.5f;

    [SerializeField] Slider sliderMaxCohereVelocity;
    [SerializeField] float maxCohereVelocity = 3;
    [SerializeField] Slider sliderMaxCohereForce;
    [SerializeField] float maxCohereForce = 15;

    [Header("Min max")]
    [SerializeField] float minX = 0;
    [SerializeField] float maxX = 0;
    [SerializeField] float minY = 0;
    [SerializeField] float maxY = 0;
    [SerializeField] float minZ = 0;
    [SerializeField] float maxZ = 0;

    [Header("Obstacles")]
    public GameObject[] obstacles = new GameObject[1];
    public GameObject[] sheepList = new GameObject[1];

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            Time.timeScale = 1;
        }
        else 
        {
            Time.timeScale = 0;
        }
    }

    public void InitializeSheep()
    {
        Deinitialization();

        // Making the list of sheep
        sheepAmount = (int)sliderSheepAmount.value;
        sheepList = new GameObject[sheepAmount];

        // Other values
        mass = sliderMass.value;
        maxVelocity = sliderMaxVelocity.value;
        maxForce = sliderMaxForce.value;

        avoidRadius = sliderAvoidRadius.value;
        visionRange = sliderVisionRange.value;
        maxAvoidVelocity = sliderMaxAvoidVelocity.value;
        maxAvoidForce = sliderMaxAvoidForce.value;

        separateDistance = sliderSeparateDistance.value;
        maxSeparateVelocity = sliderMaxSeparateVelocity.value;
        maxSeparateForce = sliderMaxSeparateForce.value;

        alignDistance = sliderAlignDistance.value;
        maxAlignVelocity = sliderMaxAlignVelocity.value;
        maxAlignForce = sliderMaxAlignForce.value;

        cohereDistance = sliderCohereDistance.value;
        maxCohereVelocity = sliderMaxCohereVelocity.value;
        maxCohereForce = sliderMaxCohereForce.value;

        // Random spawn position
        float x, y, z = 0;

        for (int i = 0; i < sheepAmount; i++)
        {
            x = Random.Range(minX, maxX);
            y = Random.Range(minY, maxY);
            z = Random.Range(minZ, maxZ);

            sheepList[i] = Instantiate(sheep, new Vector3(x, y, z), Quaternion.identity);

            // Set SheepDog
            sheepList[i].GetComponent<SteeringBehaviour>().theSheepHandler = this.gameObject;

            // Targeting
            sheepList[i].GetComponent<SteeringBehaviour>().mouseTarget = mouseTarget;
            sheepList[i].GetComponent<SteeringBehaviour>().targetPosition = targetPosition;

            // Main value setting
            sheepList[i].GetComponent<SteeringBehaviour>().mass = mass;
            sheepList[i].GetComponent<SteeringBehaviour>().maxVelocity = maxVelocity;
            sheepList[i].GetComponent<SteeringBehaviour>().maxForce = maxForce;

            // Bools
            sheepList[i].GetComponent<SteeringBehaviour>().avoid = avoid;
            sheepList[i].GetComponent<SteeringBehaviour>().separate = separate;
            sheepList[i].GetComponent<SteeringBehaviour>().align = align;
            sheepList[i].GetComponent<SteeringBehaviour>().cohere = cohere;

            // Other value setting
            sheepList[i].GetComponent<SteeringBehaviour>().avoidRadius = avoidRadius;
            sheepList[i].GetComponent<SteeringBehaviour>().visionRange = visionRange;
            sheepList[i].GetComponent<SteeringBehaviour>().maxAvoidVelocity = maxAvoidVelocity;
            sheepList[i].GetComponent<SteeringBehaviour>().maxAvoidForce = maxAvoidForce;

            sheepList[i].GetComponent<SteeringBehaviour>().separateDistance = separateDistance;
            sheepList[i].GetComponent<SteeringBehaviour>().maxSeparateVelocity = maxSeparateVelocity;
            sheepList[i].GetComponent<SteeringBehaviour>().maxSeparateForce = maxSeparateForce;

            sheepList[i].GetComponent<SteeringBehaviour>().alignDistance = alignDistance;
            sheepList[i].GetComponent<SteeringBehaviour>().maxAlignVelocity = maxAlignVelocity;
            sheepList[i].GetComponent<SteeringBehaviour>().maxAlignForce = maxAlignForce;

            sheepList[i].GetComponent<SteeringBehaviour>().cohereDistance = cohereDistance;
            sheepList[i].GetComponent<SteeringBehaviour>().maxCohereVelocity = maxCohereVelocity;
            sheepList[i].GetComponent<SteeringBehaviour>().maxCohereForce = maxCohereForce;
        }
    }

    void Deinitialization()
    {
        if (sheepList.Length > 0)
        {
            for (int i = 0; i < sheepList.Length; i++)
            {
                Destroy(sheepList[i]);
            }
        }
    }

    public void SetMouseTarget(bool targeting)
    {
        mouseTarget = targeting;
    }
    public void SetAvoid(bool toAvoid)
    {
        avoid = toAvoid;
    }
    public void SetSeparation(bool toSeparate)
    {
        separate = toSeparate;
    }
    public void SetAlignment(bool toAlign)
    {
        align = toAlign;
    }
    public void SetCohesion(bool toCohere)
    {
        cohere = toCohere;
    }

    public void SetPlayPause(bool playPause)
    {
        isPlaying = playPause;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}