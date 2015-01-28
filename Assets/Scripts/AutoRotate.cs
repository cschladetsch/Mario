using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	public Vector3 Axis = Vector3.forward;

	public float Speed = 30;

	private float _angle;

	void Update()
	{
		_angle += Time.deltaTime*Speed;

		transform.rotation = Quaternion.AngleAxis(_angle, Axis);
	}
}
