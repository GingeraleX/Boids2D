public class BoidSettings
{
    public float mapHeight;
    public float mapWidth;
    public float mapDepth;

    public SimulationType type;
    public float minSpeed;
    public float maxSpeed;
    public float sizeRandomModifier;
    public float maxSize;
    public int rotationSpeed;

    public float separationRange;
    public float alignmentRange;
    public float cohesionRange;
    public float attractionRange;

    public float separationFactor;
    public float alignmentFactor;
    public float cohesionFactor;
    public float attractionFactor;

    public enum SimulationType
    {
        TwoDimensional,
        ThreeDimensional,
        ThreeDimensionalZeroed
    }
}