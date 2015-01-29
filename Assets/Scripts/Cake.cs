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
	public bool Dropped { get { return _dropped; } }

	private float _hangTimer;

	private bool _dropped;

	void Start()
	{
		rigidbody2D.isKinematic = false;
	}

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
			StartDropped(moveRight);
	}

	private void StartDropped(bool moveRight)
	{
		FindObjectOfType<Player>().DroppedCake();
		_dropped = true;

		rigidbody2D.isKinematic = false;
		const float F = 100;
		var force = new Vector2(moveRight ? F : -F, -20);
		rigidbody2D.AddForce(force);
	}

	public void StartHanging()
	{
		_hangTimer = HangTime;
	}

	public void Reset()
	{
		_hangTimer = 0;
		Position = 0;
		_dropped = false;
		transform.localRotation = Quaternion.identity;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		var go = other.gameObject;
		if (Dropped && go.layer == 8)	// ground
		{
			//Debug.Log("Cake hit ground");
			//Destroy(parent.gameObject);
			return;
		}

		if (Dropped)
			return;

		if (go.layer == 9) // conveyor
		{
			// hit first conveyor
			//Debug.Log("Cake hit conveyor");
			rigidbody2D.isKinematic = true;
			FindObjectOfType<Level>().GetConveyor(0).AddCake(this, 0.8f);
			return;
		}
	}
}


