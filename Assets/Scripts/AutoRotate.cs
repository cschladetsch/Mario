using UnityEngine;

/// <summary>
/// Automatically and continually rotate transform around an axis at a given speed
/// </summary>
public class AutoRotate : MonoBehaviour
{
	/// <summary>
	/// The axis to rotate around
	/// </summary>
	public Vector3 Axis = Vector3.forward;

	/// <summary>
	/// How quickly to rotate
	/// </summary>
	public float Speed = 30;

	private float _angle;

	private void Update()
	{
		_angle += Time.deltaTime*Speed;

		transform.rotation = Quaternion.AngleAxis(_angle, Axis);
	}
}