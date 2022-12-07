using UnityEngine;

/// <summary>
/// This component script creates random transformations of the object in world space based on the configured parameters. 
/// </summary>
public class RandomTransformer : MonoBehaviour
{
    [Header("Should the Entity Randomly Translate?")]
    [Tooltip("Whether or not to randomly translate the object along the X, Y, and/or Z axis.")]
    public bool ShouldEntityRandomlyTranslate = true;

    [Header("Should the Entity Randomly Scale?")]
    [Tooltip("Whether or not to randomly scale the object along the X, Y, and/or Z axis.")]
    public bool ShouldEntityRandomlyScale = true;

    [Header("Should the Entity Randomly Rotate?")]
    [Tooltip("Whether or not to randomly rotate the object along the X, Y, and/or Z axis.")]
    public bool ShouldEntityRandomlyRotate = true;

    [Header("Should the Transformations be Uniform?")]
    [Tooltip("Whether or not all transformations along the X, Y, and Z axis should be equal.")]
    public bool ShouldTransformationsBeUniformAlongAxis = false;

    [Tooltip("The minimum magnitude for the random transformations of the entity.")]
    public Vector3 MinTransformationMagnitude = new Vector3(0.25f, 0.25f, 0.25f);

    [Tooltip("The maximum magnitude for the random transformations of the entity.")]
    public Vector3 MaxTransformationMagnitude = new Vector3(0.25f, 0.25f, 0.25f);

    [Tooltip("The minimum speed for the random transformations of the entity.")]
    public Vector3 MinTransformationSpeed = new Vector3(0.25f, 0.25f, 0.25f);

    [Tooltip("The maximum speed for the random transformations of the entity.")]
    public Vector3 MaxTransformationSpeed = new Vector3(0.25f, 0.25f, 0.25f);

    /// <summary>
    /// The randomized magnitude of the transformations.
    /// </summary>
    private Vector3 transformationMagnitude;

    /// <summary>
    /// The randomized speed of the transformations.
    /// </summary>
    private Vector3 transformationSpeed;

    /// <summary>
    /// A constant to represent the x-axis index:
    /// </summary>
    private const int X = 0;

    /// <summary>
    /// A constant to represent the y-axis index:
    /// </summary>
    private const int Y = 1;

    /// <summary>
    /// A constant to represent the z-axis index:
    /// </summary>
    private const int Z = 2;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        transformationMagnitude = new Vector3();
        transformationSpeed = new Vector3();
        RegenerateRandomTransformation();
    }

    /// <summary>
    /// Regenerates the randomized transformations.
    /// </summary>
    private void RegenerateRandomTransformation()
    {
        if (MaxTransformationMagnitude[X] != 0 && MaxTransformationSpeed[X] != 0)
        {
            SetRandomSpeed(X);
            SetRandomMagnitude(X);
        }
        if (!ShouldTransformationsBeUniformAlongAxis && MaxTransformationMagnitude[Y] != 0 && MaxTransformationSpeed[Y] != 0)
        {
            SetRandomSpeed(Y);
            SetRandomMagnitude(Y);
        }
        if (!ShouldTransformationsBeUniformAlongAxis && MaxTransformationMagnitude[Z] != 0 && MaxTransformationSpeed[Z] != 0)
        {
            SetRandomSpeed(Z);
            SetRandomMagnitude(Z);
        }
        if (ShouldTransformationsBeUniformAlongAxis)
        {
            transformationSpeed[Y] = transformationSpeed[X];
            transformationMagnitude[Y] = transformationMagnitude[X];
            transformationSpeed[Z] = transformationSpeed[X];
            transformationMagnitude[Z] = transformationMagnitude[X];
        }

    }

    /// <summary>
    /// Given an axis, sets a random transformation speed.
    /// </summary>
    /// <param name="axis">The X, Y, or Z axis as an integer.</param>
    private void SetRandomSpeed(int axis)
    {
        if (axis < 0 || axis > 2)
        {
            return;
        }
        if (MinTransformationMagnitude[axis] < 0)
        {
            MinTransformationMagnitude[axis] = 0;
        }
        if (MaxTransformationMagnitude[axis] < MinTransformationMagnitude[axis])
        {
            MaxTransformationMagnitude[axis] = MinTransformationMagnitude[axis];
        }
        transformationSpeed[axis] = Random.Range(MinTransformationMagnitude[axis], MaxTransformationMagnitude[axis]);
    }

    /// <summary>
    /// Given an axis, sets a random transformation magnitude.
    /// </summary>
    /// <param name="axis">The X, Y, or Z axis as an integer.</param>
    private void SetRandomMagnitude(int axis)
    {
        if (axis < 0 || axis > 2)
        {
            return;
        }
        if (MinTransformationSpeed[axis] < 0)
        {
            MinTransformationSpeed[axis] = 0;
        }
        if (MaxTransformationSpeed[axis] < MinTransformationSpeed[axis])
        {
            MaxTransformationSpeed[axis] = MinTransformationSpeed[axis];
        }
        transformationMagnitude[axis] = Random.Range(0.01f, MaxTransformationSpeed[axis]);
    }

    /// <summary>
    /// FixedUpdate is called before each internal physics update.
    /// </summary>
    void FixedUpdate()
    {
        float angle = Mathf.Cos(Time.timeSinceLevelLoad);
        Vector3 transformation = new Vector3(transformationMagnitude.x * transformationSpeed.x * angle, transformationMagnitude.y * transformationSpeed.y * angle, transformationMagnitude.z * transformationSpeed.z * angle);
        if (ShouldEntityRandomlyTranslate)
        {
            Vector3 translation = new Vector3(transformationMagnitude.x * transformationSpeed.x * angle, transformationMagnitude.y * transformationSpeed.y * angle, transformationMagnitude.z * transformationSpeed.z * angle);
            gameObject.transform.Translate(transformation);
        }
        if (ShouldEntityRandomlyRotate)
        {
            gameObject.transform.Rotate(transformation);
        }
        if (ShouldEntityRandomlyScale)
        {
            gameObject.transform.localScale += transformation;
        }
    }
}
