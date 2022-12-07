using UnityEngine;

public class RandomTransformer : MonoBehaviour
{
    public bool RandomTranslations = true;
    public bool RandomScaling = true;
    public bool RandomRotation = true;
    public bool UniformTransformation = false;
    public Vector3 MinTransformationMagnitude = new Vector3(0.25f, 0.25f, 0.25f);
    public Vector3 MaxTransformationMagnitude = new Vector3(0.25f, 0.25f, 0.25f);
    public Vector3 MinTransformationSpeed = new Vector3(0.25f, 0.25f, 0.25f);
    public Vector3 MaxTransformationSpeed = new Vector3(0.25f, 0.25f, 0.25f);
    private Vector3 transformationMagnitude;
    private Vector3 transformationSpeed;
    private const int X = 0;
    private const int Y = 1;
    private const int Z = 2;

    void Start()
    {
        transformationMagnitude = new Vector3();
        transformationSpeed = new Vector3();
        RegenerateRandomTransformation();
    }

    private void RegenerateRandomTransformation()
    {
        if (MaxTransformationMagnitude[X] != 0 && MaxTransformationSpeed[X] != 0)
        {
            SetRandomSpeed(X);
            SetRandomMagnitude(X);
        }
        if (!UniformTransformation && MaxTransformationMagnitude[Y] != 0 && MaxTransformationSpeed[Y] != 0)
        {
            SetRandomSpeed(Y);
            SetRandomMagnitude(Y);
        }
        if (!UniformTransformation && MaxTransformationMagnitude[Z] != 0 && MaxTransformationSpeed[Z] != 0)
        {
            SetRandomSpeed(Z);
            SetRandomMagnitude(Z);
        }
        if (UniformTransformation)
        {
            transformationSpeed[Y] = transformationSpeed[X];
            transformationMagnitude[Y] = transformationMagnitude[X];
            transformationSpeed[Z] = transformationSpeed[X];
            transformationMagnitude[Z] = transformationMagnitude[X];
        }

    }

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

    void FixedUpdate()
    {
        float angle = Mathf.Cos(Time.timeSinceLevelLoad);
        Vector3 transformation = new Vector3(transformationMagnitude.x * transformationSpeed.x * angle, transformationMagnitude.y * transformationSpeed.y * angle, transformationMagnitude.z * transformationSpeed.z * angle);
        if (RandomTranslations)
        {
            Vector3 translation = new Vector3(transformationMagnitude.x * transformationSpeed.x * angle, transformationMagnitude.y * transformationSpeed.y * angle, transformationMagnitude.z * transformationSpeed.z * angle);
            gameObject.transform.Translate(transformation);
        }
        if (RandomRotation)
        {
            gameObject.transform.Rotate(transformation);
        }
        if (RandomScaling)
        {
            gameObject.transform.localScale += transformation;
        }
    }
}
