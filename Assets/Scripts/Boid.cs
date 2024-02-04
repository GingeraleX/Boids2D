using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidSettings boidSettings;
    public Vector3 velocity;
    
    private static readonly int Color = Shader.PropertyToID("_Color");
    private Camera screenCamera;
    private Material boidMaterial;
    private Color originalColor;

    public void Initialize(Quaternion rotation, Camera _screenCamera, BoidSettings _boidSettings)
    {
        Transform boidRendererChild = gameObject.transform.GetChild(0);
        boidMaterial = boidRendererChild.GetComponent<Renderer>().material;
        boidMaterial.color = boidMaterial.GetColor(Color) * Random.Range(0.9f, 1.1f);

        screenCamera = _screenCamera;
        boidSettings = _boidSettings;
        velocity = rotation * Vector3.up * boidSettings.maxSpeed;
    }
   
    public void UpdateBoid(List<Boid> boids)
    {
        (Vector3 separation, Vector3 alignment, Vector3 cohesion, Vector3 attraction) = CalculateBoidBehaviors(boids);

        velocity += separation;
        velocity += alignment;
        velocity += cohesion;
        velocity += attraction;

        ClampBoidVelocity();
        UpdatePosition();
        UpdateRotation();
    }
    
    (Vector3, Vector3, Vector3, Vector3) CalculateBoidBehaviors(List<Boid> boids)
    {
        Vector3 separationVelocity = Vector3.zero;
        Vector3 alignmentVelocity = Vector3.zero;
        Vector3 cohesionVelocity = Vector3.zero;
        Vector3 attractionForce = Vector3.zero;
        
        int avoidanceBoidCount = 0;
        int alignmentBoidCount = 0;
        int cohesionBoidCount = 0;
        int attractionBoidCount = 0;
        
        Vector3 currentBoidPosition = transform.position;
        Vector3 cohesionMoveTarget = Vector3.zero;
        
        Vector3 mousePosition = screenCamera.ScreenToWorldPoint(Input.mousePosition);

        foreach (Boid otherBoid in boids)
        {
            if (ReferenceEquals(gameObject, otherBoid.gameObject))
            {
                continue;
            }

            Vector3 otherBoidPosition = otherBoid.transform.position;
            float distanceToOtherBoid = Vector3.Distance(currentBoidPosition, otherBoidPosition);

            // separation
            if (distanceToOtherBoid < boidSettings.separationRange)
            {
                Vector3 otherBoidToCurrentBoid = currentBoidPosition - otherBoidPosition;
                Vector3 normalizedDirectionToTravel = otherBoidToCurrentBoid.normalized;
                normalizedDirectionToTravel /= distanceToOtherBoid;
                separationVelocity += normalizedDirectionToTravel;
                avoidanceBoidCount++;
            }

            // alignment
            if (distanceToOtherBoid < boidSettings.alignmentRange)
            {
                alignmentVelocity += otherBoid.velocity;
                alignmentBoidCount++;
            }

            // cohesion
            if (distanceToOtherBoid < boidSettings.cohesionRange)
            {
                cohesionMoveTarget += otherBoidPosition;
                cohesionBoidCount++;
            }
        }

        // attraction
        float distanceToMouse = Vector3.Distance(transform.position, mousePosition);

        if (distanceToMouse < boidSettings.attractionRange)
        {
            Vector3 directionToMouse = mousePosition - transform.position;

            if (boidSettings.type == BoidSettings.SimulationType.TwoDimensional)
            {
                directionToMouse.z = 0f;
            }

            Vector3 normalizedDirection = directionToMouse.normalized;
            normalizedDirection /= distanceToMouse;
            attractionForce += normalizedDirection;
            attractionForce.Normalize();
            attractionForce *= boidSettings.attractionFactor;

            if (boidSettings.type == BoidSettings.SimulationType.ThreeDimensionalZeroed)
            {
                Vector3 zeroedZposition = transform.position;
                zeroedZposition.z = 0f;
                transform.position = zeroedZposition;
            }
        }
        
        if (avoidanceBoidCount != 0)
        {
            separationVelocity /= avoidanceBoidCount;
            separationVelocity.Normalize();
            separationVelocity *= boidSettings.separationFactor;
        }

        if (alignmentBoidCount != 0)
        {
            alignmentVelocity /= alignmentBoidCount;
            alignmentVelocity.Normalize();
            alignmentVelocity *= boidSettings.alignmentFactor;
        }

        if (cohesionBoidCount != 0)
        {
            cohesionMoveTarget /= cohesionBoidCount;
            Vector3 cohesionDirection = cohesionMoveTarget - currentBoidPosition;
            cohesionDirection.Normalize();
            cohesionVelocity = cohesionDirection * boidSettings.cohesionFactor;
        }

        return (separationVelocity, alignmentVelocity, cohesionVelocity, attractionForce);
    }
    
    void ClampBoidVelocity()
    {
        Vector3 direction = velocity.normalized;
        float clampedSpeed = velocity.magnitude;
        clampedSpeed = Mathf.Clamp(clampedSpeed, boidSettings.minSpeed, boidSettings.maxSpeed);
        velocity = direction * clampedSpeed;
    }
    
    void UpdatePosition()
    {
        Vector3 loopPosition = default;
        transform.position += velocity * Time.deltaTime;

        // front
        if (transform.position.z > boidSettings.mapDepth)
        {
            loopPosition = new Vector3(transform.position.x, transform.position.y, -boidSettings.mapDepth);
        }
    
        // back
        if (transform.position.z < -boidSettings.mapDepth)
        {
            loopPosition = new Vector3(transform.position.x, transform.position.y, boidSettings.mapDepth);
        }

        // top
        if (transform.position.y > boidSettings.mapHeight)
        {
            loopPosition = new Vector3(transform.position.x, -boidSettings.mapHeight, transform.position.z);
        }
    
        // bottom
        if (transform.position.y < -boidSettings.mapHeight)
        {
            loopPosition = new Vector3(transform.position.x, boidSettings.mapHeight, transform.position.z);
        }
    
        // right
        if (transform.position.x > boidSettings.mapWidth)
        {
            loopPosition = new Vector3(-boidSettings.mapWidth, transform.position.y, transform.position.z);
        }
    
        // left
        if (transform.position.x < -boidSettings.mapWidth)
        {
            loopPosition = new Vector3(boidSettings.mapWidth, transform.position.y, transform.position.z);
        }

        if (loopPosition != default)
        {
            transform.position = loopPosition;
        }
    }

    
    void UpdateRotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            boidSettings.rotationSpeed * Time.deltaTime
        );
    }
}
