using UnityEngine;

public class Cake : Pickup
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
	public bool Hanging { get { return _hanging; } }

	public bool Moved;

	public Conveyor Conveyor;

	/// <summary>
	/// True if dropped after hanging.
	/// </summary>
	public bool Dropped { get { return _dropped; } }

	public float Width { get { return _box.bounds.extents.x*2; } }

	public float Height { get { return _box.bounds.extents.y*2; } }

	/// <summary>
	/// Where we will end up in the truck
	/// </summary>
	internal Parabola TruckParabola;

	public float _hangTimer;
	public bool _dropped;

	private BoxCollider2D _box;
	private bool _hanging;
	private bool _kine;
	private bool _firstDrop;

	void Awake()
	{
		_box = GetComponent<BoxCollider2D>();
		rigidbody2D.isKinematic = false;
	}

	/// <summary>
	/// Counts down after drop starts. Mostly for debugging
	/// </summary>
	public float _droppedTimer;

	internal void UpdateCake(bool moveRight)
	{
		if (UpdateDropped())
			return;

		UpdateHang(moveRight);
	}

	private bool UpdateDropped()
	{
		if (_droppedTimer > 0)
			_dropped = true;

		if (!Dropped) 
			return false;

		_droppedTimer -= Time.time;
		if (_droppedTimer > 0)
			return false;

		Debug.LogWarning("Dropped cake not Destroyed!");
		FindObjectOfType<Player>().DroppedCake();
		if (Conveyor != null)
			Conveyor.RemoveCake(this);
		
		Destroy(gameObject);
		return true;
	}

	private void UpdateHang(bool moveRight)
	{
		if (!_hanging)
			return;

		transform.localRotation = Quaternion.Slerp(transform.localRotation,
			Quaternion.AngleAxis(moveRight ? -30 : 30, Vector3.forward), Time.deltaTime);

		_hangTimer -= Time.deltaTime;
		if (_hangTimer < 0)
			StartDropped(moveRight);
	}

	private void StartDropped(bool moveRight)
	{
		_droppedTimer = 2;
		//Debug.Log("Dropped: " + name);
		FindObjectOfType<Player>().DroppedCake();
		_dropped = true;

		rigidbody2D.isKinematic = false;
		const float F = 120;
		var force = new Vector2(moveRight ? F : -F, -20);
		rigidbody2D.AddForce(force);
	}

	public void StartHanging()
	{
		_hangTimer = HangTime;
		_hanging = true;
	}

	public void Reset()
	{
		if (Dropped)
			return;

		//_dropped = false;
		_hangTimer = 0;
		Position = 0;
		transform.localRotation = Quaternion.identity;
		_hanging = false;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		var go = other.gameObject;
		if (Dropped && go.layer == 8)	// ground
		{
			HitGround();
			return;
		}

		if (!_firstDrop && !Dropped && go.layer == 9)	// conveyor
		{
			_firstDrop = true;
			//Debug.Log("Start: " + name);
			rigidbody2D.isKinematic = true;
			FindObjectOfType<Level>().GetConveyor(0).AddCake(this, 0.8f);
			return;
		}
	}

	private void HitGround()
	{
		Destroy(gameObject);
	}

	public void Pause(bool pause)
	{
		if (pause)
		{
			_kine = rigidbody2D.isKinematic;
			rigidbody2D.isKinematic = true;
			return;
		}

		rigidbody2D.isKinematic = _kine;
	}
}

