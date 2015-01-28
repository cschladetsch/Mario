using UnityEngine;

public class Cake : MonoBehaviour
{
	/// <summary>
	/// Parameterised position along conveyor: 0 is at start, 1 at end
	/// </summary>
	public float Position;

	/// <summary>
	/// How long to end at end of conveyor before falling
	/// </summary>
	public float HangTime = 4;

	/// <summary>
	/// True if currently hanging
	/// </summary>
	public bool Hanging { get { return _hangTimer > 0; } }

	/// <summary>
	/// True if dropped after hanging.
	/// </summary>
	public bool Dropped { get { return _hangTimer < 0; } }

	private float _hangTimer;

	internal void UpdateCake(bool moveRight)
	{
		UpdateHang(moveRight);
	}

	private void UpdateHang(bool moveRight)
	{
		if (!(_hangTimer > 0))
			return;

		transform.localRotation = Quaternion.Slerp(transform.localRotation,
			Quaternion.AngleAxis(moveRight ? -30 : 30, Vector3.forward), Time.deltaTime);

		_hangTimer -= Time.deltaTime;
		if (_hangTimer < 0)
			StartDropped();
	}

	private void StartDropped()
	{
		// TODO
		FindObjectOfType<Player>().DroppedCake();
	}

	public void StartHanging()
	{
		_hangTimer = HangTime;
	}

	public void Reset()
	{
		_hangTimer = 0;
		Position = 0;
		transform.localRotation = Quaternion.identity;
	}
}


