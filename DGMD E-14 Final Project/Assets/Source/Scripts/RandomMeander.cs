using UnityEngine;

public class RandomMeander : MonoBehaviour
{
	public bool XAxisMeander = true;
	public bool YAxisMeander = true;
	public bool ZAxisMeander = true;
	public Vector3 MaxMeanderMagnitude = new Vector3(0.25f, 0.25f, 0.25f);
	public Vector3 MaxMeanderSpeed = new Vector3(0.25f, 0.25f, 0.25f);
	private Vector3 meanderMagnitude;
	private Vector3 meanderSpeed;
	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;

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

	private void SetRandomSpeed(int axis)
	{
		if (axis < 0 || axis > 2)
		{
			return;
		}
		meanderSpeed[axis] = Random.Range(0.01f, MaxMeanderMagnitude[axis]);
	}
	private void SetRandomMagnitude(int axis)
	{
		if(axis < 0 || axis > 2)
        {
			return;
        }
		meanderMagnitude[axis] = Random.Range(0, MaxMeanderSpeed[axis]);
	}

	void FixedUpdate()
	{
		float angle = Mathf.Cos(Time.timeSinceLevelLoad);
		Vector3 translation = new Vector3(meanderMagnitude.x * meanderSpeed.x * angle, meanderMagnitude.y * meanderSpeed.y * angle, meanderMagnitude.z * meanderSpeed.z * angle);
		gameObject.transform.Translate(translation);
	}
}
