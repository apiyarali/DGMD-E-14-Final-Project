using UnityEngine;

/// <summary>
/// This script component makes the entity it is attached to randomly meander along the configured axes.
/// </summary>
public class RandomMeander : MonoBehaviour
{
	[Header("X-Axis Meander")]
	[Tooltip("Whether or not to meander along the x-axis.")]
	public bool XAxisMeander = true;

	[Header("Y-Axis Meander")]
	[Tooltip("Whether or not to meander along the y-axis.")]
	public bool YAxisMeander = true;

	[Header("Z-Axis Meander")]
	[Tooltip("Whether or not to meander along the z-axis.")]
	public bool ZAxisMeander = true;

	[Header("Max Meander Magnitude")]
	[Tooltip("How far to randomly meander along each axis.")]
	public Vector3 MaxMeanderMagnitude = new Vector3(0.25f, 0.25f, 0.25f);

	[Header("Max Meander Speed")]
	[Tooltip("How fast to randomly meander along each axis.")]
	public Vector3 MaxMeanderSpeed = new Vector3(0.25f, 0.25f, 0.25f);

	/// <summary>
	/// The determined random meander magnitude for each axis.
	/// </summary>
	private Vector3 meanderMagnitude;

	/// <summary>
	/// The determined random meander speed for each axis.
	/// </summary>
	private Vector3 meanderSpeed;

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
		meanderMagnitude = new Vector3();
		meanderSpeed = new Vector3();
		meanderSpeed = new Vector3();
		meanderSpeed = new Vector3();
		if (XAxisMeander)
		{
			SetRandomSpeed(X);
			SetRandomMagnitude(X);
		}
		if (YAxisMeander)
		{
			SetRandomSpeed(Y);
			SetRandomMagnitude(Y);
		}
		if (ZAxisMeander)
		{
			SetRandomSpeed(Z);
			SetRandomMagnitude(Z);
		}
	}

	/// <summary>
	/// Assigns a random meander speed for the given access.
	/// </summary>
	/// <param name="axis">The x, y, or z axis as an integer value.</param>
	private void SetRandomSpeed(int axis)
	{
		if (axis < 0 || axis > 2)
		{
			return;
		}
		meanderSpeed[axis] = Random.Range(0.01f, MaxMeanderMagnitude[axis]);
	}

	/// <summary>
	/// Assigns a random meander magnitude for the given access.
	/// </summary>
	/// <param name="axis">The x, y, or z axis as an integer value.</param>
	private void SetRandomMagnitude(int axis)
	{
		if(axis < 0 || axis > 2)
        {
			return;
        }
		meanderMagnitude[axis] = Random.Range(0, MaxMeanderSpeed[axis]);
	}

	/// <summary>
	/// FixedUpdate will be executed once every time step is settled in Unity.
	/// </summary>
	void FixedUpdate()
	{
		float angle = Mathf.Cos(Time.timeSinceLevelLoad);
		Vector3 translation = new Vector3(meanderMagnitude.x * meanderSpeed.x * angle, meanderMagnitude.y * meanderSpeed.y * angle, meanderMagnitude.z * meanderSpeed.z * angle);
		gameObject.transform.Translate(translation);
	}
}
