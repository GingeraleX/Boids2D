using UnityEngine;

[CreateAssetMenu(fileName = "BoidConfiguration", menuName = "Boid/Configuration")]
public class BoidConfiguration : ScriptableObject
{
    [Header("Settings")]
    [Range(1, 500)]
    public int numberOfBoids = 300;
    public GameObject boidPrefab;

    [Header("Speeds")]
    [Range(0f, 5f)]
    public float minSpeed = 0.5f;
    [Range(0f, 5f)]
    public float maxSpeed = 1.5f;
    [Range(0, 100)]
    public int rotationSpeed = 50;

    [Header("Ranges")]
    [Range(0f, 3f)]
    public float separationRange = 1;
    [Range(0f, 3f)]
    public float alignmentRange = 2;
    [Range(0f, 3f)]
    public float cohesionRange = 3;

    [Header("Weights")]
    [Range(0f, 1f)]
    public float separationFactor = 0.3f;
    [Range(0f, 1f)]
    public float alignmentFactor = 0.4f;
    [Range(0f, 0.2f)]
    public float cohesionFactor = 0.1f;
}
