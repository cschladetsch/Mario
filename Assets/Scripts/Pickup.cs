using UnityEngine;

/// <summary>
/// Something that can interact with the player
/// </summary>
public class Pickup : MarioObject
{
	public IngredientType Type;

	/// <summary>
	/// Parameterised position along conveyor: 0 is at start, 1 at end
	/// </summary>
	public float Position;

	/// <summary>
	/// How long to end at end of conveyor before falling
	/// </summary>
	public float HangTime = 4;

	public bool Moved;

	/// <summary>
	/// The conveyor this pickup is currently on
	/// </summary>
	public Conveyor Conveyor;

	public float _hangTimer;

	public bool _dropped;

	/// <summary>
	/// Counts down after drop starts. Mostly for debugging
	/// </summary>
	public float _droppedTimer;

	/// <summary>
	/// True if currently hanging
	/// </summary>
	public bool Hanging
	{
		get { return _hanging; }
	}

	/// <summary>
	/// True if dropped after hanging.
	/// </summary>
	public bool Dropped
	{
		get { return _dropped; }
	}

	private BoxCollider2D _box;
	private bool _hanging;
	private bool _kine;
	private bool _firstDrop;

	public float Width
	{
		get { return _box.bounds.extents.x*2; }
	}

	public float Height
	{
		get { return _box.bounds.extents.y*2; }
	}

	protected override void Construct()
	{
		base.Construct();
		_box = GetComponent<BoxCollider2D>();
		if (rigidbody2D)
			rigidbody2D.isKinematic = false;
	}

	public void Create(Transform folder)
	{
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

	internal void UpdateItem(bool moveRight)
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

		_droppedTimer -= Time.deltaTime;
		if (_droppedTimer > 0)
			return false;

		Debug.LogWarning("Dropped cake not Destroyed!");
		FindObjectOfType<Player>().DroppedCake(this);
		if (Conveyor != null)
			Conveyor.RemoveItem(this);

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

	private void OnCollisionEnter2D(Collision2D other)
	{
		var level = FindObjectOfType<Level>();

		var go = other.gameObject;
		if (Dropped && go.layer == 8) // ground
		{
			HitGround();
			return;
		}

		if (!_firstDrop && !Dropped && go.layer == 9) // conveyor
		{
			_firstDrop = true;
			rigidbody2D.isKinematic = true;
			var conveyor = level.GetConveyor(0);

			// HACKS
			if (conveyor != null)
				conveyor.AddItem(this, 0.8f);
		}
	}

	protected virtual void StartDropped(bool moveRight)
	{
	}

	private void HitGround()
	{
		Player.DroppedCake(this);

		World.CurrentLevel.TestForEnd();

		Destroy(gameObject);
	}

	public virtual bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		return true;
	}

	protected void Remove()
	{
		Debug.Log("Obj " + name + " destroyed");
		Conveyor.RemoveItem(this);

		Destroy(gameObject);
	}
}