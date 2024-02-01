using System.Collections.Generic;
using UnityEngine;

public class BoidSimulation : MonoBehaviour
{
    public BoidConfiguration configuration;
    
    private int lastHeight, lastWidth;
    private float screenHeigth, screenWidth;
    
    private List<Boid> boids;
    
    private Camera cam;
    
    BoidSettings CreateBoidSettings()
    {
        BoidSettings boidSettings = new BoidSettings
        {
            mapHeight = screenHeigth,
            mapWidth = screenWidth,
            
            minSpeed = configuration.minSpeed,
            maxSpeed = configuration.maxSpeed,
            rotationSpeed = configuration.rotationSpeed,
            
            separationRange = configuration.separationRange,
            alignmentRange = configuration.alignmentRange,
            cohesionRange = configuration.cohesionRange,
            
            separationFactor = configuration.separationFactor,
            alignmentFactor = configuration.alignmentFactor,
            cohesionFactor = configuration.cohesionFactor,
        };
        return boidSettings;
    }
    
    void Start()
    {
        cam = Camera.main;
        RunScreenSizeCheck();
        
        CreateBoids();
    }

    void CreateBoids()
    {
        GameObject boidsContainer = new GameObject("Boids");
        BoidSettings defaultBoidSettings = CreateBoidSettings();
        
        boids = new List<Boid>();
        Quaternion rotation = new Quaternion(-1f, 0f, 0, 1f);

        for (int i = 0; i < configuration.numberOfBoids; i++)
        {
            Vector3 position = new Vector3(Random.Range(-defaultBoidSettings.mapWidth, defaultBoidSettings.mapWidth)
                , Random.Range(-defaultBoidSettings.mapHeight, defaultBoidSettings.mapHeight), 0f);
            
            GameObject boidGameObject = Instantiate(configuration.boidPrefab, position, rotation , boidsContainer.transform);
            boidGameObject.name = "Boid " + i;

            Boid boidControllerInstance = boidGameObject.GetComponent<Boid>();
            
            boidControllerInstance.Initialize(Quaternion.identity, defaultBoidSettings);

            boids.Add(boidGameObject.GetComponent<Boid>());
        }
    }
    
    void UpdateBoidSettings()
    {
        if (boids == null) { return; }
        
        BoidSettings updatedBoidSettings = CreateBoidSettings();
        foreach (Boid boid in boids)
        {
            boid.boidSettings = updatedBoidSettings;
        }
    }

    void LateUpdate()
    {
        RunScreenSizeCheck();
        RunUpdateBoid();
    }


    void RunUpdateBoid()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].UpdateBoid(boids);
        }
    }

    void RunScreenSizeCheck()
    {
        int currentScreenHeight = Screen.height;
        int currentScreenWidth = Screen.width;

        if (currentScreenWidth != lastWidth || currentScreenHeight != lastHeight)
        {
            lastHeight = currentScreenHeight;
            lastWidth = currentScreenWidth;
            
            CalculateCameraPlane();
            UpdateBoidSettings();
        }
    }

    void CalculateCameraPlane()
    {
        float halfFieldOfView = cam.fieldOfView * 0.5f;
        float screenAspectRatio = Screen.width / (float)Screen.height;

        screenHeigth = 2.0f * Mathf.Tan(Mathf.Deg2Rad * halfFieldOfView) * cam.farClipPlane;
        screenWidth = screenHeigth * screenAspectRatio;
    }
}
