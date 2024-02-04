using UnityEngine;

[CreateAssetMenu(fileName = "BoidConfiguration", menuName = "Boid/Configuration")]
public class BoidConfiguration : ScriptableObject
{
    [Header("Settings")]
    [Range(1, 500)]
    public int numberOfBoids = 350;
    public GameObject boidPrefab;
    public BoidSettings.SimulationType simulationType = BoidSettings.SimulationType.TwoDimensional;
    
    [Header("Speeds")]
    [Range(0f, 5f)]
    public float minSpeed = 0.5f;
    [Range(0f, 5f)]
    public float maxSpeed = 4f;
    [Range(0, 100)]
    public int rotationSpeed = 50;
    [Range(0f, 100f)] 
    public float sizeRandomModifier = 3f;

    [Header("Ranges")]
    [Range(0f, 3f)]
    public float separationRange = 1.5f;
    [Range(0f, 3f)]
    public float alignmentRange = 2f;
    [Range(0f, 3f)]
    public float cohesionRange = 3f;
    [Range(24.5f, 25.5f)]
    public float attractionRange = 25.01f;

    [Header("Weights")]
    [Range(0f, 1f)]
    public float separationFactor = 0.3f;
    [Range(0f, 1f)]
    public float alignmentFactor = 0.4f;
    [Range(0f, 0.2f)]
    public float cohesionFactor = 0.1f;
    [Range(0f, 10f)]
    public float attractionFactor = 0.5f;
}
