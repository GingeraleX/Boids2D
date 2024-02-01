using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidSettings boidSettings;
    public Vector3 velocity;
    
    private static readonly int Color = Shader.PropertyToID("_Color");

    public void Initialize(Quaternion rotation, BoidSettings defaultBoidSettings)
    {
        boidSettings = defaultBoidSettings;
        velocity = rotation * Vector3.up * boidSettings.maxSpeed;
        SetBoidColor();
    }
    
    void SetBoidColor()
    {
        Transform boidRendererChild = gameObject.transform.GetChild(0);
        Material boidMaterial = boidRendererChild.GetComponent<Renderer>().material;
        Color randomizedColor = boidMaterial.GetColor(Color) * Random.Range(0.7f, 0.9f);
        boidMaterial.SetColor(Color, randomizedColor);
        boidMaterial.color = randomizedColor;
    }

    public void UpdateBoid(List<Boid> boids)
    {
        (Vector3 separation, Vector3 alignment, Vector3 cohesion) = CalculateBoidBehaviors(boids);

        velocity += separation;
        velocity += alignment;
        velocity += cohesion;

        ClampBoidVelocity();
        UpdatePosition();
        UpdateRotation();
    }
    
    (Vector3, Vector3, Vector3) CalculateBoidBehaviors(List<Boid> boids)
    {
        Vector3 separationVelocity = Vector3.zero;
        Vector3 alignmentVelocity = Vector3.zero;
        Vector3 cohesionVelocity = Vector3.zero;

        int avoidanceBoidCount = 0;
        int alignmentBoidCount = 0;
        int cohesionBoidCount = 0;
        
        Vector3 currentBoidPosition = transform.position;
        Vector3 cohesionMoveTarget = Vector3.zero;

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

        return (separationVelocity, alignmentVelocity, cohesionVelocity);
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

        // top
        if (transform.position.y > boidSettings.mapHeight)
        {
            loopPosition = new Vector3(transform.position.x, -boidSettings.mapHeight, 0f);
        }
        
        // bottom
        if (transform.position.y < -boidSettings.mapHeight)
        {
            loopPosition = new Vector3(transform.position.x, boidSettings.mapHeight, 0f);
        }
        
        // right
        if (transform.position.x > boidSettings.mapWidth)
        {
            loopPosition = new Vector3(-boidSettings.mapWidth, transform.position.z, 0f);
        }
        
        // left
        if (transform.position.x < -boidSettings.mapWidth)
        {
            loopPosition = new Vector3(boidSettings.mapWidth, transform.position.z, 0f);
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
